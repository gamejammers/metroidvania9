using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CmFreeLookCameraControlInversion : MonoBehaviour
{
    [Range(0,1)]
    public int mouseButtonIndex;
    void Start(){
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }
    public bool isInverted = false;
    public float GetAxisCustom(string axisName){
        if( !isInverted )
        {
            if(axisName == "Mouse X")
            {
                
                return -UnityEngine.Input.GetAxis("Mouse X");
                
            }
            else if (axisName == "Mouse Y")
            {
                
                return -UnityEngine.Input.GetAxis("Mouse Y");
               
            }
            return UnityEngine.Input.GetAxis(axisName);
        }
        else
        {
            if(axisName == "Mouse X")
            {
                
                return UnityEngine.Input.GetAxis("Mouse X");
                
            }
            else if (axisName == "Mouse Y")
            {
                
                return UnityEngine.Input.GetAxis("Mouse Y");
                
            }
            return UnityEngine.Input.GetAxis(axisName);
        }
       
    }
}
