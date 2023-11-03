using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AnalyticsHandler : MonoBehaviour
{
    public static AnalyticsHandler Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    string url = "https://woodsy-wayfinder-default-rtdb.firebaseio.com/";

    bool sendingData = false;
    
    Queue<Tuple<List<Vector2>, int>> PathData;
    Queue<int> Attempts;
    Queue<float> CompletionTimes;
    Queue<int> Completions;
    Queue<Tuple<Vector2, int>> RobberInteractions;
    Queue<int> Resets;
    Queue<int> SceneSwitches;

    private void Start()
    {
        sendingData = false;
        PathData = new Queue<Tuple<List<Vector2>, int>>();
    }

    private void Update()
    {
        if (!sendingData)
        {
            if (PathData.Count > 0)
            {
                StartCoroutine(PostPathData_coroutine(PathData.Peek().Item1, PathData.Peek().Item2));
                PathData.Dequeue();
            }
            else if(Attempts.Count > 0)
            {
                StartCoroutine(PostAttemptsData_coroutine(Attempts.Peek()));
                PathData.Dequeue();
            }

        }
    }

    public void PostPathData(List<Vector2> path, int level)
    {
        List<Vector2> newList = new List<Vector2>(path);
        PathData.Enqueue(Tuple.Create(newList, level));
    }

    public void PostAttempts(int level)
    {
        Attempts.Enqueue(level);
    }

    public void PostCompletion(int level)
    {
        CompletionTimes.Enqueue(Time.time - SceneHandler.Instance.startLevelTime);
        Completions.Enqueue(level);
    }

    public void PostRobberInteractions(Vector2 robberPos, int level)
    {
        RobberInteractions.Enqueue(Tuple.Create(robberPos, level));
    }

    public void ResetUsage(int level)
    {
        Resets.Enqueue(level);
    }

    public void SceneUsage(int level)
    {
        SceneSwitches.Enqueue(level);
    }

    IEnumerator PostPathData_coroutine(List<Vector2> path, int level)
    {
        sendingData = true;

        string value_string = level.ToString();
        string json = "{ \"" + value_string + "\": [";
        for (int i=0; i<path.Count; i++)
        {
            Vector2 item = path[i];
            json += "\"" + item.x.ToString() + "," + item.y.ToString() + "\"";
            if (i + 1 < path.Count)
                json += ",";
        }
        json += "]}";

        string firebaseUrl = $"{url}gameData/levels/{value_string}.json";

        using (UnityWebRequest request = UnityWebRequest.Put(firebaseUrl, json))
        {
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Error sending data: {request.error}");
        }

        sendingData = false;
    }

    IEnumerator PostAttemptsData_coroutine(int level)
    {
        sendingData = true;

        //string value_string = level.ToString();
        //string json = "{ \"" + value_string + "\": [";
        //for (int i = 0; i < path.Count; i++)
        //{
        //    Vector2 item = path[i];
        //    json += "\"" + item.x.ToString() + "," + item.y.ToString() + "\"";
        //    if (i + 1 < path.Count)
        //        json += ",";
        //}
        //json += "]}";

        //string firebaseUrl = $"{url}gameData/levels/{value_string}.json";

        //using (UnityWebRequest request = UnityWebRequest.Put(firebaseUrl, json))
        //{
        //    request.method = "POST";
        //    request.SetRequestHeader("Content-Type", "application/json");

        //    yield return request.SendWebRequest();

        //    if (request.result != UnityWebRequest.Result.Success)
        //        Debug.LogError($"Error sending data: {request.error}");
        //}
        yield return null;

        sendingData = false;
    }
}
