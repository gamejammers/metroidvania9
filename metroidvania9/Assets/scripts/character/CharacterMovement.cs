using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public CharacterAnimation characterAnimation;
    public Camera cam;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed;
    public float turnSpeedTreshold;
    public float maxSpeed;
    public float minSpeed;
    public float currentSpeed;
    public float speedIncrement;
    public float speedDecrement;
    public Vector3 moveVector;
    public float gravity = 14.0f;
    public float startingJumpVelocity = 10.0f;
    public float jumpForce = 28f;
    public bool isGrounded;
    public bool move;
    private float verticalVelocity;
    private float moveMagnitude;
    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {        
        if(!move) return;

        PlayerMovementAndRotation();

        this.isGrounded = controller.isGrounded;

        if(this.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            if(Input.GetKeyDown(Keys.JUMP))
            {
                if(currentSpeed > minSpeed)
                {
                    characterAnimation.RunningJump();
                }
                else
                {
                    characterAnimation.StandingJump();
                }
                verticalVelocity = startingJumpVelocity;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; 
        }
        moveVector = new Vector3(0, verticalVelocity, 0).normalized * jumpForce + desiredMoveDirection.normalized * currentSpeed;
        controller.Move(moveVector * Time.deltaTime);
    }
    void PlayerMovementAndRotation()
    {
        InputX = Input.GetAxis(Axis.HORIZONTAL_AXIS);
        InputZ = Input.GetAxis(Axis.VERTICAL_AXIS);
        
        characterAnimation.SetSpeed(currentSpeed); // move animation

        moveMagnitude = new Vector2(InputX, InputZ).sqrMagnitude;

        if(moveMagnitude > turnSpeedTreshold)
        {
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize(); 

        

            desiredMoveDirection = forward * InputZ + right * InputX;

            currentSpeed = Mathf.Clamp( currentSpeed, minSpeed, maxSpeed );
            currentSpeed += speedIncrement * Time.deltaTime;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed); 
        }
        else if(moveMagnitude < turnSpeedTreshold)
        {
            currentSpeed -= speedDecrement * Time.deltaTime;
            currentSpeed = Mathf.Clamp( currentSpeed, 0, maxSpeed );
        }
    }

    
}
