using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public AbilityManager abilityManager;
    public Camera cam;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed;
    public float speed;
    public Vector3 moveVector;
    public float gravity = 14.0f;
    public float jumpForce = 10.0f;
    public bool isGrounded;
    public bool move;
    private float verticalVelocity;
    
    void Awake()
    {
        cam = Camera.main;
        abilityManager = FindObjectOfType<AbilityManager>();
    }


    void Update()
    {
        CallAbility();
        
        if(!move) return;

        PlayerMovementAndRotation();

        this.isGrounded = controller.isGrounded;

        if(this.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            if(Input.GetKeyDown(Keys.JUMP))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; 
        }
        moveVector = new Vector3(0, verticalVelocity, 0).normalized + desiredMoveDirection;
        controller.Move(moveVector.normalized * speed * Time.deltaTime);
    }
    void PlayerMovementAndRotation()
    {
        InputX = Input.GetAxis(Axis.HORIZONTAL_AXIS);
        InputZ = Input.GetAxis(Axis.VERTICAL_AXIS);

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize(); 

        desiredMoveDirection = forward * InputZ + right * InputX;

        if(desiredMoveDirection.sqrMagnitude != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed); 
    }

    void CallAbility()
    {
        if(Input.GetKeyDown(Keys.DASH))
        {
            abilityManager.CallAbility(AbilityType.DASH);
        }
    }
    void InputMagnitude()
    {
        PlayerMovementAndRotation();
        //
        //Set animations
        //
    }
}
