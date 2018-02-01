using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
//using System.Security.Cryptography.X509Certificates;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class LuisManager : MonoBehaviour {

    [System.Serializable] //this class represents the LUIS response
    public class AnalysedQuery
    {
        public TopScoringIntentData topScoringIntent;
        public EntityData[] entities;
        public string query;
    }

    [System.Serializable] //this class contains the Intent LUIS determines to be the most likely
    public class TopScoringIntentData
    {
        public string intent;
        public float score;
    }

    [System.Serializable] //this class contains data for an Entity
    public class EntityData
    {
        public string entity;
        public string type;
        public int startIndex;
        public int endIndex;
        public float score;
    }

    public static LuisManager instance;

    //Substitute the value of authorizationKey with your own Key
    string luisEndpoint = "https://eastus.api.cognitive.microsoft.com/luis/v2.0/apps/54926605-efec-4983-88a7-86e6c89551e0?subscription-key=166aa92f0c124acbb09a39299d1a31b0&verbose=true&timezoneOffset=-300&q=";

    private void Awake()
    {
        // allows this class instance to behave like a singleton
        instance = this;
    }

    /// <summary>
    /// Call LUIS to submit a dictation result.
    /// </summary>
    public IEnumerator SubmitRequestToLuis(string dictationResult)
    {
        WWWForm form = new WWWForm();
        string queryString;
        queryString = string.Concat(Uri.EscapeDataString(dictationResult));

        using (UnityWebRequest www = UnityWebRequest.Get(luisEndpoint + queryString))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            long responseCode = www.responseCode;

            try
            {
                using (Stream stream = GenerateStreamFromString(www.downloadHandler.text))
                {
                    StreamReader reader = new StreamReader(stream);
                    AnalysedQuery analysedQuery = new AnalysedQuery();
                    analysedQuery = JsonUtility.FromJson<AnalysedQuery>(www.downloadHandler.text);

                    //analyse the elements of the response 
                    AnalyseResponseElements(analysedQuery);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Luis Request Exception Message: " + e.Message);
            }
            yield return null;
        }
    }
    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private void AnalyseResponseElements(AnalysedQuery aQuery)
    {
        string topIntent = aQuery.topScoringIntent.intent;

        //create a dictionary of entities associated with their type
        Dictionary<string, string> entityDic = new Dictionary<string, string>();

        foreach (EntityData ed in aQuery.entities)
        {
            entityDic.Add(ed.type, ed.entity);
        }

        //depending on the topmost recognised intent, read the entities name
        switch (aQuery.topScoringIntent.intent)
        {
            case "ChangeObjectColor":

                string targetForColor = null;
                string color = null;
                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForColor = pair.Value;
                    }
                    else if (pair.Key == "color")
                    {
                        color = pair.Value;
                    }
                }
                Behaviours.instance.ChangeTargetColor(targetForColor, color);
                break;

            case "ChangeObjectSize":

                string targetForSize = null;
                foreach (var pair in entityDic)
                {
                    if (pair.Key == "target")
                    {
                        targetForSize = pair.Value;
                    }
                }
                if (entityDic.ContainsKey("upsize") == true)
                {
                    Behaviours.instance.UpSizeTarget(targetForSize);
                }
                else if (entityDic.ContainsKey("downsize") == true)
                {
                    Behaviours.instance.DownSizeTarget(targetForSize);
                }
                break;
        }
    }
}
