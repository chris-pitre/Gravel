using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Header("References")]
    public Transform origin;

    public void ResetPosition(){
        transform.position = origin.transform.position;
    }
}
