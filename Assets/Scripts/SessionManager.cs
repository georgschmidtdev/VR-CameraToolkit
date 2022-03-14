using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager
{
    public string CreateSessionID()
    {
        string year = System.DateTime.Now.Year.ToString();
        string month = System.DateTime.Now.Month.ToString();
        string day = System.DateTime.Now.Day.ToString();

        int hour = System.DateTime.Now.Hour;
        int minute = System.DateTime.Now.Minute;
        int second = System.DateTime.Now.Second;

        int seed = hour + minute + second;

        Random.InitState(seed);

        string randomId = Random.state.ToString();

        string sessionId = year + month + day + "_" + randomId;

        return sessionId;
    }
}
