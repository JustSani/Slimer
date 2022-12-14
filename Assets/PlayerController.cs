using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

public class PlayerController : MonoBehaviour
{
    
    IPAddress ipServer;
    clsSocket clientSocket;
    clsMessaggio msgByServer;

    public float moveSpeed = 1f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    public float collisionOffset = 0.05f;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;
    bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // BETTER FOR PHYSICS
    private void FixedUpdate() {
        if(canMove){
            // if movement input is not 0, try to move
            if(movementInput != Vector2.zero){
                bool success = TryMove(movementInput);
            
                if(!success){
                    success = TryMove(new Vector2(movementInput.x, 0));

                    if(!success){
                        success = TryMove(new Vector2(0, movementInput.y));

                    }  
                }

                animator.SetBool("isMoving",success);
            } else {
                animator.SetBool("isMoving", false);
            }

            // Set direction of sprite to movement direction
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if(movementInput.x > 0){
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove(Vector2 direction){
        if(direction != Vector2.zero){
            //check for potential collisions
            int count = rb.Cast(
                direction, // X and Y values between-1 and 1 that rappresent the direction from the body to look fopr collisions
                movementFilter, // the settings that determine where a collision can occur on such as layers to coluide with
                castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                moveSpeed * Time.fixedDeltaTime + collisionOffset); // The amouynt to cast equal to the movement polus an offset
                
            if(count == 0){ // if not we move
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

                //Invio al Server del nostro movimento
                if(SceneManager.GetActiveScene().name != "SinglePlayerTestScene"){
                    string Address = PlayerPrefs.GetString("Address").ToString();
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
                            inviaDatiServer("*SEND*@X:" + direction.x + ",Y:"+ direction.y);
                        }
                        catch (Exception ex)
                        {
                            print("ATTENZIONE: " + ex.Message);
                        }

                    }
                }
                return true;
            }
            else {
                return false;
            }
        } else // Can't move if ther's no direction to move in
            return false;
    }
    
    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
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

    void OnFire(){
         animator.SetTrigger("swordAttack");
    }

    public void SwordAttack(){
        LockMovement();
        
        if(spriteRenderer.flipX == true)
            swordAttack.AttackLeft();
        else
            swordAttack.AttackRight();
    }
    public void EndSwordAttack(){
        UnLockMovement();
        swordAttack.StopAttack();
    }

    public void LockMovement () {
        canMove = false;
    }
    public void UnLockMovement() {
        canMove = true;
    }
}
