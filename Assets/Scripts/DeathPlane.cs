using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    [Header("References")]
    public Cargo cargo;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player"))
            GameManager.Instance.ResetLevel();
        if(other.gameObject.CompareTag("Cargo")) 
            cargo.ResetPosition();
    }
}
