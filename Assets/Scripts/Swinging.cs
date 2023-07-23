using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lr;
    public Transform tip;
    public Transform cam;
    public Transform swingDirection;
    public Transform player;
    public LayerMask grappleable;
    public PlayerMovement pm;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse1;

    [Header("Swinging")]
    private float maxSwingDistance = 25f;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Air Movement")]
    public Transform orientation;
    public Rigidbody rb;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;

    private void Update() {
        if(Input.GetKeyDown(swingKey)) StartSwing();
        if(Input.GetKeyUp(swingKey)) StopSwing();

        CheckForSwingPoints();

        if(joint != null) AirMovement();
    }

    private void LateUpdate() {
        DrawRope();
    }
    private void StartSwing(){
        if(predictionHit.point == Vector3.zero) return;
        
        if(GetComponent<Grappling>() != null)
            GetComponent<Grappling>().StopGrapple();

        pm.ResetRestrictions();
        pm.activeSwinging = true;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = tip.position;

    }
    public void StopSwing(){
        pm.activeSwinging = false;

        lr.positionCount = 0;
        Destroy(joint);
    }
    private void AirMovement(){
        if(Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if(Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        if(Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);

        if(Input.GetKey(KeyCode.Space)){
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        if(Input.GetKey(KeyCode.S)){
            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
    }
    private Vector3 currentGrapplePosition;
    void DrawRope(){
        if(!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, tip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    private void CheckForSwingPoints(){
        if(joint != null) return;
        
        RaycastHit sphereCastHit;
        Physics.SphereCast(swingDirection.position, predictionSphereCastRadius, swingDirection.forward, out sphereCastHit, maxSwingDistance, grappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, grappleable);

        Vector3 realHitPoint;

        if(raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point;
        else if(sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point;
        else
            realHitPoint = Vector3.zero;
        
        if(realHitPoint != Vector3.zero){
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        } else {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }
}
