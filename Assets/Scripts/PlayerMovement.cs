using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Playermovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    private float Speed;
    private float groundDrag;
    public float groundDragStart;
    public float groundDragSlide;

    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float airAcceleration;
    bool readyToJump = true;

    [Header("Crouch")]
    public float crouchYScale;
    private float startYScale;
    public float crouchChange;
    private float currentYScale;
    private float goalYScale;
    private bool callCrouchDown;
    public float crouchSpeedChange;

    [Header("Slide")]
    public float slideFriction;
    public float slideThreshold;
    public float slideEndSpeed;
    private bool isSliding;
    private bool SlideCheck;
    public float slideTimer;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Wall Running")]
    private float wallRunStartSpeed;
    public bool isWallRunning;
    private bool isWallRunningPrevious;
    public float maxWallRunSpeed;
    public float minWallRunSpeed;



    public Transform orientation;
 
    private float horizontalInput;
    private float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
        isSliding = false;
        isWallRunning = false;
        SlideCheck = false;
        groundDrag = groundDragStart;
        rb.AddForce(Vector3.right * 0.001f, ForceMode.Impulse);
    }
    
    void Update(){
        //check for wall and limit speed
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        

        StateHandler();
        MyInput();
        if(!SlideCheck) LimitSpeed();

        if(isSliding){
            groundDrag = groundDragSlide;
        }else{
            groundDrag = groundDragStart;
        }
        
        if(isGrounded){
            rb.drag = groundDrag;
        }else{
            rb.drag = 0;
        }

        isWallRunningPrevious = isWallRunning;
    }

    void FixedUpdate(){
        MovePlayer();
    }

    void MyInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && isGrounded){
            Invoke(nameof(DisableSlide), 0.1f);
            Invoke(nameof(StopSlideAccelerate), 0.1f);
            goalYScale = startYScale;
            currentYScale = crouchYScale;
            CrouchHandlerUp();
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if (isGrounded && (Input.GetKeyDown(crouchKey)|| callCrouchDown)){
            callCrouchDown = false;
            goalYScale = crouchYScale;
            currentYScale = startYScale;
            CrouchHandlerDown();
        }else if (Input.GetKeyDown(crouchKey) && !isGrounded){
            callCrouchDown = true;
        }

        //stop crouch
        if (Input.GetKeyUp(crouchKey) && isGrounded){
            isSliding = false;
            SlideCheck = false;
            goalYScale = startYScale;
            currentYScale = crouchYScale;
            CrouchHandlerUp();
        }
        
    }

    void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


        //slope handling
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * Speed * 10f, ForceMode.Force);
        }

        //ground and air handling
        if(isGrounded){
            rb.AddForce(moveDirection.normalized * Speed * 10f * airAcceleration, ForceMode.Force);
        }else if(!isGrounded){
            rb.AddForce(moveDirection.normalized * Speed * 10f * airAcceleration, ForceMode.Force);
        }
    }

    void LimitSpeed(){
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(isWallRunning){
            if(!isWallRunningPrevious && isWallRunning)wallRunStartSpeed = Math.Min(Math.Max(horizontalVel.magnitude, minWallRunSpeed), maxWallRunSpeed);
            if(horizontalVel.magnitude > wallRunStartSpeed)
            {
                Vector3 limitedVel = horizontalVel.normalized * wallRunStartSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        else if(isSliding || !isGrounded)
        {
                float reductionFactor = (horizontalVel.magnitude - slideFriction) / horizontalVel.magnitude;
                Vector3 limitedVel = horizontalVel * reductionFactor;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                if (limitedVel.magnitude < slideEndSpeed) isSliding = false;
        }
        else if(!isSliding && horizontalVel.magnitude > Speed)
        {
            Vector3 limitedVel = horizontalVel.normalized * Speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        
    }

    void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump(){
        readyToJump = true;
    }

    void StateHandler(){
        if (!isGrounded)
        {
            //in air
            Speed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            //player is crouching
            Speed = crouchSpeed;
            //Check for slide
            if (isGrounded)
            {
                Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                if (horizontalVel.magnitude > slideThreshold)
                {
                    if (!isSliding)
                    {
                        SlideCheck = true;
                        Invoke(nameof(StopSlideAccelerate), slideTimer);
                    }
                    isSliding = true;
                }
            }
        }
        else if (Input.GetKey(sprintKey))
        {
            //player is sprinting
            Speed = sprintSpeed;
        }
        else
        {
            //player is walking
            Speed = moveSpeed;
        }
    }
    
    void CrouchHandlerDown(){
        if (currentYScale < goalYScale)
            currentYScale = goalYScale;
        else
            Invoke(nameof(CrouchHandlerDown), crouchSpeedChange);

        transform.localScale = new Vector3(transform.localScale.x, currentYScale, transform.localScale.z);
        //transform.Translate(Vector3.down * ((currentYScale - goalYScale) / 2));
        currentYScale -= crouchChange;
    }

    void CrouchHandlerUp()
    {
        if (currentYScale > goalYScale)
            currentYScale = goalYScale;
        else
            Invoke(nameof(CrouchHandlerUp), crouchSpeedChange);

        transform.localScale = new Vector3(transform.localScale.x, currentYScale, transform.localScale.z);
        currentYScale += crouchChange;
    }

    void StopSlideAccelerate()
    {
        SlideCheck = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(){
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void DisableSlide()
    {
        isSliding = false;
    }
}
