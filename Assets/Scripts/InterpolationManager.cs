using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolationManager : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = CalculatePosition();
    }

    Vector3 CalculatePosition()
    {
        float interpolatedX = Interpolate(leftHand.transform.position.x, rightHand.transform.position.x);
        float interpolatedY = Interpolate(leftHand.transform.position.y, rightHand.transform.position.y);
        float interpolatedZ = Interpolate(leftHand.transform.position.z, rightHand.transform.position.z);

        Vector3 newPosition = new Vector3(interpolatedX, interpolatedY, interpolatedZ);
        return newPosition;
    }

    float Interpolate(float firstValue, float secondValue)
    {
        float newValue = (firstValue + secondValue) / 2;
        return newValue;
    }
}
