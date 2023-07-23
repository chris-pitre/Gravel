using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public float sensX = 4.0f;
    public float sensY = 4.0f;
    public Transform orientation;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    private float rotX;
    private float rotY;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
    }
    
    private void CameraMovement(){
        float x = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * sensX;
        float y = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * sensY;

        rotY += x;
        rotX -= y;

        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
        orientation.rotation = Quaternion.Euler(0, rotY, 0);
    }
}
