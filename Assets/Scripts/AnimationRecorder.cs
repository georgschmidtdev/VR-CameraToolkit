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
    GameObjectRecorder recorder;
    public GameObject activeObject;
    public XRNode inputDevice;
    public InputHelpers.Button startRecordingInput;
    public InputHelpers.Button stopRecordingInput;
    public InputHelpers.Button deleteRecordingInput;
    public string customPrefix = "";
    public string customName = "";
    public enum NamingConvention {Random, Prefix, Custom};
    public NamingConvention namingConvention;

    private string sessionDirectory;
    private string sessionId;
    private string clipName;
    private float framerate = 24f;
    private string startRecordingKey = "i";
    private string stopRecordingKey = "o";
    private string deleteRecordingKey = "p";
    private bool scriptIsEnabled = false;
    private bool overwriteExistingFiles = false;
    private AnimationClip clip;
    private AnimationClip currentClip;
    private bool canRecord = true;
    private int index;
    private List<AnimationClip> sessionClips;

    void Start()
    {
        recorder = new GameObjectRecorder(activeObject);
        recorder.BindComponentsOfType<Transform>(activeObject, false);
        recorder.BindComponentsOfType<Camera>(activeObject, false);
    }

    void Update()
    {
        if (scriptIsEnabled)
        {
            if (Input.GetKeyDown(startRecordingKey))
            {
                StartRecording();
                Debug.Log("Recording started");
            }

            if (Input.GetKeyDown(stopRecordingKey))
            {
                StopRecording();
                Debug.Log("Recording stopped");
            }

            if (Input.GetKeyDown(deleteRecordingKey))
            {
                DeleteRecording();
                Debug.Log("Recording deleted");
            }
        }
    }

    private void StartRecording()
    {
        sessionDirectory = SessionManager.GetSessionDirectory();
        canRecord = true;
        CreateNewClip();
    }

    private void StopRecording()
    {
        canRecord = false;
        recorder.SaveToClip(currentClip);
        string fullPath = sessionDirectory + clipName + ".anim";
        AssetDatabase.CreateAsset(currentClip, fullPath);

        if (overwriteExistingFiles)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            AssetDatabase.SaveAssets();
        }

        else
        {
            if (File.Exists(fullPath) && namingConvention == NamingConvention.Custom)
            {
                Debug.Log("File already exists, please use another name or change the naming convention");
            }

            else
            {
                AssetDatabase.SaveAssets();
            }
        }
    }

    private void DeleteRecording()
    {
        if (canRecord)
        // Dont delete during recording
        {
            return;
        }

        if (true)
        // File does not exist
        {
            return;
        }
        // ToDo:
        // Rework saving functionality to avoid using UnityEditor namespace
    }

    private void LateUpdate()
    {
        if (clip == null)
        {
            return;
        }

        if (canRecord)
        {
            recorder.TakeSnapshot(Time.deltaTime);
        }
    }

    private void CreateNewClip()
    {
        clip = new AnimationClip();

        if (namingConvention == NamingConvention.Random)
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
        }
        
        clip.frameRate = framerate;
        currentClip = clip;
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
    }
}
