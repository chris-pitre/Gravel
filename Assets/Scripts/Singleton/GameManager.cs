using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool cargoInArea = false;
    public int levelNum;

    private static GameManager _instance;
    public static GameManager Instance{ get { return _instance; } }

    private void Awake() {
        if(_instance != null && _instance != this){
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void ChangeLevel(){
        
    }
}
