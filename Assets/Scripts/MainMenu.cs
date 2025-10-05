using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Room0");
    }
 
    public void Quit()
    {
        Application.Quit();
    }
}