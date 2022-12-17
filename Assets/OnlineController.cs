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

        
        

        if(esito){
            btnIndietro.SetActive(false);
            //Cambiare testo dell'button btnReady in "In attesa del server e testo"
                        
        }
        
    }


    //LISTEING ALL SERVER,
    private void datiRicevuti(clsMessaggio Msg){
            
            OperazioneSuClient = Msg;
            
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
       
    }

    
}
