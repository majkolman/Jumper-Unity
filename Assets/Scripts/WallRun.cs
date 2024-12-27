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
    public float maxWallRunTime = 5f;
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
    private bool exitingWall;
    private float exitWallTime;
    private float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    private Playermovement playerMovement;
    private Rigidbody rb;


    void Start()
    {
        playerMovement = GetComponent<Playermovement>();
        rb = GetComponent<Rigidbody>();
        orientation = this.gameObject.transform.GetChild(0).gameObject.transform;
        groundLayer = LayerMask.GetMask("groundMask");
        wallRunLayer = LayerMask.GetMask("groundMask");
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement.isWallRunning = isWallRunning;
        CheckForWall();
        StateCheck();
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
        return !Physics.Raycast(transform.position, Vector3.down, wallCheckHeight, groundLayer);
    }

    private void StateCheck()
    {
       horizontalInput = Input.GetAxis("Horizontal");
       verticalInput = Input.GetAxis("Vertical");

        if ((isWallRight || isWallLeft) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if(!isWallRunning)
            {
                StartWallRun();
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
    }
    

}
