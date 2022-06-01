using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class AnimationRecorder : MonoBehaviour
// Uses the Unity GameObjectRecorder to capture different properties of a given gameObject
{
    public GameObject viewport;
    public XRNode inputDevice;
    public InputHelpers.Button toggleRecordingInput;
    public string customPrefix = "";
    public string customName = "";
    public enum NamingConvention {Random, Prefix, Custom};
    public NamingConvention namingConvention;
    public float framerate = 24;

    private bool scriptIsEnabled = false;
    private bool wasActivated = false;
    private GameObjectRecorder recorder;
    private string sessionDirectory;
    private string sessionId;
    private string clipName;
    private AnimationClip clip;
    private bool recording = false;
    private int index;
    private List<AnimationClip> sessionClips;
    private ViewportManager viewportManager;

    void Start()
    {
        DisableScript();
        viewportManager = GetComponent<ViewportManager>();
    }

    void Update()
    {
        if (scriptIsEnabled)
        {
            CheckIfActive();
        }
    }

    void StartRecording()
    {
        CreateNewClip();
        recorder = new GameObjectRecorder(viewport);
        recorder.BindComponentsOfType<Transform>(viewport, false);
        recorder.BindComponentsOfType<Camera>(viewport, false);
        recorder.BindComponentsOfType<AspectRatioManager>(viewport, false);

        sessionDirectory = SessionManager.GetSessionDirectory();

        viewportManager.StartRecordingIndicator();
        recording = true;
        Debug.Log("Recording started");
    }

    void StopRecording()
    {
        recorder.SaveToClip(clip, framerate);
        string fullPath = "Assets/" + Path.GetRelativePath(Application.dataPath, sessionDirectory) + clipName + ".anim";
        AssetDatabase.CreateAsset(clip, fullPath);
        AssetDatabase.SaveAssets();

        viewportManager.StopRecordingIndicator();
        recording = false;
        Debug.Log("Recording stopped");
    }

    void LateUpdate()
    {
        if (recording)
        {
            recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    void CreateNewClip()
    {
        clip = new AnimationClip();
        clipName = SessionManager.CreateRandomId();
        clip.legacy = false;

        /**if (namingConvention == NamingConvention.Random)
        {
            clipName = SessionManager.CreateRandomId();
        }

        else if (namingConvention == NamingConvention.Prefix)
        {
            if (customPrefix != "")
            {
                clipName = customPrefix + SessionManager.CreateRandomId();
            }

            else
            {
                Debug.Log("Custom prefix empty");
            }
        }

        else if (namingConvention == NamingConvention.Custom)
        {
            if (customName != "")
            {
                clipName = customName;
            }

            else
            {
                Debug.Log("Custom name empty");
            }
        }**/
    }

    public void SetFramerate(float fps)
    {
        framerate = fps;
    }

        void CheckIfActive()
    // Unitys integrated GetKeyDown() function is not available for XR-Controller based inputs
    // This function emulates the same behaviour for a given (InputDevice device) and button (activationButton)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputDevice);

        InputHelpers.IsPressed(device, toggleRecordingInput, out bool isPressed);
        
        if (isPressed && !recording && !wasActivated)
        {
            wasActivated = true;
            StartRecording();
        }

        if (!isPressed && wasActivated)
        {
            wasActivated = false;
        }

        if (isPressed && recording && !wasActivated)
        {
            StopRecording();
        }
    }

    public void DisableScript()
    {
        if (recording)
        {
            StopRecording();
        }

        scriptIsEnabled = false;
        viewport.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        viewport.gameObject.SetActive(true);
        viewportManager.ResetViewportPosition();
    }
}
