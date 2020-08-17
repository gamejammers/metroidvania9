using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityManager : MonoBehaviour
{
    public List<AbilityBase> allAbilities;
    public List<AbilityType> ownedAbilities;
    public bool lockAbilities;
    public CharacterMovement characterMovement;
    
    void Awake()
    {
        characterMovement = FindObjectOfType<CharacterMovement>();
    }

    public void CallAbility(AbilityType t)
    {
        if( !lockAbilities )
        {
            if( ownedAbilities.Any(s => s == t) )
            {
                allAbilities.First(s => s.abilityType == t).Use();
            }
        }
        else
        {
            Debug.Log("ability locked down");
        }
    } 
}
public enum AbilityType
{
    DASH,HIGHERJUMP
}
