using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        GameManager.Instance.ChangeToNextLevel();
    }

    public void QuitGame(){
        Application.Quit();
    }
}
