using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest : MonoBehaviour {
    private bool on = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ColorToggle()
    {
        if (!on)
        {
            GetComponent<Renderer>().material.color = Color.blue;
            on = true;
        } else
        {
            GetComponent<Renderer>().material.color = Color.white;
            on = false;
        }
        
    }

}
