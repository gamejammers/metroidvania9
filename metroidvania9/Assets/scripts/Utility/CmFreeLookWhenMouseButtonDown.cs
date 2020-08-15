using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CmFreeLookWhenMouseButtonDown : MonoBehaviour
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
                if (Input.GetMouseButton(1))
                {
                    return -UnityEngine.Input.GetAxis("Mouse X");
                } 
                else
                {
                    return 0;
                }
            }
            else if (axisName == "Mouse Y")
            {
                if (Input.GetMouseButton(1))
                {
                    return -UnityEngine.Input.GetAxis("Mouse Y");
                }
                else
                {
                    return 0;
                }
            }
            return UnityEngine.Input.GetAxis(axisName);
        }
        else
        {
            if(axisName == "Mouse X")
            {
                if (Input.GetMouseButton(1))
                {
                    return UnityEngine.Input.GetAxis("Mouse X");
                } 
                else
                {
                    return 0;
                }
            }
            else if (axisName == "Mouse Y")
            {
                if (Input.GetMouseButton(1))
                {
                    return UnityEngine.Input.GetAxis("Mouse Y");
                } 
                else
                {
                    return 0;
                }
            }
            return UnityEngine.Input.GetAxis(axisName);
        }
       
    }
}
