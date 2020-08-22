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
    public float waitForLandingInterval = 0.7f;
    public float verticalVelocityFallAnimThreshold;
    private bool focusingForJump;
    private bool jumpRising;
    private bool fallingAnimationStarted;
/*     private bool isRunningJump;
 */    private float standingJumpCurrentDuration;
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

        moveMagnitude = new Vector2(InputX, InputZ).sqrMagnitude;

        if(moveMagnitude > turnSpeedTreshold)
        {
            characterAnimation.SetSpeed(currentSpeed); // canMove animation

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
            characterAnimation.SetSpeed(currentSpeed); // canMove animation
            
            currentSpeed -= speedDecrement * Time.deltaTime;
            currentSpeed = Mathf.Clamp( currentSpeed, 0, maxSpeed );
        }
    }

    void Jump()
    {        
        if(this.isGrounded)
        {
            if(fallingAnimationStarted)
            {
                characterAnimation.Landing();
                movementLocked = true;
                StartCoroutine( WaitForLanding() );
                fallingAnimationStarted = false;
            }
            else if(jumpRising == true)
            {
                jumpRising = false;
                characterAnimation.EndFocusingJump();
            }

            verticalVelocity = -gravity * Time.deltaTime;
            
            if(  Input.GetKeyDown(Keys.JUMP) )
            {
                if(currentSpeed > minSpeed)
                {
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
                    EndFocusingForJump();
                    characterAnimation.EndFocusingJump(); 
                }
            }  
            else if(focusingForJump && Input.GetKeyUp(Keys.JUMP))
            {
                if(standingJumpCurrentDuration >= jumpMinDuration && standingJumpCurrentDuration <= jumpMaxDuration)
                {
                    verticalVelocity = standingJumpCurrentDuration.Map(jumpMinDuration, jumpMaxDuration, 
                        minJumpVelocity, maxJumpVelocity );
                    EndFocusingForJump();
                    characterAnimation.JumpRising();
                }
                else if(standingJumpCurrentDuration < jumpMinDuration)
                {
                    EndFocusingForJump();
                    characterAnimation.EndFocusingJump(); 
                }
            }            
        }
        else
        {
            if(verticalVelocity > 0)
            {
                verticalVelocity -= gravity * Time.deltaTime;
                jumpRising = true;
            }
            else if(verticalVelocity <= 0 )
            {
                verticalVelocity -= gravity * fallMultiplier * Time.deltaTime;
                if(!fallingAnimationStarted && verticalVelocity < verticalVelocityFallAnimThreshold)
                {
                    jumpRising = false;
                    characterAnimation.JumpFalling();
                    fallingAnimationStarted = true;
                }
            }
        }
    }
    void EndFocusingForJump()
    {
        focusingForJump = false;
        canMove = true;
        standingJumpCurrentDuration = 0;        
    }
    IEnumerator WaitForLanding()
    {
        yield return new WaitForSeconds( waitForLandingInterval );
        movementLocked = false;
    }
}
