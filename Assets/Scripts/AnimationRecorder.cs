using System.Collections;
using System.Collections.Generic;
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

    private string saveDirectory = "/RecordedAnimations";
    private string sessionID;
    private string fullDirectory;
    private string clipName;
    private float framerate = 24f;
    private string startRecordingKey = "i";
    private string stopRecordingKey = "o";
    private string deleteRecordingKey = "p";
    private string deleteIndexKey = "l";
    private bool scriptIsEnabled = false;
    private AnimationClip clip;
    private AnimationClip currentClip;
    private bool canRecord = true;
    private int index;
    private string currentClipName;
    private List<AnimationClip> sessionClips;

    void Start()
    {
        var savedIndex = PlayerPrefs.GetInt(activeObject.name + "Index");

        if (savedIndex != 0)
        {
            index = savedIndex;
        }

        recorder = new GameObjectRecorder(activeObject);
        recorder.BindComponentsOfType<Transform>(activeObject, false);
        recorder.BindComponentsOfType<Camera>(activeObject, false);

        clipName = activeObject.name + "_animation";
        clipName = clipName.Replace(" ", "_");
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

            if (Input.GetKeyDown(deleteIndexKey))
            {
                PlayerPrefs.DeleteKey(activeObject.name + "Index");
                index = 0;
                Debug.Log("Index reset");
            }
        }
    }

    private void StartRecording()
    {
        canRecord = true;
        CreateNewClip();
    }

    private void StopRecording()
    {
        canRecord = false;
        recorder.SaveToClip(currentClip);
        AssetDatabase.CreateAsset(currentClip, Application.persistentDataPath + saveDirectory  + currentClipName + ".anim");
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

        if (clip.name.Contains(clip.name))
        {
            clip.name = clipName + "_" + (index++);
            currentClipName = clip.name;
        }

        clip.frameRate = framerate;
        currentClip = clip;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(activeObject.name + "Index", index);
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
