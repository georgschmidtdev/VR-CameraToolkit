using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtManager : MonoBehaviour
{
    public Canvas informationCanvas;

    private GameObject vrCamera;
    private LookAtConstraint constraint;
    private bool scriptIsEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        DisableScript();
        vrCamera = GameObject.FindWithTag("VRCamera");
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = vrCamera.transform;
        constraint = gameObject.GetComponent<LookAtConstraint>();
        constraint.AddSource(source);
    }

    // Update is called once per frame
    void Update()
    {
       if (scriptIsEnabled)
       {
           constraint.constraintActive = true;
       }
       else
       {
           constraint.constraintActive = false;
       }
    }

    public void DisableScript()
    {
        scriptIsEnabled = false;
        informationCanvas.gameObject.SetActive(false);
    }

    public void EnableScript()
    {
        scriptIsEnabled = true;
        informationCanvas.gameObject.SetActive(true);
    }
}
