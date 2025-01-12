using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform cameraPos;
    public Mousemovement playerCam;

    [Header("Wall Climb Settings")]
    public float WallClimbUpForce = 1000f;
    public float WallClimbMaxDistance = 1f;
    public float playerHeight = 3f;
    public float playerRadius = 1f;
    public float WallLerpDuration = 0.5f;
    private bool isWallClimbing = false;
    public bool OrientationHitWall = false;
    private bool CameraHitWall = false;
    private bool isLerping = false;
    private RaycastHit OrientationHit;
    private RaycastHit CameraHit;

    [Header("Input")]
    private float verticalInput;
    Rigidbody rb;

    public Animator animator;
    public bool climbAnim;

    void Start()
    {
        
        orientation = GameObject.Find("Orientation").transform;
        cameraPos = GameObject.Find("CameraPos").transform;
        rb = GetComponent<Rigidbody>();
        playerCam = GameObject.Find("PlayerCam").GetComponent<Mousemovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(isLerping) return;
        if (climbAnim) return;

        OrientationHitWall = Physics.Raycast(orientation.position, orientation.forward, out OrientationHit, WallClimbMaxDistance);
        CameraHitWall = Physics.Raycast(cameraPos.position, cameraPos.forward, out CameraHit, WallClimbMaxDistance);

        verticalInput = Input.GetAxisRaw("Vertical");

        if(OrientationHitWall && OrientationHit.collider.CompareTag("Wall") && CameraHitWall && CameraHit.collider.CompareTag("Wall"))
        {
            isWallClimbing = true;
        }
        else if(OrientationHitWall && OrientationHit.collider.CompareTag("Wall") && !CameraHitWall /*&& Input.GetKey(KeyCode.Space)*/)
        {
            isWallClimbing = false;
            playerCam.CameraShakeWallLerp();
            Climb();
            return;
        }else{
            isWallClimbing = false;
        }

        if(isWallClimbing && verticalInput > 0 && Input.GetKey(KeyCode.Space))
        {
            WallClimbUp();
        }else if(isWallClimbing){
            // Push away from wall
            rb.AddForce(-orientation.forward * 10, ForceMode.Force);
        }

        
        if(Physics.Raycast(orientation.position, -orientation.forward, out RaycastHit hit, WallClimbMaxDistance)){
            // Push away from wall
            if(hit.collider.CompareTag("Wall")) rb.AddForce(orientation.forward * 10, ForceMode.Force);
        }
        
            
    }

    void WallClimbUp()
    {
        rb.velocity = new Vector3(0.001f, 0.001f, 0.001f);
        rb.AddForce(orientation.up * WallClimbUpForce * Time.deltaTime, ForceMode.Impulse);
    }

    void Climb()
    {
        rb.isKinematic = true;
        climbAnim = true;
        animator.SetBool("canClimb", climbAnim);
        Debug.Log(climbAnim);
        /*if (Physics.Raycast(OrientationHit.point + (orientation.forward * playerRadius) + (Vector3.up * 0.6f * playerHeight), Vector3.down, out var secondHit, playerHeight) )
        {
            StartCoroutine(LerpClimb(secondHit.point, WallLerpDuration));
            isLerping = true;
        }*/
    }

   

    /*IEnumerator LerpClimb(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        isLerping = false;
    }*/
    
}
