using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class SerializedAnimation
{
    public string clipName;
    public float frameRate;
    public float length;
    List<Vector3> position;
    List<Vector4> rotation;
    List<float> fov;
    List<float> focalLength;
}

public static class SerializationManager
{
    private static AnimationRecorder animationRecorder;
    private static string saveDirectory = "/RecordedAnimations";
    private static string sessionID = "/currentSession/";
    private static List<AnimationClip> sessionClips;

    public static void SaveFile(AnimationClip clip)
    {
        SerializedAnimation animation = Serialize(clip);
        string fileName = clip.name + ".saf";
        string jsonAnimation = JsonUtility.ToJson(animation);
        string dir = Application.persistentDataPath + saveDirectory + sessionID;
        Debug.Log(dir);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(dir + fileName, jsonAnimation);
    }

    public static AnimationClip LoadFile(string filePath)
    {
        string jsonAnimation = File.ReadAllText(filePath);
        SerializedAnimation animation = JsonUtility.FromJson<SerializedAnimation>(jsonAnimation);
        AnimationClip clip = Deserialize(animation);

        return clip;
    }

    public static SerializedAnimation Serialize(AnimationClip clip)
    {
        SerializedAnimation animation = new SerializedAnimation();
        animation.clipName = clip.name;
        animation.frameRate = clip.frameRate;
        animation.length = clip.frameRate;
        
        return animation;
    }

    public static AnimationClip Deserialize(SerializedAnimation animation)
    {
        AnimationClip clip = new AnimationClip();
        return clip;
    }
}
