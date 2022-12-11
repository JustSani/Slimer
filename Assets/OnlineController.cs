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

    clsSocket clientSocket;
    IPAddress ipServer;
    clsMessaggio msgByServer;
    

    // Start is called before the first frame update
    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "LoadingMenu"){
            bool esito = false;

            print("Scene loading:" + serverResponse.text);
            string Address = PlayerPrefs.GetString("Address").ToString();
            //Address = "192.168.178.109";
            
            if(Address == ""){
                print("Empty" + Address);
                //inputField.Focus();
            }
            else
            {
                Address = Address.Remove(15);

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
                        inviaDatiServer("*TEST*");
                        esito = true;
                    }
                    catch (Exception ex)
                    {
                        print("ATTENZIONE: " + ex.Message);
                    }


                    if (esito)
                    {
                        print("CONNESSO ZIO CAN");
                    }
                }

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

    public void caricamento(){
        //Aggiungere i controlli al campo 


        PlayerPrefs.SetString("Address", InputField.text);
        print("Addres in ip scene: " + PlayerPrefs.GetString("Address"));
        SceneManager.LoadScene(3);

    }

    public void backToMain(){
        SceneManager.LoadScene(0);
    }

}
