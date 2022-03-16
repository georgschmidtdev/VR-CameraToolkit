using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportManager : MonoBehaviour
{
    public GameObject viewport;
    public GameObject viewportCamera;
    public GameObject viewportInterface;
    public enum AspectRatio {Standard, Square, HDTV, Panavision, Custom};
    public AspectRatio aspectRatio;

    private Vector2 standardAspect;
    private Vector2 squareAspect;
    private Vector2 hdtvAspect;
    private Vector2 panavisionAspect;
    private Vector2 customAspect;
    private Dictionary<AspectRatio, Vector2> aspectRatioDictionary;

    // Start is called before the first frame update
    void Start()
    {
        squareAspect = new Vector2(1.0f, 1.0f);
        standardAspect = new Vector2(4.0f, 3.0f);
        hdtvAspect = new Vector2(16.0f, 9.0f);
        panavisionAspect = new Vector2(2.39f, 1.0f);
        customAspect = Vector2.zero;

        aspectRatioDictionary.Add(AspectRatio.Square, squareAspect);
        aspectRatioDictionary.Add(AspectRatio.Standard, standardAspect);
        aspectRatioDictionary.Add(AspectRatio.HDTV, hdtvAspect);
        aspectRatioDictionary.Add(AspectRatio.Panavision, panavisionAspect);
        aspectRatioDictionary.Add(AspectRatio.Custom, customAspect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateCustomAspectRatio()
    {
        aspectRatioDictionary.Remove(AspectRatio.Custom);
        aspectRatioDictionary.Add(AspectRatio.Custom, customAspect);
    }

    private void ChangeViewportAspectRatios()
    {
        
    }
}
