using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using Autodesk.Fbx;

public class AnimationExporter : MonoBehaviour
{
    public GameObject activeObject;
    public enum fileFormat {fbx, obj};

    private Component transformComponent;
    private Component cameraComponent;
    private fileFormat format;
    private string objectPath;
    private string objectName;
    private string basePath = "Assets/Exports/";
    private string exportPath;

    // Start is called before the first frame update
    void Start()
    {      
        objectName = activeObject.name;
        exportPath = basePath + objectName + "." + format.ToString();

        transformComponent = activeObject.GetComponent("Transform");
        cameraComponent = activeObject.GetComponent("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("x"))
        {
            Debug.Log(exportPath);
            ExportAnimation(exportPath);
        }
    }
    
    void ExportAnimation(string fileName)
    {
        using(FbxManager manager = FbxManager.Create())
        {
            // configure IO settings.
            FbxIOSettings ioSettings = FbxIOSettings.Create (manager, Globals.IOSROOT);
            manager.SetIOSettings(ioSettings);

            // Export the scene
            using (FbxExporter exporter = FbxExporter.Create(manager, "exporter"))
            {
                // Initialize the exporter.
                bool status = exporter.Initialize(fileName, -1, manager.GetIOSettings ());

                // Create a new scene to export
                //FbxScene scene = FbxScene.Create(manager, "myScene");
                FbxScene scene = FbxScene.Create(manager, "scene");

                // Export the scene to the file.
                exporter.Export(scene);
            }
        }
    }
}
