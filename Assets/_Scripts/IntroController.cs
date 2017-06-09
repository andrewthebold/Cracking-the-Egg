using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void quitApp ()
    {
        Application.Quit();
    }

    public void startApp ()
    {
        StartCoroutine(startAppAfterFade());
    }

    IEnumerator startAppAfterFade ()
    {
        yield return new WaitForSeconds(3);
   
        SceneManager.LoadScene("Primary");
    }
}
