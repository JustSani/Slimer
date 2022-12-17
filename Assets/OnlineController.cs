using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using TMPro;

public class OnlineController : MonoBehaviour
{
    public TextMeshProUGUI InputField;
    public TextMeshProUGUI serverResponse;
    public GameObject btnReady;
    public GameObject btnIndietro;
    
    clsMessaggio OperazioneSuClient = new clsMessaggio();

    clsSocket clientSocket;
    IPAddress ipServer;
    clsMessaggio msgByServer;
    
    bool esito = false;
    string Address;

    // Start is called before the first frame update
    void Start()
    {
        OperazioneSuClient.messaggio = "";
        string currentScene = SceneManager.GetActiveScene().name;

        //SE CI TROVIAMO NELLA SCENA DI LOADING, INVIAMO LA RICHIESTA DI PARTECIPAZIONE ALLA PARTITA
        if(currentScene == "LoadingMenu"){
<<<<<<< Updated upstream

            print("Scene loading:" + serverResponse.text);
            Address = PlayerPrefs.GetString("Address").ToString();
            //Address = "192.168.178.109";
=======
>>>>>>> Stashed changes
            
            if (esito){
                btnReady.SetActive(true);
                btnIndietro.SetActive(true);
            }else{
                btnReady.SetActive(false);
                btnIndietro.SetActive(true);
            }
        
        }
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        /*
                "*STAR*" ==> Loading della scena
                "*MOVE*" ==> MOVIMENTO LOCALE DI UN PLAYER ONLINE

        */

=======
        // Aspettiamo una risposta dal server
>>>>>>> Stashed changes
        if(OperazioneSuClient.messaggio != ""){
            string tipoRQ;;

            
            tipoRQ = OperazioneSuClient.messaggio.Substring(0, 6);
            print(tipoRQ);
            print("Operazione da eseguire: (" + tipoRQ + ")");
            switch (tipoRQ)
            {
                case "*STAR*":
<<<<<<< Updated upstream
                    //serverSocket.inviaMsgSERVER("Done");

=======
                    
>>>>>>> Stashed changes
                    OperazioneSuClient.messaggio = "";
                    SceneManager.LoadScene(4);
                    break;
                case "*MOVE*":
                    print(OperazioneSuClient.messaggio);
                    
                    break;
                default:
                        OperazioneSuClient.esito = "ERR_TXRQ";
                    break;
            }
        }
    }

<<<<<<< Updated upstream
    public void inviaDatiServer(string strIN){
            // Instanzio il Client Socket
            clientSocket = new clsSocket(false, Convert.ToInt16(8888), ipServer);

=======
    public void inviaDatiServer(string strIN, IPAddress server){
            // Instanzio il Client Socket            
            clientSocket = new clsSocket(false, 8888, server);
            
>>>>>>> Stashed changes
            // Invio il Messaggio al Server
            clientSocket.inviaMsgCLIENT(strIN);

            // Aspetto il Messaggio di Risposta del Server
            OperazioneSuClient = clientSocket.clientRicevi();          // --> CONTIENE RISPOSTA

            // Chiudo il Socket
            clientSocket.Dispose();

    }
    
    // comunichiamo al server che siamo pronti e ogni 200ms dobbiamo chiedere al server un aggiornamento
    public void btnReadyClick(){
<<<<<<< Updated upstream
        //MI METTO IN ASCOLTO DI UNA RISPOSTA DAL SERVER PER QUANDO INIZIA LA PARTITA
        //EFFETIVO INIZIO DEL DIALOGO TRA CLIENT E SERVER NELLA QUALE OGNIUNO ASCOLTA
        IPAddress ip;
        bool errore = false;

        
        
        try
        {
            if (serverSocket == null)
            {
            // Creo l'IP su cui attivare il Server
            ip = IPAddress.Any;

            // Creo il Server Socket
            serverSocket = new clsSocket(true, Convert.ToInt32(6969), ip);

            // Aggiungo l'Evento datiRicevuti
            serverSocket.datiRicevutiEvent += new datiRicevutiEventHandler(datiRicevuti);
            }
        }
        catch (Exception ex)
        {
            print("ATTENZIONE: " + ex.Message);
            errore = true;
        }

        if (!errore)    
        {
            // Avvio del Socket
            serverSocket.avviaServer();
            print("CLIENT LISTENING TO SERVER");
        }

        esito = false;
=======
    
        

    }

    public bool sendMsg(string s){
        bool esito = false;
>>>>>>> Stashed changes
        try
        {
            ipServer = clsAddress.cercaIP(ipPrefs());
        }
        catch (Exception ex)
        {
            ipServer = null;
            esito = false;
        }

        if (ipServer != null)
        {
            // provo a Connettermi al SERVER

            try
            {
<<<<<<< Updated upstream
                serverSocket.inviaMsgSERVER("*REAY*");
=======
                inviaDatiServer(s, ipServer);
>>>>>>> Stashed changes
                esito = true;
            }
            catch (Exception ex)
            {
                esito = false;
            }
        }
<<<<<<< Updated upstream
        if(esito){
            btnIndietro.SetActive(false);
            //Cambiare testo dell'button btnReady in "In attesa del server e testo"
                        
        }
        
=======

        return esito;
            
>>>>>>> Stashed changes
    }


    //LISTEING ALL SERVER,
    private void datiRicevuti(clsMessaggio Msg){
            
            OperazioneSuClient = Msg;
            serverSocket.inviaMsgSERVER("*TKS");
            print("Response arrivata con successo:" + OperazioneSuClient.ToString());


    }



    //fixare
    public string ipPrefs(){
        string address = PlayerPrefs.GetString("Address");
        return address.Remove(address.Length - 1).ToString();
    }
/*********************************/
//******SWITCHING TRA LE SCENE***//
/********************************/

    public void caricamento(){
        //Aggiungere i controlli al campo 
        PlayerPrefs.SetString("Address", InputField.text);
        print("Addres in ip scene: " + PlayerPrefs.GetString("Address"));
        SceneManager.LoadScene(3);
        
        bool esito = sendMsg("*TESTS");
        if (esito)
        {
            print("Esito andato con successo");
            // IN ATTESA DEI GIOCATORI COLLEGATI

            //switch di scenaManager
        }
        else{
            
            print("Esito andato NON successo");
        }

    }

    public void backToMain(){
        SceneManager.LoadScene(0);
    }
    //CHIUSURA CONNESSIONE IN CASO NON VOGLIAMO CONTINUARE
    public void backToIp(){
        SceneManager.LoadScene(2);

        //Chiusura connessione
        if(esito){
            
        }
    }
}
