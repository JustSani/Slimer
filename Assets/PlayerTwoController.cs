using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;
public class PlayerTwoController : MonoBehaviour
{
    public GameObject playerTwo;
    Vector2 movementRequest;

    Thread requests;
    clsSocket clientSocket;
    IPAddress ipServer;
    string msgToSend;
    bool loopRequests = false;
    bool esito;
    Thread connessione;
    clsMessaggio OperazioneSuClient;

    // Start is called before the first frame update
    void Awake()
    {
        
        print("ASKING FOR NEWS");
            try { ipServer = clsAddress.cercaIP(getIp()); }
            catch (Exception ex) {
                print("Indirizzo IP non valido : " + ex.Message);
                ipServer = null; }

            // Invio del primo messaggio tramite un thread
            msgToSend = "*NEWS*";
            loopRequests = true;
            requests = new Thread(new ThreadStart(AskingServer));
            requests.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(movementRequest != Vector2.zero){
            //rb.transform.position.x = movementRequest.x;
            //rb.transform.position.y = movementRequest.y;    
            print("X:" + movementRequest.x + ",  Y:" + movementRequest.y);
            playerTwo.transform.position = new Vector3(movementRequest.x, movementRequest.y);
        }
    }


    public void AskingServer(){
        int i = 0;
        do {
            esito = false;
            MakeRequest();
            Thread.Sleep(50);
            i++;
            if(!esito)
                requests.Abort();
        }while(esito);
    }
    
    public void MakeRequest(){
        if (ipServer != null)
        {
            // provo a Connettermi al SERVER
            try
            {
                inviaDatiServer(msgToSend);
                esito = true;
            }
            catch (Exception ex)
            {
                esito = false;
                print("ATTENZIONE: " + ex.Message);
                OperazioneSuClient.messaggio = "*ERROR*@No response";
            }
        }
        if(!loopRequests)
        //Uccido il thread nel caso in cui non dobbiamo richbiedere all infinito
        connessione.Abort();
    }

    public void inviaDatiServer(string strIN){
            // Instanzio il Client Socket
            clientSocket = new clsSocket(false, Convert.ToInt16(8888), ipServer);

            // Invio il Messaggio al Server
            clientSocket.inviaMsgCLIENT(strIN);

            // Aspetto il Messaggio di Risposta del Server
            clsMessaggio msgByServer = clientSocket.clientRicevi();

            // Salvo le nuove coordinate
            movementRequest.x = float.Parse(msgByServer.messaggio.Split(":")[1].Split("#")[0]);
            movementRequest.y = float.Parse(msgByServer.messaggio.Split(":")[2]);


            // Chiudo il Socket
            clientSocket.Dispose();
    }
    
    public static string getIp(){
        string s = PlayerPrefs.GetString("Address");
        return s.Remove(s.Length - 1);
    }

}
