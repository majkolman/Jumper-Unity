using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wall Running")]
    public LayerMask wallRunLayer;
    public LayerMask groundLayer;
    public float wallRunForce = 200f;
    public float wallJumpUpForce = 12f;
    public float wallJumpSideForce = 12f;
    public float maxWallRunTime = 1f;
    private float wallRunTime;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    private KeyCode wallJumpKey = KeyCode.Space;

    [Header("Wall Running Detection")]
    public float wallCheckDistance = 0.7f;
    public float wallCheckHeight = 2f;
    private RaycastHit leftWallHit;
    private bool isWallLeft;
    private RaycastHit rightWallHit;
    private bool isWallRight;
    private bool isWallRunning;

    [Header("Exiting")]
    public float exitWallTime = .1f;
    public float exitWallSpeedTime = .2f;
    private bool exitingWall;
    private float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    public Mousemovement playerCam;
    private Playermovement playerMovement;
    private Rigidbody rb;

    void Awake()
    {
        orientation = this.gameObject.transform.GetChild(0).GetChild(0).gameObject.transform;
        groundLayer = LayerMask.GetMask("groundMask");
        wallRunLayer = LayerMask.GetMask("groundMask");
        playerCam = GameObject.Find("PlayerCam").GetComponent<Mousemovement>();
    }
    void Start()
    {
        playerMovement = GetComponent<Playermovement>();
        rb = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.isWallRunning = isWallRunning;
        CheckForWall();
        StateCheck();

        if(!isWallRunning && (isWallLeft || isWallRight))
        {
            Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
            rb.AddForce(wallNormal * 10, ForceMode.Force);
        }
    }

    private void FixedUpdate()
    {
        if(isWallRunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wallRunLayer);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wallRunLayer);
    }

    private bool AboveGround()
    {
        bool notFloor = !Physics.Raycast(transform.position, Vector3.down, wallCheckHeight, groundLayer);
        Debug.Log("notFloor:" + notFloor);
        return notFloor;
    }

    private void StateCheck()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //Debug.Log("isWallRight: "+ isWallRight + " isWallLeft: " + isWallLeft + " verticalInput: " + verticalInput);
        // zakomentiral sem AboveGround() ker wallrun ni hotu delat for some reason, vse kar zdej spremeni je
        // da lahko po zidu zdej zacnes takoj tect tud ce si na tleh
        if ((isWallRight || isWallLeft) && verticalInput > 0 && /*AboveGround() &&*/ !exitingWall)
        {
            //Debug.Log("isWallRunning: " + isWallRunning);
            if(!isWallRunning)
            {
                //Debug.Log("wall run started");
                StartWallRun();
            }

            if(wallRunTime > 0)
            {
                wallRunTime -= Time.deltaTime;
            }
            
            if(wallRunTime <= 0 && isWallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(wallJumpKey)) 
            {
                WallJump();
            }

        }
        else if (exitingWall) 
        {
            if (isWallRunning) 
            {
                StopWallRun();
            }

            if (exitWallTimer > 0) 
            {
                exitWallTimer -= Time.deltaTime;
                if(isWallLeft || isWallRight) exitWallTimer = exitWallTime;
            }

            if (exitWallTimer <= 0) 
            {
                exitingWall = false;
            }
        }
        else
        {
            StopWallRun();
        }
        
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        rb.useGravity = false;
        wallRunTime = maxWallRunTime;

        playerCam.DoFov(playerCam.WallRunFov, playerCam.WallRunTransitionSpeed);
        if(isWallLeft) playerCam.DoTilt(-playerCam.WallRunTilt, playerCam.WallRunTransitionSpeed);
        else playerCam.DoTilt(playerCam.WallRunTilt, playerCam.WallRunTransitionSpeed);
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    private void WallRunningMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        if(!(isWallLeft && horizontalInput > 0) && !(isWallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void WallJump() {
        Vector3 wallNormal = isWallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);        
        rb.AddForce(forceToApply, ForceMode.Impulse);
        exitingWall = true;
        exitWallTimer = exitWallTime;
        playerMovement.exitWallSpeedTimer = exitWallSpeedTime;
    }
    

}
