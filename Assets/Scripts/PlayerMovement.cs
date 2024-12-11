using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    private float Speed;
    public float groundDrag;

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
        SlideCheck = false;
    }
    
    void Update(){
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        StateHandler();
        MyInput();
        if(!SlideCheck) LimitSpeed();

        if(isGrounded){
            rb.drag = groundDrag;
        }else{
            rb.drag = 0;
        }
    }

    void FixedUpdate(){
        MovePlayer();
    }

    void MyInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && isGrounded){
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if (isGrounded && (Input.GetKeyDown(crouchKey)|| callCrouchDown)){
            callCrouchDown = false;
            Debug.Log("Crouch");
            goalYScale = crouchYScale;
            currentYScale = startYScale;
            CrouchHandlerDown();
        }else if (Input.GetKeyDown(crouchKey) && !isGrounded){
            callCrouchDown = true;
        }

        //stop crouch
        if (Input.GetKeyUp(crouchKey) && isGrounded){
            Debug.Log("Stand");
            isSliding = false;
            SlideCheck = false;
            goalYScale = startYScale;
            currentYScale = crouchYScale;
            CrouchHandlerUp();
        }
        
    }

    void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(isGrounded){
            rb.AddForce(moveDirection.normalized * Speed * 10f, ForceMode.Force);
        }else if(!isGrounded){
            rb.AddForce(moveDirection.normalized * Speed * 10f * airAcceleration, ForceMode.Force);
        }
    }

    void LimitSpeed(){
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (!isSliding && horizontalVel.magnitude > Speed)
        {
            Vector3 limitedVel = horizontalVel.normalized * Speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        else if(isSliding)
        {
                float reductionFactor = (horizontalVel.magnitude - slideFriction) / horizontalVel.magnitude;
                Vector3 limitedVel = horizontalVel * reductionFactor;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                if (limitedVel.magnitude < slideEndSpeed) isSliding = false;
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
}
