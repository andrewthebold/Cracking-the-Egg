using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PointersControl : MonoBehaviour
{
    private OVRInputModule _inputModule;
    private OVRInputModule inputModule
    {
        get
        {
            if (_inputModule == null)
            {
                _inputModule = EventSystem.current.currentInputModule as OVRInputModule;
            }
            return _inputModule;
        }
    }

    void Awake()
    {
    }

    public void SetUseSphereTest(bool on)
    {
        inputModule.performSphereCastForGazepointer = on;
    }

    public void SetMatchNormal(bool on)
    {
        FindObjectOfType<OVRInputModule>().matchNormalOnPhysicsColliders = on;
    }
    public void SetHideGazepointerByDefault(bool hide)
    {
        OVRGazePointer.instance.hideByDefault = hide;
    }
    public void SetOnlyDimCursorWhenMouseActive(bool dim)
    {
        OVRGazePointer.instance.dimOnHideRequest = dim;
    }

    // Use this for initialization
    void Start()
    {
        FindObjectOfType<OVRPlayerController>().SetSkipMouseRotation(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
