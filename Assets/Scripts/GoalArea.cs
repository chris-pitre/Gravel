using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [Header("References")]
    public MeshCollider leaveArea;

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Cargo"){
            Debug.Log("Cargo in Goal Area");
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Cargo"){
            Debug.Log("Cargo left Goal Area");
        }
    }

}
