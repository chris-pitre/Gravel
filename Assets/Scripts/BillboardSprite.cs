using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    [Header("Settings")]
    public bool freezeXZAxis = true;
    private void Update() {
        if(freezeXZAxis){
            transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);   
        } else {
            transform.rotation = cam.transform.rotation;
        }
    }
}
