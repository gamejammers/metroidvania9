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
   public void StandingJump()
   {
       anim.SetTrigger( AnimtaionTags.JUMP_TRIGGER );
   }
   public void RunningJump()
   {
       anim.SetTrigger( AnimtaionTags.RUNNINGJUMP_TRIGGER );
   }
}
