using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ExitMenuScript : MonoBehaviour
{
    public GameObject Canvas;
    bool menu;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    

    public void btnSiClick(){
        SceneManager.LoadScene(0);
    }
    public void btnNoClick(){
    }
}
