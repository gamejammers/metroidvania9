using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : AbilityBase
{
    public Vector3 moveDirection;
    public const float maxDashTime = 1.0f;
    public float dashDistance = 10;
    public float dashStoppingSpeed = 0.1f;
    float currentDashTime;
    public float dashSpeed = 6;
 
    public override void Use()
    {
        Debug.Log("Dash start");
        currentDashTime = 0;
        abilityManager.characterMovement.move = false;
        StartCoroutine( DashRoutine() );
    }

    IEnumerator DashRoutine () 
    {
        while (currentDashTime < maxDashTime)
        {
            moveDirection = abilityManager.characterMovement.transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;

            abilityManager.characterMovement.controller.Move(moveDirection * Time.deltaTime * dashSpeed);
            
            yield return null;
        }
        abilityManager.characterMovement.move = true;
        EndAbility();
    }
    
}
