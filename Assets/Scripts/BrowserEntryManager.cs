using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrowserEntryManager : MonoBehaviour
{
    public TMP_Dropdown previewDropdown;
    public AnimationManager animationManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVisibility()
    {
        if (previewDropdown.options.Count > 0)
        {
            animationManager.ToggleVisibility(previewDropdown.options[previewDropdown.value].text);
        }
    }

    public void DeleteEntry()
    {
        if (previewDropdown.options.Count > 0)
        {
            animationManager.DeleteAnimation(previewDropdown.options[previewDropdown.value].text);
        }
    }
}
