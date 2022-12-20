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

    Rigidbody2D rb;
    public GameObject playerTwo;
    Vector2 movementRequest;
    Vector2 movementRequestSaved;
    SpriteRenderer spriteRenderer;
    Animator animator;
    bool isMov;
    bool flip;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        movementRequestSaved = new Vector2(0 , 0);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
    void FixedUpdate(){
        if(isMov)
            animator.SetBool("TwoisMoving",true);
        else
            animator.SetBool("TwoisMoving",false);
        if(flip)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        if(animator.GetBool("TwoisMoving")){
            print("X:" + movementRequest.x + ",  Y:" + movementRequest.y);
            
            rb.MovePosition(movementRequest);   
        
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
                ///OperazioneSuClient.messaggio = "*ERROR*@No response";
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

            if(movementRequestSaved != movementRequest){
                isMov = true;
                print("Moving");
            }
            else{
                isMov = false;
                print("Static");
            }
            if(movementRequest.x < movementRequestSaved.x)
                flip = true;
            else if (movementRequest.x > movementRequestSaved.x)
                flip = false;
            
            movementRequestSaved = movementRequest;

            // Chiudo il Socket
            clientSocket.Dispose();
    }
    
    public static string getIp(){
        string s = PlayerPrefs.GetString("Address");
        return s.Remove(s.Length - 1);
    }

}
