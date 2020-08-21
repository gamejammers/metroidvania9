using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterAbilityController : MonoBehaviour
{
    public AbilityManager abilityManager;
    public CharacterMovement characterMovement;
    void Awake()
    {
        abilityManager = FindObjectOfType<AbilityManager>();   
    }
    void Update()
    {
        CallAbility();
    }
    void CallAbility()
    {
        if(Input.GetKeyDown(Keys.DASH))
        {
            if(characterMovement.moveMagnitude > characterMovement.turnSpeedTreshold)
                abilityManager.CallAbility(AbilityType.DASH);
        }
    }
}
