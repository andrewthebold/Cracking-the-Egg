
// # This script _must_ be attached to the main camera which renders 3D scene
// # Default coordinate system is left handed. If your project is using different coordinate system please configuration data in Start method accordingly
// # Use property Ansel.IsAvailable to adjust UI and other items which allow user to interact with Ansel
// # Use property Ansel.IsSessionActive to adjust game logic (game should be paused and camera parameters (position, orientation, FOV, view/projection matrices etc) _must_ not be changed elsewhere in script)
// # Use property Ansel.IsCaptureActive to disable effects (e.g. motion blur) which can cause Ansel not to work correctly during capture
// # Use Ansel.ConfigureSession to enable/disable Ansel sessions and features (only when session is not active)
// # Key parameters are exposed as properties so they can be edited directly in the editor. Other parameters should be changed only in rare scenarios.

using UnityEngine;
using System.Runtime.InteropServices;

namespace NVIDIA
{
  public class Ansel : MonoBehaviour
  {
    [StructLayout(LayoutKind.Sequential)]
    public struct ConfigData
    {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] forward;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] up;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] right;

      // The speed at which camera moves in the world
      public float translationalSpeedInWorldUnitsPerSecond;
      // The speed at which camera rotates 
      public float rotationalSpeedInDegreesPerSecond;
      // How many frames it takes for camera update to be reflected in a rendered frame
      public uint captureLatency;
      // How many frames we must wait for a new frame to settle - i.e. temporal AA and similar
      // effects to stabilize after the camera has been adjusted
      public uint captureSettleLatency;
      // Game scale, the size of a world unit measured in meters
      public float metersInWorldUnit;
      // Integration will support Camera::screenOriginXOffset/screenOriginYOffset
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraOffcenteredProjectionSupported;
      // Integration will support Camera::position
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraTranslationSupported;
      // Integration will support Camera::rotation
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraRotationSupported;
      // Integration will support Camera::horizontalFov
      [MarshalAs(UnmanagedType.I1)]
      public bool isCameraFovSupported;
      // Integration allows a filter/effect to remain active when the Ansel session is not active
      [MarshalAs(UnmanagedType.I1)]
      public bool isFilterOutsideSessionAllowed;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraData
    {
      public float fov; // degrees
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
      public float[] projectionOffset;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
      public float[] position;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
      public float[] rotation;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct SessionData
    {
      [MarshalAs(UnmanagedType.I1)]
      public bool isAnselAllowed; // if set to false none of the below parameters is relevant
      [MarshalAs(UnmanagedType.I1)]
      public bool is360MonoAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool is360StereoAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isFovChangeAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isHighresAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isPauseAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isRotationAllowed;
      [MarshalAs(UnmanagedType.I1)]
      public bool isTranslationAllowed;
    };

    // The speed at which camera moves in the world
    [SerializeField] public float TranslationalSpeedInWorldUnitsPerSecond = 5.0f;
    // The speed at which camera rotates 
    [SerializeField] public float RotationalSpeedInDegreesPerSecond = 45.0f;
    // How many frames it takes for camera update to be reflected in a rendered frame
    [SerializeField] public uint CaptureLatency = 0;
    // How many frames we must wait for a new frame to settle - i.e. temporal AA and similar
    // effects to stabilize after the camera has been adjusted
    [SerializeField] public uint CaptureSettleLatency = 10;
    // Game scale, the size of a world unit measured in meters
    [SerializeField] public float MetersInWorldUnit = 1.0f;
    // Integration allows a filter/effect to remain active when the Ansel session is not active
    [SerializeField] public bool IsFilterOutsideSessionAllowed = false;

    public static bool IsSessionActive
    {
      get
      {
        return sessionActive;
      }
    }

    public static bool IsCaptureActive
    {
      get
      {
        return captureActive;
      }
    }

    public static bool IsAvailable
    {
      get
      {
        return anselIsAvailable();
      }
    }    

    // --------------------------------------------------------------------------------
    public void Start()
    {
      if(!IsAvailable)
      {
        Debug.LogError("Ansel is not available or enabled on this platform. Did you forget to whitelist your executable?");
        return;
      }

      // Get our camera (this script _must_ be attached to the main camera which renders the 3D scene)
      mainCam = GetComponent<UnityEngine.Camera>();      

      // Default coordinate system is left handed.
      // If your project is using different coordinate system please adjust accordingly
      ConfigData config = new ConfigData();
      config.right = new float[3] { 1, 0, 0 };
      config.up = new float[3] { 0, 1, 0 };
      config.forward = new float[3] { 0, 0, 1 };
      // Can be set by user from the editor
      config.translationalSpeedInWorldUnitsPerSecond = TranslationalSpeedInWorldUnitsPerSecond;
      config.rotationalSpeedInDegreesPerSecond = RotationalSpeedInDegreesPerSecond;
      config.captureLatency = CaptureLatency;
      config.captureSettleLatency = CaptureSettleLatency;
      config.metersInWorldUnit = MetersInWorldUnit;
      // These should always be true unless there is some special scenario
      config.isCameraOffcenteredProjectionSupported = true;
      config.isCameraRotationSupported = true;
      config.isCameraTranslationSupported = true;
      config.isCameraFovSupported = true;
      // This value can be used to allow users to run some cool filter through entire game
      config.isFilterOutsideSessionAllowed = IsFilterOutsideSessionAllowed;
      anselInit(ref config);
      
      // Ansel will return camera parameters here
      anselCam = new CameraData();

      // Default session configuration which allows everything.
      // Game can reconfigure session anytime session is not active by calling ConfigureSession.
      SessionData ses = new SessionData();
      ses.isAnselAllowed = true; // if false none of the below parameters is relevant
      ses.isFovChangeAllowed = true;
      ses.isHighresAllowed = true;
      ses.isPauseAllowed = true;
      ses.isRotationAllowed = true;
      ses.isTranslationAllowed = true;
      ses.is360StereoAllowed = true;
      ses.is360MonoAllowed = true;
      anselConfigureSession(ref ses);

      print("Ansel is initialized and ready to use");
    }    

    // --------------------------------------------------------------------------------
    public void ConfigureSession(SessionData ses)
    {
      if (!IsAvailable)
      {
        Debug.LogError("Ansel is not available or enabled on this platform. Did you forget to whitelist your executable?");
        return;
      }

      if (anselIsSessionOn())
      {
        Debug.LogError("Ansel session cannot be configured while session is active");
        return;
      }
      anselConfigureSession(ref ses);      
    }    

    // --------------------------------------------------------------------------------
    public void OnPreRender()
    {
      if (!IsAvailable)
      {
        Debug.LogError("Ansel is not available or enabled on this platform. Did you forget to whitelist your executable?");
        return;
      }

      if (anselIsSessionOn())
      {
        // Ansel session is active (user pressed Alt+F2)
        if (!sessionActive)
        {
          sessionActive = true;
          // On first update after session is activated we need to store
          // camera and other parameters so they can be restored later on
          SaveState();
          print("Started Ansel session");
        }
        
        // Check if capture is active
        captureActive = anselIsCaptureOn();          
        
        Transform trans = mainCam.transform;

        anselCam.fov = mainCam.fieldOfView;
        anselCam.projectionOffset = new float[2] { 0, 0 };
        anselCam.position = new float[3] { trans.position.x, trans.position.y, trans.position.z };
        anselCam.rotation = new float[4] { trans.rotation.x, trans.rotation.y, trans.rotation.z, trans.rotation.w };
        anselUpdateCamera(ref anselCam);

        // Reset projection matrix so that potential FOV changes below can take effect
        mainCam.ResetProjectionMatrix();                
        mainCam.transform.position = new Vector3(anselCam.position[0], anselCam.position[1], anselCam.position[2]);
        mainCam.transform.rotation = new Quaternion(anselCam.rotation[0], anselCam.rotation[1], anselCam.rotation[2], anselCam.rotation[3]);
        mainCam.fieldOfView = anselCam.fov;
        if (anselCam.projectionOffset[0] != 0 || anselCam.projectionOffset[1] != 0)
        {
          // Hi-res screen shots require projection matrix adjustment
          projectionMatrix = mainCam.projectionMatrix;
          float l = -1.0f + anselCam.projectionOffset[0];
          float r = l + 2.0f;
          float b = -1.0f + anselCam.projectionOffset[1];
          float t = b + 2.0f;
          projectionMatrix[0, 2] = (l + r) / (r - l);
          projectionMatrix[1, 2] = (t + b) / (t - b);
          mainCam.projectionMatrix = projectionMatrix;
        }               
      }
      else
      {
        // Ansel session is no longer active
        if (sessionActive)
        {
          sessionActive = false;
          captureActive = false;
          RestoreState();
          print("Stopped Ansel session");
        }
      }
    }

    // --------------------------------------------------------------------------------
    private void SaveState()
    {
      Transform trans = mainCam.transform;

      cameraPos = trans.position;
      cameraRotation = trans.rotation;
      cameraFOV = mainCam.fieldOfView;
      cursorVisible = Cursor.visible;

      // Stop time counting to effectively pause the game
      Time.timeScale = 0.0f;
      // Disable user input
      Input.ResetInputAxes();

      // TODO: Hide GUI/HUD and disable anything else that might cause bad user experience with Ansel
      //GameObject.Find("NGUICamera").GetComponent<Camera>().enabled = false;

      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
    }

    // --------------------------------------------------------------------------------
    private void RestoreState()
    {
      // Reset time scale back to 1.0f (or whatever value game is using as default)
      Time.timeScale = 1.0f;

      // Restore camera parameters to the original values        
      mainCam.ResetProjectionMatrix();
      mainCam.transform.position = cameraPos;
      mainCam.transform.rotation = cameraRotation;
      mainCam.fieldOfView = cameraFOV;
      Cursor.visible = cursorVisible;
      Cursor.lockState = CursorLockMode.None;

      // TODO: show any hidden GUI/HUD elements and re-enable all items which were disabled when session started
      //GameObject.Find("NGUICamera").GetComponent<Camera>().enabled = true;
    }

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void anselInit(ref ConfigData conf);

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void anselUpdateCamera(ref CameraData cam);

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void anselConfigureSession(ref SessionData ses);

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern bool anselIsSessionOn();

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern bool anselIsCaptureOn();

#if UNITY_64 || UNITY_EDITOR_64
    [DllImport("AnselPlugin64", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#else
  [DllImport("AnselPlugin32", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern bool anselIsAvailable();

    private static bool sessionActive = false;
    private static bool captureActive = false;
    
    private bool cursorVisible = false;
    private Vector3 cameraPos;
    private Quaternion cameraRotation;
    private float cameraFOV;

    private CameraData anselCam;
    private Matrix4x4 projectionMatrix;
    private Camera mainCam;
  };
}