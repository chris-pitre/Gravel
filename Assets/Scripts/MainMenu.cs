using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        GameManager.Instance.ChangeLevel(1);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
