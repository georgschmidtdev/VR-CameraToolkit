using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class SerializedAnimation
{
    public AnimationClip clip;
}

public static class SerializationManager
{
    private static string saveDirectory = "/RecordedAnimations";
    private static string sessionID = "/currentSession/";

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
        animation.clip = clip;

        return animation;
    }

    public static AnimationClip Deserialize(SerializedAnimation animation)
    {
        return animation.clip;
    }
}
