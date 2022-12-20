using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : OnlineController
{
    public GameObject slime;
    Animator animator;
    public float Health {
        set {
            health = value;
            if(health <= 0){
                Defeated();
            }
        }
        get {
            return health;
        }
    }
    public string name(){
        return slime.name;
    }
    public float health = 1;

    public void Start(){
        animator = GetComponent<Animator>();
    }
    public void Defeated(){
        animator.SetTrigger("Defeated");
    }

    public void RemoveEnemy(){
        SlimeNumber -= 1;
        Destroy(gameObject);

    }
}
