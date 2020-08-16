using System.Collections;
using System.Collections.Generic;
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
            abilityManager.CallAbility(AbilityType.DASH);
        }
    }
}
