using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalCheckpointOffset : MonoBehaviour
{
    public float verticalOffset;
    
    private float defaultOffset;
    // Start is called before the first frame update
    void Start()
    {
        if (verticalOffset <= 0)
        {
            // Offset y-position with default
        }
        else
        {
            // Offset with custom value
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
