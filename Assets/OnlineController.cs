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
    
    clsSocket clientSocket;
    IPAddress ipServer;
    clsMessaggio msgByServer;
    
    bool esito = false;
    string Address;

    // Start is called before the first frame update
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
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
                        inviaDatiServer("*JOINING*");
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

    public void btnReadyClick(){
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
                inviaDatiServer("*READY*");
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

    public void caricamento(){
        //Aggiungere i controlli al campo 


        PlayerPrefs.SetString("Address", InputField.text);
        print("Addres in ip scene: " + PlayerPrefs.GetString("Address"));
        SceneManager.LoadScene(3);

    }

    public void backToMain(){
        SceneManager.LoadScene(0);
    }
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
                        inviaDatiServer("*QUITTING*");
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
