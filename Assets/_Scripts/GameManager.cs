using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float fadeSpeed = 2;

    public GameObject WhiteFader;

    // Interface Panel 1
    public GameObject SuccessPanelObj;
    public GameObject Icosphere;

    // Interface Panel 2
    public GameObject LoadingBarObj;
    public GameObject ObjectivesObj;
    public GameObject LoadingAudioObj;

    // Egg
    public GameObject EggTopObj;
    public Material TranslucentWhite;
    public Material TranslucentMoreWhite;
    public GameObject OfflineBand;

    // Environments
    private GameObject curEnvironment;

    public GameObject SelectorMenu;

    private TabMenuController tabMenuController;

    public GameObject PasswordObj;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        tabMenuController = SelectorMenu.GetComponent<TabMenuController>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void WhiteFlash()
    {
        StartCoroutine(WhiteFlashCoroutine());
    }

    IEnumerator WhiteFlashCoroutine()
    {
        float fadeLevel = 0;

        yield return new WaitForSeconds(15);

        while (fadeLevel < 1)
        {
            yield return null;
            fadeLevel += fadeSpeed * Time.deltaTime;
            fadeLevel = Mathf.Clamp01(fadeLevel);
            WhiteFader.GetComponent<OVRScreenFade2>().SetFadeLevel(fadeLevel);
        }

        InitializeEgg();

        yield return new WaitForSeconds(4);

        // Fade out white at slower pace
        while (fadeLevel > 0)
        {
            yield return null;
            fadeLevel -= fadeSpeed / 20 * Time.deltaTime;
            fadeLevel = Mathf.Clamp01(fadeLevel);
            WhiteFader.GetComponent<OVRScreenFade2>().SetFadeLevel(fadeLevel);
        }
    }

    public void InitializeEgg()
    {
        // Hide loading screen
        LoadingBarObj.SetActive(false);

        // Play welcome music
        LoadingAudioObj.GetComponent<AudioSource>().Play();

        // Clear up Egg Material
        EggTopObj.GetComponent<MeshRenderer>().material = TranslucentMoreWhite;

        // Reveal first environment and update tab
        tabMenuController.UpdateTab(tabMenuController.startTab);
        tabMenuController.UpdateEnvironment(tabMenuController.startEnvironment);

        // Hide Icosphere
        Icosphere.SetActive(false);

        // Hide warning message (offline band)
        OfflineBand.SetActive(false);

        // Show success message;
        SuccessPanelObj.SetActive(true);
    }

    public void LoadEnvironment (GameObject environment)
    {
        // Hide any current environment
        if (curEnvironment) curEnvironment.SetActive(false);

        if (environment)
        {
            // Show the new environment
            environment.SetActive(true);

            curEnvironment = environment;
        }
    }

    public void newPassword ()
    {
        List<string> passwords = new List<string>();

        passwords.Add("Scrambled");
        passwords.Add("Over-easy");
        passwords.Add("Boiled");
        passwords.Add("Sunny-side Up");
        passwords.Add("Poached");
        passwords.Add("Egg-celent");
        passwords.Add("Whisked");

        int index = Random.Range(0, passwords.Count - 1);

        PasswordObj.GetComponentsInChildren<Text>()[0].text = "\"" + passwords[index] + "\"";
    }

    public void restartGame ()
    {
        SceneManager.LoadScene(0);
    }
}
