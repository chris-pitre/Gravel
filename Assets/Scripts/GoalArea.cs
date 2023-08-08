using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [Header("References")]
    public ExitArea exitArea;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Cargo")){
            GameManager.Instance.cargoInArea = true;
            exitArea.SetExitActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Cargo")){
            GameManager.Instance.cargoInArea = false;
            exitArea.SetExitActive(false);
        }
    }

}
