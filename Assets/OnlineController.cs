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
    public GameObject btnReady;
    public GameObject btnIndietro;    
    public GameObject barraCaricamento;
    public TextMeshProUGUI Titolo;
    public TextMeshProUGUI rispostaServer;
    
    clsMessaggio OperazioneSuClient;
    clsSocket clientSocket;
    IPAddress ipServer;

    Thread requests;
    Thread connessione;
    string msgToSend;
    bool newMsg;

    bool esito = false;
    string Address;

    // Start is called before the first frame update
    void Start()
    { 
        OperazioneSuClient = new clsMessaggio();
        OperazioneSuClient.messaggio = "";

        if(SceneManager.GetActiveScene().name == "LoadingMenu"){
            
            // ricerca del ip in input
            try { ipServer = clsAddress.cercaIP(getIp()); }
            catch (Exception ex) {
                print("Indirizzo IP non valido : " + ex.Message);
                ipServer = null; }

            // Invio del primo messaggio tramite un thread
            msgToSend = "*TEST*";
            connessione = new Thread(new ThreadStart(MakeRequest));
            connessione.Start();

            btnReady.SetActive(false);
        }
    }    

    // Update is called once per frame
    void Update()
    { 
        /*
            "*STAR*" ==> Loading della scena
            "*MOVE*" ==> MOVIMENTO LOCALE DI UN PLAYER ONLINE
        */
        if(newMsg){                        
            string tipoRQ = OperazioneSuClient.messaggio.Split("*")[1];
            print("Operazione da eseguire: (" + tipoRQ + ")");
            switch (tipoRQ)
            {
                // Movimento di un Giocatore
                case "MOVE":
                    newMsg = false;
                    break;
                // Prima connessione riuscita con successo
                case "CONN":
                    Titolo.text = "LOBBY TROVATA";
                    rispostaServer.text = OperazioneSuClient.messaggio.Split("@")[1];
                    newMsg = false;
                    barraCaricamento.SetActive(false);
                    btnReady.SetActive(true);
                    break;
                // Tu sei pronto, in attesa di altri giocatori 
                case "READY":
                    newMsg = false;
                    Titolo.text = "ATTENDI";
                    btnReady.SetActive(false);
                    barraCaricamento.SetActive(true);
                    btnIndietro.SetActive(false);
                    

                    // Start di un Thread che fa continue richiesta al server per sapere 
                    // se ce una risposta
                    msgToSend = "*ASKING*";
                    requests = new Thread(new ThreadStart(AskingServer));
                    requests.Start();

                    break;
                case "START":
                    newMsg = false;
                    rispostaServer.text = OperazioneSuClient.messaggio.Split("@")[1];
                    //stoppo la connessione
                    requests.Abort();
                    //Switch di scena
                    SceneManager.LoadScene(4);


                break;
                
                case "WAIT":
                    rispostaServer.text = OperazioneSuClient.messaggio.Split("@")[1];
                    break;
                default:
                    newMsg = false;
                    OperazioneSuClient.esito = "ERR_TXRQ";
                    break;
            }
        }
    }

    

    //NEL CASO IN CUI SIAMO PRONTI LO COMUNICHIAMO AL SERVER E CI METTIAMO IN ASCOLTO 
    //PER INIZIARE LA PARTITA
    public void btnReadyClick(){
        
        // Invio del primo messaggio tramite un thread
        esito = false;
        msgToSend = "*READY*";
        connessione = new Thread(new ThreadStart(MakeRequest));
        connessione.Start();

    }

    ///////////////////////////////////
    //    GESTIONE CONNNESSIONI     //
    ///////////////////////////////////

    public void AskingServer(){
        int i = 0;
        do {
            esito = false;
            MakeRequest();
            Thread.Sleep(200);
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
                print("ATTENZIONE: " + ex.Message);
            }
        }
    }

    public void inviaDatiServer(string strIN){
            // Instanzio il Client Socket
            clientSocket = new clsSocket(false, Convert.ToInt16(8888), ipServer);

            // Invio il Messaggio al Server
            clientSocket.inviaMsgCLIENT(strIN);

            // Aspetto il Messaggio di Risposta del Server
            clsMessaggio msgByServer = clientSocket.clientRicevi();

            // METTO IN LISTA LA NUOVA TASK DA ESEGUIRE
            OperazioneSuClient.messaggio = msgByServer.messaggio;

            //Segnalo nuovo messaggio
            newMsg = true;

            // Chiudo il Socket
            clientSocket.Dispose();

            //Uccido il thread
            connessione.Abort();
    }

    public string getIp(){
        string s = PlayerPrefs.GetString("Address");
        return s.Remove(s.Length - 1);
    }


    ///////////////////////////////////
    //    SWITCHING TRA LE SCENE     //
    ///////////////////////////////////

    public void caricamento(){
        //Aggiungere i controlli al campo 
        PlayerPrefs.SetString("Address", InputField.text);
        print("Addres in ip scene: " + PlayerPrefs.GetString("Address"));
        SceneManager.LoadScene(3);

    }

    public void backToMain(){
        SceneManager.LoadScene(0);
    }
    //CHIUSURA CONNESSIONE IN CASO NON VOGLIAMO CONTINUARE
    public void backToIp(){
        SceneManager.LoadScene(2);

        //Chiusura connessione
    }

    
}
