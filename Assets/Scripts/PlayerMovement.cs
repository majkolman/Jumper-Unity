using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    
    public float jumpForce;
    public float jumpCooldown;
    public float airAcceleration;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    bool isGrounded;


    public Transform orientation;
 
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    int playerState = 0;
    
    void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    
    void Update(){
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        MyInput();
        LimitSpeed();

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
    }

    void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(isGrounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }else if(!isGrounded){
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airAcceleration, ForceMode.Force);
        }
    }

    void LimitSpeed(){
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        switch(playerState){
            case 0:
                //normal movement
                if(horizontalVel.magnitude > moveSpeed){
                    Vector3 limitedVel = horizontalVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
                break;

            default:
                break;
        }
    }

    void Jump(){
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump(){
        readyToJump = true;
    }
}
