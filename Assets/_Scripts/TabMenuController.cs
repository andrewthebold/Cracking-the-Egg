using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabMenuController : MonoBehaviour {

    /// <summary>
    /// Starting GameObject button
    /// </summary>
    public Button startTab;
    public Text startText;
    public GameObject startEnvironment;

    /// <summary>
    /// Holds current tab (GameObject) being displayed
    /// </summary>
    private Button curTab;
    private GameObject curEnvironment;

    public GameObject tabExtenderBG;

    public Color enabledColor;
    public Color disabledColor;

    public Text DescriptionText;
    public Text SubtitleText;

    public GameObject gameManagerObj;
    private GameManager gameManager;

    private void Awake()
    {
        curTab = startTab;
    }

    // Use this for initialization
    void Start () {
        gameManager = gameManagerObj.GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
    
	}

    public void UpdateTab (Button newTab)
    {
        if (!newTab.interactable) return;

        if (curTab)
        {
            // Reenable previous tab
            curTab.interactable = true;
            curTab.GetComponent<Image>().color = disabledColor;
        }

        // Disable new tab
        newTab.interactable = false;
        newTab.GetComponent<Image>().color = enabledColor;

        // Move tab extender bg to new position
        Vector3 tabDelta = new Vector3(0f, newTab.transform.localPosition.y - tabExtenderBG.transform.localPosition.y, 0f);
        tabExtenderBG.transform.localPosition += tabDelta;

        // Update Description
        bool descriptionFound = false;

        foreach (Transform child in newTab.transform)
        {
            if (child.CompareTag("TabDescription"))
            {
                DescriptionText.text = child.GetComponent<Text>().text;
                descriptionFound = true;
                break; // Only use the first one found in the button
            }
        }

        if (!descriptionFound) DescriptionText.text = "Description not found.";

        // Update Egg Age
        bool subtitleFound = false;

        foreach (Transform child in newTab.transform)
        {
            if (child.CompareTag("TabSubtitle"))
            {
                SubtitleText.text = child.GetComponent<Text>().text;
                subtitleFound = true;
                break; // Only use the first one found in the button
            }
        }

        if (!subtitleFound) SubtitleText.text = "Subtitle not found.";

        // Update the objective panel

        // Change the active tab;
        curTab = newTab;
    }

    public void UpdateEnvironment (GameObject environment)
    {
        // Update the environment
        gameManagerObj.GetComponent<GameManager>().LoadEnvironment(environment);
    }
}
