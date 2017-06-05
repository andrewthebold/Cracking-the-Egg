using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private GameObject curObjective;

    public GameObject tabExtenderBG;

    public Color enabledColor;
    public Color disabledColor;

    public Text DescriptionText;
    public Text SubtitleText;

    public GameObject gameManagerObj;
    private GameManager gameManager;

    public Color completeMarkerColor;
    public Color incompleteMarkerColor;

    public GameObject objectiveCompleteSoundSource;

    public Material MaterialWireframe;

    public Text ObjectNameText;

    public Text ObjectiveFoundMarkerText;
    public Image ObjectiveFoundMarkerImage;

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

        // Change the active tab;
        curTab = newTab;
    }

    public void UpdateObjectiveObject (GameObject objective)
    {
        if (curObjective) curObjective.SetActive(false);

        curObjective = objective;

        objective.SetActive(true);

        bool completed = objective.tag == "ObjectiveComplete";

        // Hide the wireframe
        bool wireframeFound = false;

        // Find the active marker within the tab
        foreach (Transform child in objective.transform)
        {
            if (child.CompareTag("ObjectiveObjWireframe"))
            {
                child.gameObject.SetActive(!completed);

                wireframeFound = true;
                break; // Only use the first one found in the button
            }
        }

        if (!wireframeFound) Debug.LogError("Wireframe of objective not found.");

        // Show the complete
        bool completeFound = false;

        // Find the active marker within the tab
        foreach (Transform child in objective.transform)
        {
            if (child.CompareTag("ObjectiveObjNormal"))
            {
                child.gameObject.SetActive(completed);

                completeFound = true;
                break; // Only use the first one found in the button
            }
        }

        if (!completeFound) Debug.LogError("Complete of objective not found.");

        // Update the marker
        if (completed)
        {
            ObjectiveFoundMarkerText.text = "Found";
            ObjectiveFoundMarkerImage.color = completeMarkerColor;
        } else
        {
            ObjectiveFoundMarkerText.text = "Not Found";
            ObjectiveFoundMarkerImage.color = incompleteMarkerColor;
        }
    }

    public void UpdateEnvironment (GameObject environment)
    {
        // Update the environment
        gameManagerObj.GetComponent<GameManager>().LoadEnvironment(environment);
    }

    public void CompleteObjective (GameObject tab)
    {
        // Update Description
        bool activeMarkerFound = false;

        // Find the active marker within the tab
        foreach (Transform child in tab.transform)
        {
            if (child.CompareTag("TabActiveMarker"))
            {
                // Set the active marker to checked
                child.GetComponentsInChildren<Text>()[0].text = "o";
                child.GetComponentsInChildren<Image>()[0].color = completeMarkerColor;

                // Play completion sound
                objectiveCompleteSoundSource.GetComponent<AudioSource>().Play();
   
                // Update objective screen

                activeMarkerFound = true;
                break; // Only use the first one found in the button
            }
        }

        if (!activeMarkerFound) Debug.LogError("Active marker of tab not found.");


        // Do a check for if we finished all objectives (how?)
        // TODO
    }

    public void CompleteWireframeMaterial (GameObject obj)
    {
        if (obj.GetComponent<MeshRenderer>())
        {
            MeshRenderer cachedRenderer = obj.GetComponent<MeshRenderer>();

            Material[] intMaterials = new Material[cachedRenderer.materials.Length];
            for (int i = 0; i < intMaterials.Length; i++)
            {
                intMaterials[i] = MaterialWireframe;
            }
            cachedRenderer.materials = intMaterials;
        }

        if (obj.GetComponent<SkinnedMeshRenderer>())
        {
            SkinnedMeshRenderer cachedRenderer = obj.GetComponent<SkinnedMeshRenderer>();

            Material[] intMaterials = new Material[cachedRenderer.materials.Length];
            for (int i = 0; i < intMaterials.Length; i++)
            {
                intMaterials[i] = MaterialWireframe;
            }
            cachedRenderer.materials = intMaterials;
        }
    }
}
