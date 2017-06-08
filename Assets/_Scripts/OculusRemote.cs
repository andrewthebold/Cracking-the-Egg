using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusRemote : MonoBehaviour {

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.DpadDown)) {
            UnityEngine.VR.InputTracking.Recenter();
        }
    }
}
