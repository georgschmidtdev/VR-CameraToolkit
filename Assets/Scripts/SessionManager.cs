using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    private static string saveDirectory = Application.persistentDataPath + "/RecordedAnimations/";
    private static string sessionDirectory;
    private static int year;
    private static int month;
    private static int day;
    private static int hour;
    private static int minute;
    private static int second;

    private static string sessionId;

    void Start()
    {
        InitialDirectorySetup();
        SessionDirectorySetup();
    }

    private static void GetCurrentTime()
    {
        year = System.DateTime.Now.Year;
        month = System.DateTime.Now.Month;
        day = System.DateTime.Now.Day;

        hour = System.DateTime.Now.Hour;
        minute = System.DateTime.Now.Minute;
        second = System.DateTime.Now.Second;
    }

    public static string CreateRandomId()
    {
        GetCurrentTime();

        int seed = hour + minute + second;

        Random.InitState(seed);

        string randomId = Random.state.ToString();

        string sessionId = year + month + day + "_" + randomId;

        return sessionId;
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
   
    public static void SessionDirectorySetup()
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

    public static string GetCurrentSessionDirectory()
    {
        return sessionDirectory;
    }
}
