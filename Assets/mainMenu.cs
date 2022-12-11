using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// for input fields
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public string ip;

    public void playSingleplayer(){
        SceneManager.LoadScene(1);
    }

    public void openServerList(){
        SceneManager.LoadScene(2);
    }

    public void quitGame(){
        Application.Quit();
    }

    public void backToMain(){
        SceneManager.LoadScene(0);
    }

    public void btnCerca(){
        GameObject obj = GameObject.Find("TextInput");
        //string IpAddress = obj.GetComponent<InputField>().text;
        
        print(obj.GetComponent<InputField>());
    }
    
}
