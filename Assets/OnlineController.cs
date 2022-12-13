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
    clsSocket serverSocket;
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

            print("Scene loading:" + serverResponse.text);
            Address = PlayerPrefs.GetString("Address").ToString();
            //Address = "192.168.178.109";
            
            if(Address == ""){
                print("Empty" + Address);
                //inputField.Focus();
            }
            else
            {
                Address = Address.Remove(Address.Length - 1);
                print("Address: " + Address);
                try
                {
                    ipServer = clsAddress.cercaIP(Address);
                }
                catch (Exception ex)
                {
                    print("Indirizzo IP non valido : " + ex.Message);
                    //inputField.Focus();
                    ipServer = null;
                }

                if (ipServer != null)
                {
                    // provo a Connettermi al SERVER

                    try
                    {
                        inviaDatiServer("*JOIN*");
                        esito = true;
                    }
                    catch (Exception ex)
                    {
                        print("ATTENZIONE: " + ex.Message);
                    }

                }

            }
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
        /*
                "*STAR*" ==> Loading della scena
                "*MOVE*" ==> MOVIMENTO LOCALE DI UN PLAYER ONLINE

        */

        if(OperazioneSuClient.messaggio != ""){
            string tipoRQ;
            string[] vDati;

            
            tipoRQ = OperazioneSuClient.messaggio.Substring(0, 6);
            print(tipoRQ);
            print("Operazione da eseguire: (" + tipoRQ + ")");
            switch (tipoRQ)
            {
                case "*STAR*":
                    //serverSocket.inviaMsgSERVER("Done");

                    OperazioneSuClient.messaggio = "";
                    SceneManager.LoadScene(4);
                    break;
                case "*MOVE*":
                    //serverSocket.inviaMsgSERVER("Done");
                    print(OperazioneSuClient.messaggio);
                    
                    break;
                default:
                        OperazioneSuClient.esito = "ERR_TXRQ";
                    break;
            }
        }
    }

    public void inviaDatiServer(string strIN){
            // Instanzio il Client Socket
            clientSocket = new clsSocket(false, Convert.ToInt16(8888), ipServer);

            // Invio il Messaggio al Server
            clientSocket.inviaMsgCLIENT(strIN);

            // Aspetto il Messaggio di Risposta del Server
            msgByServer = clientSocket.clientRicevi();

            // Aggiungo alla Lista la Risposta del Server
            print("Response: " + msgByServer.ToString());

            // Chiudo il Socket
            clientSocket.Dispose();

    }

    //NEL CASO IN CUI SIAMO PRONTI LO COMUNICHIAMO AL SERVER E CI METTIAMO IN ASCOLTO 
    //PER INIZIARE LA PARTITA
    public void btnReadyClick(){
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
        try
        {
            ipServer = clsAddress.cercaIP(Address);
        }
        catch (Exception ex)
        {
            print("Indirizzo IP non valido : " + ex.Message);
            //inputField.Focus();
            ipServer = null;
        }

        if (ipServer != null)
        {
            // provo a Connettermi al SERVER
            try
            {
                serverSocket.inviaMsgSERVER("*REAY*");
                esito = true;
            }
            catch (Exception ex)
            {
                print("ATTENZIONE: " + ex.Message);
            }            
        }
        if(esito){
            btnIndietro.SetActive(false);
            //Cambiare testo dell'button btnReady in "In attesa del server e testo"
                        
        }
        
    }


    //LISTEING ALL SERVER,
    private void datiRicevuti(clsMessaggio Msg){
            
            OperazioneSuClient = Msg;
            serverSocket.inviaMsgSERVER("*TKS");
            print("Response arrivata con successo:" + OperazioneSuClient.ToString());

        }




/*********************************/
/*********************************/
//******SWITCHING TRA LE SCENE***//
/*********************************/
/********************************/

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
        if(esito){
            if(Address == ""){
                print("Empty" + Address);
                //inputField.Focus();
            }
            else
            {
                Address = PlayerPrefs.GetString("Address").ToString();
                Address = Address.Remove(Address.Length - 1);
                
                try
                {
                    ipServer = clsAddress.cercaIP(Address);
                }
                catch (Exception ex)
                {
                    print("Indirizzo IP non valido : " + ex.Message);
                    //inputField.Focus();
                    ipServer = null;
                }

                if (ipServer != null)
                {
                    // provo a Connettermi al SERVER

                    try
                    {
                        inviaDatiServer("*QUIT*");
                        esito = true;
                    }
                    catch (Exception ex)
                    {
                        print("ATTENZIONE: " + ex.Message);
                    }


                    
                }

            }
        }
    }
}
