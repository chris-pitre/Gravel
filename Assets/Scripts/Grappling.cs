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
    public LayerMask holdable;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    [Header("Holding")]
    public float pickupForce = 150.0f;
    public GameObject heldObj;
    public Rigidbody heldObjRB;

    [Header("Cooldown")]
    public float grapplingCooldown;
    private float grapplingCooldownTimer;

    [Header("Input")]
    public KeyCode grappleInput = KeyCode.Mouse0;


    [Header("Prediction")]
    public RaycastHit predictionHit;
    public RaycastHit predictionHoldHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;
    private bool predictionHoldable;
    private bool grappling;
    private bool holding;
    private bool reeling = false;

    private void Start() {
        pm = GetComponent<PlayerMovement>();
    }

    private void Update() {
        if(Input.GetKeyDown(grappleInput) && heldObj != null) DropObject();
        if(Input.GetKeyDown(grappleInput) && heldObj == null) StartGrapple();
        if(heldObj != null) MoveObject();

        Debug.Log(predictionHoldable);

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
        } else if(holding){
            lr.SetPosition(0, tip.position);
            lr.SetPosition(1, heldObj.transform.position);
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

        
        if (predictionHoldable){
            reeling = false;
            grapplePoint = predictionHit.transform.position;
            PickupObject(predictionHit.transform.gameObject);
        } else if(predictionHit.point != Vector3.zero && !predictionHoldable){
            grappling = true;      
            reeling = false;
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
    
    private void PickupObject(GameObject obj){
        if(obj.GetComponent<Rigidbody>()){
            heldObjRB = obj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = hold;
            heldObj = obj;

            holding = true;
        }
    }
    
    private void DropObject(){
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObj.transform.parent = null;
        heldObj = null;

        holding = false;

        StopGrapple();
    }

    private void MoveObject(){
        if(Vector3.Distance(heldObj.transform.position, hold.position) > 0.1f){
            Vector3 moveDirection = (hold.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDirection * pickupForce);
        }
    }

    private void CheckForGrapplePoints(){
        if(grappling) return;

        RaycastHit sphereCastHit;
        Physics.SphereCast(grappleDirection.position, predictionSphereCastRadius, grappleDirection.forward, out sphereCastHit, maxGrappleDistance, grappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, grappleable);

        RaycastHit sphereCastHitHold;
        Physics.SphereCast(grappleDirection.position, predictionSphereCastRadius, grappleDirection.forward, out sphereCastHitHold, maxGrappleDistance, holdable);

        RaycastHit raycastHitHold;
        Physics.Raycast(cam.position, cam.forward, out raycastHitHold, maxGrappleDistance, holdable);
        
        Vector3 realHitPoint;

        if(raycastHit.point != Vector3.zero || raycastHitHold.point != Vector3.zero){
            if(raycastHit.point != Vector3.zero){
                realHitPoint = raycastHit.point;
                predictionHit = raycastHit;
                predictionHoldable = false;
            } else {
                realHitPoint = raycastHitHold.point;
                predictionHit = raycastHitHold;
                predictionHoldable = true;
            }
        } else if(sphereCastHit.point != Vector3.zero || sphereCastHitHold.point != Vector3.zero){
            if(sphereCastHit.point != Vector3.zero){
                realHitPoint = sphereCastHit.point;
                predictionHit = sphereCastHit;
                predictionHoldable = false;
            } else {
                realHitPoint = sphereCastHitHold.point;
                predictionHit = sphereCastHitHold;
                predictionHoldable = true;
            }
        } else{
            realHitPoint = Vector3.zero;
            predictionHoldable = false;
        }
        
        if(realHitPoint != Vector3.zero){
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        } else {
            predictionPoint.gameObject.SetActive(false);
        }
    }
}
