using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimationRecorder : MonoBehaviour
{
    GameObjectRecorder recorder;
    public GameObject activeObject;
    string saveLocation = "Assets/Recordings/";
    string clipName;
    float framerate = 24f;

    string startRecordingKey = "i";
    string stopRecordingKey = "o";
    string deleteRecordingKey = "p";
    string deleteIndexKey = "l";

    private AnimationClip clip;
    private AnimationClip currentClip;
    private bool canRecord = true;
    private int index;
    private string currentClipName;

    void Start()
    {
        if (clip == null)
        {
            CreateNewClip();
        }

        var savedIndex = PlayerPrefs.GetInt(activeObject.name + "Index");

        if (savedIndex != 0)
        {
            index = savedIndex;
        }

        recorder = new GameObjectRecorder(activeObject);
        recorder.BindComponentsOfType<Transform>(activeObject, true);

        //if (clipName == "")
        //{
            clipName = activeObject.name + "_animation";
        //}
    }

    void Update()
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

    private void StartRecording()
    {
        canRecord = true;
        CreateNewClip();
    }

    private void StopRecording()
    {
        canRecord = false;
        recorder.SaveToClip(currentClip);
        AssetDatabase.CreateAsset(currentClip, saveLocation + currentClipName + ".anim");
        AssetDatabase.SaveAssets();
    }

    private void DeleteRecording()
    {
        if (canRecord)
        {
            return;
        }

        if (!AssetDatabase.Contains(currentClip))
        {
            return;
        }

        AssetDatabase.DeleteAsset(saveLocation + currentClipName + ".anim");
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
}
