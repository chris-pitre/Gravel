using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExitArea : MonoBehaviour
{
    [Header("References")]
    public Material inactiveMaterial;
    public Material activeMaterial;
    public TextMeshPro prompt;

    public void SetExitActive(bool active){
        gameObject.GetComponent<MeshRenderer>().material = active ? activeMaterial : inactiveMaterial;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            prompt.text = GameManager.Instance.cargoInArea ? "(E) Exit" : "Cargo Missing!";
            prompt.enabled = true;
        }        
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            prompt.enabled = false;
        }        
    }
}
