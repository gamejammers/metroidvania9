//
// (c) GameJammers 2020
// http://www.jamming.games
//

using Cinemachine;
using UnityEngine;

namespace Metroidvania
{
    public class BasicCharacter
        : MonoBehaviour
    {
        //
        // members ////////////////////////////////////////////////////////////
        //

        public float moveSpeed                                  = 10f;
        public AnimationCurve jumpCurve                         = new AnimationCurve();
        public float jumpStrength                               = 10f;
        public float fallSpeed                                  = 15f;
        public Camera sceneCamera                               = null;
        public CinemachineVirtualCameraBase playerCam           = null;
        public CharacterController controller                   = null;

        public bool isJumping                                   { get; private set; }
        public float jumpStartTime                              { get; private set; }
        public bool isGrounded                                  { get; private set; }

        //
        // unity callbacks ////////////////////////////////////////////////////
        //

        protected virtual void Awake()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //
        // --------------------------------------------------------------------
        //

        protected virtual void Update()
        {
            Vector3 deltaMove = sceneCamera.transform.TransformDirection(
                new Vector3(
                    Input.GetAxis("Horizontal"),
                    0f,
                    Input.GetAxis("Vertical")
                )
            ) * moveSpeed;

            deltaMove.y = -fallSpeed;

            if(isJumping)
            {
                float elapsed = Time.time - jumpStartTime;
                if(isGrounded && elapsed > 0.1f)
                {
                    isJumping = false;
                }
                else if(elapsed >= jumpCurve.keys[jumpCurve.length-1].time)
                {
                    isJumping = false;
                }
                else
                {
                    deltaMove.y = jumpCurve.Evaluate(elapsed) * jumpStrength;
                }
            }
            else if(isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                isJumping = true;
                jumpStartTime = Time.time;
            }

            controller.Move(deltaMove * Time.deltaTime);
            isGrounded = controller.isGrounded;
        }
    }
}
