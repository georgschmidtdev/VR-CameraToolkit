using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrowserEntryManager : MonoBehaviour
{
    public GameObject nameField;

    private GameObject rig;
    private TextMeshProUGUI animationName;
    private AnimationManager animationManager;
    // Start is called before the first frame update
    void Start()
    {
        animationName = nameField.GetComponent<TextMeshProUGUI>();
        rig = GameObject.FindWithTag("Rig");
        animationManager = rig.GetComponent<AnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisibility()
    {
        animationManager.ToggleVisibility(animationName.text);
    }

    public void DeleteEntry()
    {
        animationManager.DeleteAnimation(animationName.text);
        Destroy(gameObject);
    }
}
