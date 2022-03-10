using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SerializationManager : MonoBehaviour
{
    [Serializable]
    public class SerializedAnimation
    {
        public string name;
        public float length;
        public float framerate;
        public AnimationClip clip;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string Serialize(AnimationClip clip)
    {
        SerializedAnimation animation = new SerializedAnimation();
        animation.name = clip.name;
        animation.length = clip.length;
        animation.length = clip.frameRate;
        animation.clip = clip;
        string serializedAnimationClip = JsonUtility.ToJson(animation);

        return serializedAnimationClip;
    }

    public AnimationClip DeSerialize(string serializedAnimationClip)
    {
        SerializedAnimation animation = JsonUtility.FromJson<SerializedAnimation>(serializedAnimationClip);
        
        return animation.clip;
    }
}
