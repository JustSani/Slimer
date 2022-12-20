using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System;
using System.Linq;
using System.Net.Sockets;

public class SwordAttack : MonoBehaviour
{
    IPAddress ipServer;
    clsSocket clientSocket;
    clsMessaggio msgByServer;

    public Collider2D swordCollider;
    Vector2 rightAttackOffset;
    
    public float damage = 3;
    private void Start(){
        rightAttackOffset = transform.position;
    }

    public void AttackRight() {
        print("Attack right");
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }

    public void AttackLeft() {
        print("Attack left");
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack (){
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy"){
            // Deal damage to the enemy
            Enemy enemy = other.GetComponent<Enemy>();

            if(enemy != null){
                enemy.Health -= damage;

                print("Enemy name:" + enemy.name());

                //Invio al Server del nostro movimento
                if(SceneManager.GetActiveScene().name != "SinglePlayerTestScene"){
                    string Address = PlayerPrefs.GetString("Address").ToString();
                    Address = Address.Remove(Address.Length - 1);
                    print("Address: " + Address + ", len is " + Address.Length);
                    try
                    {
                        ipServer = clsAddress.cercaIP(Address);
                    }
                    catch (Exception ex)
                    {
                        print("Indirizzo IP non valido : " + ex.Message);
                        SceneManager.LoadScene(0);

                        //inputField.Focus();
                        ipServer = null;
                    }

                    if (ipServer != null)
                    {
                        // provo a Connettermi al SERVER

                        try
                        {
                            inviaDatiServer("*SEND*@KILLED-"+ enemy.name() + "-");
                        }
                        catch (Exception ex)
                        {
                            print("ATTENZIONE: " + ex.Message);
                            SceneManager.LoadScene(0);
                        }

                    }
                }

            }
        }
    }

    public void inviaDatiServer(string strIN){
        // Instanzio il Client Socket
        clientSocket = new clsSocket(false, 8888, ipServer);

        print("Sended: " + ipServer.ToString());
        // Invio il Messaggio al Server
        clientSocket.inviaMsgCLIENT(strIN);
        // Aspetto il Messaggio di Risposta del Server
        msgByServer = clientSocket.clientRicevi();

        // Aggiungo alla Lista la Risposta del Server
        print("Response: " + msgByServer.ToString());

        // Chiudo il Socket
        clientSocket.Dispose();

    }

}
