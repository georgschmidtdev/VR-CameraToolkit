using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SerializationManager : MonoBehaviour
{
    [Serializable]
    public class SerializedAnimation
    {
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

    public void Serialize(AnimationClip animatiion)
    {
        SerializedAnimation animation = new SerializedAnimation();
        animation.clip = animatiion;
        string serializedAnimation = JsonUtility.ToJson(animation);
    }

    public void DeSerialize(string serializedAnimation)
    {
        SerializedAnimation animation = JsonUtility.FromJson<SerializedAnimation>(serializedAnimation);
    }
}
