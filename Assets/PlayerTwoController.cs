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
    
    Vector2 movementRequest;
    public float moveSpeed = 1f;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();


    Thread requests;
    clsSocket clientSocket;
    IPAddress ipServer;
    string msgToSend;
    bool loopRequests = false;
    bool esito;
    bool newMsg;
    Thread connessione;
    clsMessaggio OperazioneSuClient;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();


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
    void Update()
    {
        if(movementRequest != Vector2.zero){
            int count = rb.Cast(
                movementRequest, // X and Y values between-1 and 1 that rappresent the direction from the body to look fopr collisions
                movementFilter, // the settings that determine where a collision can occur on such as layers to coluide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amouynt to cast equal to the movement polus an offset
            if(count == 0){
                rb.MovePosition(rb.position + movementRequest * moveSpeed * Time.fixedDeltaTime);
            }
        }
    }


    public void AskingServer(){
        int i = 0;
        do {
            esito = false;
            MakeRequest();
            Thread.Sleep(20);
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
                newMsg = true;
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
            movementRequest.x = float.Parse(msgByServer.messaggio.Split(":")[1].Split(",")[0]);
            movementRequest.y = float.Parse(msgByServer.messaggio.Split(":")[2]);

            //Segnalo nuovo messaggio
            newMsg = true;

            // Chiudo il Socket
            clientSocket.Dispose();
    }
    
    public static string getIp(){
        string s = PlayerPrefs.GetString("Address");
        return s.Remove(s.Length - 1);
    }

}
