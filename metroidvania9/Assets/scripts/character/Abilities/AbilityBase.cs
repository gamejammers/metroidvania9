using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityBase : MonoBehaviour
{
    public AbilityType abilityType;
    public AbilityManager abilityManager;
    public float cooldown = 5.0f;
    protected float lastUsedTime;
    public virtual void Use()
    {
        Debug.Log("implement ability");
    }
    public void EndAbility()
    {
        abilityManager.lockAbilities = false;
    }
}
