using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform tip;
    public Transform grappleDirection;
    public LayerMask grappleable;
    public LineRenderer lr;
    public Transform hold;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    [Header("Holding")]
    public float pickupRange = 5.0f;
    public float pickupForce = 150.0f;

    [Header("Cooldown")]
    public float grapplingCooldown;
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode grappleInput = KeyCode.Mouse0;


    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;


    private bool grappling;
    private bool reeling = false;

    private GameObject heldObj;
    private Rigidbody heldObjRB;

    private void Start() {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update() {
        if(Input.GetKeyDown(grappleInput)) StartGrapple();

        CheckForGrapplePoints();

        if(grapplingCooldownTimer > 0)
            grapplingCooldownTimer -= Time.deltaTime;
    }

    private void LateUpdate() {
        DrawRope();
    }
    private Vector3 currentGrapplePosition;
    void DrawRope(){
        if(grappling){
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

            lr.SetPosition(0, tip.position);
            lr.SetPosition(1, currentGrapplePosition);
        } else if(reeling){
            currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, tip.position, Time.deltaTime * 8f);

            lr.SetPosition(0, tip.position);
            lr.SetPosition(1, currentGrapplePosition);

            if(Vector3.Distance(currentGrapplePosition, tip.position) <= 1f){
                reeling = false;
                lr.enabled = false;
            }
        }

    }

    private void StartGrapple(){
        if(grapplingCooldownTimer > 0) return;
        GetComponent<Swinging>().StopSwing();
        grappling = true;      
        reeling = false;

        if(predictionHit.point != Vector3.zero){
            grapplePoint = predictionHit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        } else {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        currentGrapplePosition = tip.position;
    }

    private void ExecuteGrapple(){

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple(){
        grappling = false;
        reeling = true;

        grapplingCooldownTimer = grapplingCooldown;
    }
    private void CheckForGrapplePoints(){
        if(grappling) return;
        
        RaycastHit sphereCastHit;
        Physics.SphereCast(grappleDirection.position, predictionSphereCastRadius, grappleDirection.forward, out sphereCastHit, maxGrappleDistance, grappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, grappleable);

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
