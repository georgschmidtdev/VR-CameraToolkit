using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static string saveDirectory;
    private static string sessionDirectory;
    private static string year;
    private static string month;
    private static string day;
    private static int hour;
    private static int minute;
    private static int second;
    private static string sessionId;

    void Start()
    {
        saveDirectory = Application.dataPath + "/Resources/RecordedAnimations/";
        InitialDirectorySetup();
        SessionDirectorySetup();
    }

    private static void GetCurrentTime()
    {
        year = System.DateTime.Now.Year.ToString();
        month = System.DateTime.Now.Month.ToString();
        day = System.DateTime.Now.Day.ToString();

        hour = System.DateTime.Now.Hour;
        minute = System.DateTime.Now.Minute;
        second = System.DateTime.Now.Second;
    }

    public static string CreateRandomId()
    {
        GetCurrentTime();

        int seed = hour + minute + second;

        Random.InitState(seed);

        string randomId = Random.value.ToString();

        return randomId;
    }
    
    public static void CreateSessionId()
    {
        GetCurrentTime();
        sessionId = year + month + day + "_" + hour + minute + second + "/";
    }

    private void InitialDirectorySetup()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
    }
   
    private void SessionDirectorySetup()
    {
        CreateSessionId();
        sessionDirectory = saveDirectory + GetSessionId();

        if (!Directory.Exists(sessionDirectory))
        {
            Directory.CreateDirectory(sessionDirectory);
        }
    }

    public static string GetSessionId()
    {
        if (sessionId == null)
        {
            Debug.Log("Error getting session ID, please try again");
            return null;
        }

        else
        {
            return sessionId;
        }
    }

    public static string GetSaveDirectory()
    {
        return saveDirectory;
    }

    public static string GetSessionDirectory()
    {
        return sessionDirectory;
    }
}
