using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Animator transition;

    [Header("Level Manager")]
    public bool cargoInArea = false;
    public float transitionTime = 1f;

    private static GameManager _instance;
    public static GameManager Instance{ get { return _instance; } }

    private void Awake() {
        if(_instance != null && _instance != this){
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }

    public void ChangeToNextLevel(){
        StartCoroutine(ChangeLevelCoroutine(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void ResetLevel(){
        StartCoroutine(ChangeLevelCoroutine(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator ChangeLevelCoroutine(int levelIndex){
        transition.SetTrigger("StartFade");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
