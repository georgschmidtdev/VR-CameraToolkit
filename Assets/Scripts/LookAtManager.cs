using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtManager : MonoBehaviour
{
    public Canvas informationCanvas;

    private GameObject vrCamera;
    private LookAtConstraint constraint;

    // Start is called before the first frame update
    void Start()
    {
        //DisableScript();
        vrCamera = GameObject.FindWithTag("MainCamera");
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = vrCamera.transform;
        constraint = gameObject.GetComponent<LookAtConstraint>();
        constraint.AddSource(source);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}