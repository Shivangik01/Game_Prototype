using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsHandler : MonoBehaviour
{
    public static AnalyticsHandler Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    string url = "https://woodsy-wayfinder-default-rtdb.firebaseio.com/";

    bool sendingData = false;
    Queue<Tuple<List<Vector2>, int>> PathData;

    private void Start()
    {
        sendingData = false;
        PathData = new Queue<Tuple<List<Vector2>, int>>();
    }

    private void Update()
    {
        if (!sendingData && PathData.Count > 0)
        {
            StartCoroutine(PostPathData_coroutine(PathData.Peek().Item1, PathData.Peek().Item2));
            PathData.Dequeue();
        }
    }

    public void PostPathData(List<Vector2> path, int level)
    {
        PathData.Enqueue(Tuple.Create(path, level));
    }

    IEnumerator PostPathData_coroutine(List<Vector2> path, int level)
    {

    sendingData = true;
        string URL = url + "levels/" + level.ToString() + ".json";

        using (UnityWebRequest response = UnityWebRequest.Get(URL))
{
    yield return response.SendWebRequest();
    if (response.responseCode == 200)
    {
        string json = @"{
                    1:
                    {
                        ""uni_number"": 001,
                        ""level"" : 3,
                        ""Hp"" : 15
                    },
                    2:
                    {
                        ""uni_number"": 000,
                        ""level"" : 0,
                        ""Hp"" : 0
                    }
                    }";

        BoundsConve
        var values = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
        foreach (var uni in values)
        {
            //you can print values here or add to a list or ...
            string uni_number = uni.Value["uni_number"];
            string levels = uni.Value["level"];
            string Hp = uni.Value["Hp"];
            Debug.Log(uni_number);
            Debug.Log(levels);
            Debug.Log(Hp);
        }
    }
}

sendingData = false;
yield return null;
    }
}
