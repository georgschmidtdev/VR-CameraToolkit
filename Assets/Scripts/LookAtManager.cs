using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class LookAtManager : MonoBehaviour
{
    public GameObject constraintPivot;
    public Canvas informationCanvas;

    private GameObject vrCamera;
    private LookAtConstraint constraint;

    // Start is called before the first frame update
    void Start()
    {
        vrCamera = GameObject.FindWithTag("MainCamera");
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = vrCamera.transform;
        constraint = constraintPivot.GetComponent<LookAtConstraint>();
        constraint.AddSource(source);
        constraint.constraintActive = true;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}