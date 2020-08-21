using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void SetSpeed(float speed)
    {
        anim.SetFloat(AnimtaionTags.SPEED, speed, 0.0f, Time.deltaTime);
    }
    public void RunningJump()
    {
        anim.SetTrigger( AnimtaionTags.RUNNINGJUMP_TRIGGER );
    }
    public void StartFocusingJump()
    {
        anim.SetTrigger( AnimtaionTags.STARTFOCUSINGJUMP_TRIGGER);
    }
    public void EndFocusingJump()
    {
        anim.SetTrigger( AnimtaionTags.ENDFOCUSINGJUMP_TRIGGER);
    }
    public void JumpRising()
    {
        anim.SetTrigger( AnimtaionTags.JUMPRISING_TRIGGER );
    }
    public void JumpFalling()
    {
        anim.SetTrigger( AnimtaionTags.JUMPFALLING_TRIGGER );
    }
    public void Landing()
    {
        anim.SetTrigger( AnimtaionTags.LANDING_TRIGGER );
    }
}
