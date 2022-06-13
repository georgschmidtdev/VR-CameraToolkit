using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioManager : MonoBehaviour
{
    public float aspectRatio;
    private Camera previewCamera;
    // Start is called before the first frame update
    void Start()
    {
        aspectRatio = 1.0f;
        previewCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aspectRatio != previewCamera.aspect)
        {
            previewCamera.aspect = aspectRatio;
        }
    }

    public void UpdateAspectRatio(Vector2 ratio)
    {
        float newAspectRatio = ratio.x / ratio.y;
        aspectRatio = newAspectRatio;
        ApplyAspectRatio();
    }

    public float GetAspectRatio()
    {
        return aspectRatio;
    }

    public void ApplyAspectRatio()
    {
        gameObject.GetComponent<Camera>().aspect = aspectRatio;
    }
}
