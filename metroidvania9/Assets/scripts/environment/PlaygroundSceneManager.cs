//
// (c) GameJammers 2020
// http://www.jamming.games
//

using Cinemachine;
using System.Collections;
using UnityEngine;

namespace Metroidvania
{
    public class PlaygroundSceneManager
        : MonoBehaviour
    {
        //
        // members ////////////////////////////////////////////////////////////
        //

        public CinemachineVirtualCamera cutsceneCam             = null;
        public Animator gateway                                 = null;

        //
        // unity callbacks ////////////////////////////////////////////////////
        //

        protected virtual void Awake()
        {
            StartCoroutine(StartCutscene());
        }

        //
        // private methods ////////////////////////////////////////////////////
        //
        
        private IEnumerator StartCutscene()
        {
            cutsceneCam.Priority = 100;
            yield return new WaitForSeconds(3f);
            cutsceneCam.Priority = 0;
            gateway.SetTrigger("open");
        }
        
    }
}
