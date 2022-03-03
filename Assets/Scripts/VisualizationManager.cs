using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizationManager : MonoBehaviour
{
    private bool scriptIsEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            
        }
    }

    //  Approach 1:
    //      Select .anim file
    //      Instantiate temporary gameObject
    //      Attach Animation to gameObject
    //          Offset Animation one frame at a time
    //          Store the coordinates at each frame
    //          Destroy object
    //      Instantiate new gameObject with LineRenderer
    //      Draw line from stored coordinates

    //  Approach 2:
    //      Select .anim file
    //      Read file
    //      Extract coordinates
    //      ... (see above)      

    public void DisableScript()
    {
        scriptIsEnabled = false;
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
    }
}
