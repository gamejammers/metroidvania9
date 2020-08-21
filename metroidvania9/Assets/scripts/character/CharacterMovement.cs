using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    /////// REFERANCES ///////
    public CharacterController controller;
    public CharacterAnimation characterAnimation;
    public CharacterAbilityController abilityController;
    public Camera cam;
    /////// ---------- ///////

    /////// MOVEMENT ///////
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
    public bool canMove;
    public float moveMagnitude;
    /////// -------- ///////

    /////// JUMP //////
    public float gravity = 14.0f;
    public float fallMultiplier;
    public float runningJumpVelocity = 10.0f;
    public bool isGrounded;
    public float jumpMaxDuration;
    public float jumpMinDuration;
    public float minJumpVelocity;
    public float maxJumpVelocity;
    private bool focusingForJump;
    private bool fallingAnimationStarted;
    private bool isRunningJump;
    private float standingJumpCurrentDuration;
    private float verticalVelocity;
    ///////---- ///////
    public bool movementLocked;
    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {        
        if(movementLocked) return;

        this.isGrounded = controller.isGrounded;

        MovementAndRotation();

        Jump();

        moveVector = new Vector3(0, verticalVelocity, 0) + desiredMoveDirection.normalized * currentSpeed;
        controller.Move(moveVector * Time.deltaTime);
    }
    void MovementAndRotation()
    {
        if(!canMove) return;

        InputX = Input.GetAxis(Axis.HORIZONTAL_AXIS);
        InputZ = Input.GetAxis(Axis.VERTICAL_AXIS);
        
        characterAnimation.SetSpeed(currentSpeed); // canMove animation

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

    void Jump()
    {        
        if(this.isGrounded)
        {
            isRunningJump = false;
            if(fallingAnimationStarted)
            {
                characterAnimation.Landing();
                fallingAnimationStarted = false;
            }
            verticalVelocity = -gravity * Time.deltaTime;
            if(  Input.GetKeyDown(Keys.JUMP) )
            {
                if(currentSpeed > minSpeed)
                {
                    isRunningJump = true;
                    characterAnimation.RunningJump();
                    verticalVelocity = runningJumpVelocity;
                }
                else if( currentSpeed < minSpeed ) 
                {
                    focusingForJump = true;
                    canMove = false;
                    characterAnimation.StartFocusingJump();
                }
            }
            else if( focusingForJump && Input.GetKey(Keys.JUMP))
            {
                standingJumpCurrentDuration += Time.deltaTime;

                if(standingJumpCurrentDuration > jumpMaxDuration)
                {
                    EndJump();
                    characterAnimation.EndFocusingJump(); 
                }
            }  
            else if(focusingForJump && Input.GetKeyUp(Keys.JUMP))
            {
                if(standingJumpCurrentDuration >= jumpMinDuration && standingJumpCurrentDuration <= jumpMaxDuration)
                {
                    isRunningJump = false;
                    verticalVelocity = standingJumpCurrentDuration.Map(jumpMinDuration, jumpMaxDuration, 
                        minJumpVelocity, maxJumpVelocity );
                    EndJump();
                    characterAnimation.JumpRising();
                }
                else if(standingJumpCurrentDuration < jumpMinDuration)
                {
                    EndJump();
                    characterAnimation.EndFocusingJump(); 
                }
            }            
        }
        else
        {
            if(verticalVelocity > 0)
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }
            else if(verticalVelocity <= 0 )
            {
                verticalVelocity -= gravity * fallMultiplier * Time.deltaTime;
                if(!fallingAnimationStarted && isRunningJump == false)
                {
                    characterAnimation.JumpFalling();
                    fallingAnimationStarted = true;
                }
            }
        }
    }
    void EndJump()
    {
        focusingForJump = false;
        canMove = true;
        standingJumpCurrentDuration = 0;        
    }
}
