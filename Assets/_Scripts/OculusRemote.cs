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

    /*// White dot cursor
    public GameObject cursor;

    // Main Camera
    public GameObject mainCamera;

	// Use this for initialization
	void Start () {
        cursor.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo;

        OverlaySwitch();

        //OVROverlay currentOverlay = GetComponent<OVROverlay>();

        Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitInfo, 500);

        //if (hitInfo.collider != null)
        //{
            //cursor.transform.position = hitInfo.point;
        //} else {
            cursor.transform.position = mainCamera.transform.forward * 26 + mainCamera.transform.position;
        //}

        cursor.transform.LookAt(mainCamera.transform.position); cursor.transform.LookAt(mainCamera.transform.position);
    }

    private void OverlaySwitch ()
    {
        // Hide the cursor if the overlay is called
        bool isMenuActive = OVRInspector.instance.IsMenuActive();
        //cursor.transform.localScale = isMenuActive ? new Vector3(0, 0, 0) : new Vector3(0.3f, 0.3f, 0.3f);
    }*/
}
