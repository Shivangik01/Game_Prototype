using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;

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
        Attempts = new Queue<int>();
        CompletionTimes = new Queue<float> ();
        Completions = new Queue<int>();

        RobberInteractions = new Queue<Tuple<Vector2, int>> ();
        Resets = new Queue<int>();
        SceneSwitches = new Queue<int>();
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
                Attempts.Dequeue();
            }
            else if (CompletionTimes.Count > 0)
            {
                StartCoroutine(PostCompletionTimesData_coroutine(Completions.Peek(), CompletionTimes.Peek()));
                CompletionTimes.Dequeue();
                Completions.Dequeue();
            }
            else if(RobberInteractions.Count > 0)
            {
                StartCoroutine(PostRobberData_coroutine(RobberInteractions.Peek().Item2, RobberInteractions.Peek().Item1));
                RobberInteractions.Dequeue();
            }
            else if (Resets.Count > 0)
            {
                StartCoroutine(PostResetsData_coroutine(Resets.Peek()));
                Resets.Dequeue();
            }
            else if(SceneSwitches.Count > 0)
            {
                StartCoroutine(PostSceneSwitchData_coroutine(SceneSwitches.Peek()));
                SceneSwitches.Dequeue();
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
        Completions.Enqueue(level);
        CompletionTimes.Enqueue(Time.time - SceneHandler.Instance.startLevelTime);
    }

    public void PostRobberInteractions(Vector2 robberPos, int level)
    {
        RobberInteractions.Enqueue(Tuple.Create(robberPos, level));
    }

    public void PostResetUsage(int level)
    {
        Resets.Enqueue(level);
    }

    public void PostSceneUsage(int level)
    {
        SceneSwitches.Enqueue(level);
    }

    IEnumerator PostPathData_coroutine(List<Vector2> path, int level)
    {
        sendingData = true;

        string value_string = level.ToString();
        string json = "[";
        for (int i=0; i<path.Count; i++)
        {
            Vector2 item = path[i];
            json += "\"" + item.x.ToString() + "," + item.y.ToString() + "\"";
            if (i + 1 < path.Count)
                json += ",";
        }
        json += "]";

        string firebaseUrl = $"{url}gameData/levels/{value_string}/tiles.json";

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

        string value_string = level.ToString();
        string firebaseUrl = $"{url}gameData/levels/{value_string}/";
        using (UnityWebRequest request = UnityWebRequest.Get(firebaseUrl+ "attempts.json"))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                if (result == "null")
                    result = "1";
                else
                    result = (int.Parse(result)+1).ToString();

                result = "{ \"attempts\": " + result + "}";

                using(UnityWebRequest sendData = UnityWebRequest.Put(firebaseUrl+".json", result))
                {
                    sendData.method = "PATCH";
                    sendData.SetRequestHeader("Content-Type", "application/json");

                    yield return sendData.SendWebRequest();
                    if (sendData.result != UnityWebRequest.Result.Success)
                        Debug.LogError($"Error sending data: {sendData.error}");
                }
            }
        }
        yield return null;

        sendingData = false;
    }

    IEnumerator PostCompletionTimesData_coroutine(int level, float time)
    {
        sendingData = true;

        string value_string = level.ToString();
        string json = "[" + time.ToString() + "]";

        string firebaseUrl = $"{url}gameData/levels/{value_string}/completionTimes.json";

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

    IEnumerator PostRobberData_coroutine(int level, Vector2 value)
    {
        sendingData = true;

        string value_string = level.ToString();
        string json = "[\"" + value.x.ToString() + "," + value.y.ToString() + "\"]";
        
        string firebaseUrl = $"{url}gameData/levels/{value_string}/robberies.json";

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

    IEnumerator PostResetsData_coroutine(int level)
    {
        sendingData = true;

        string value_string = level.ToString();
        string firebaseUrl = $"{url}gameData/levels/{value_string}/";
        using (UnityWebRequest request = UnityWebRequest.Get(firebaseUrl + "resets.json"))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                if (result == "null")
                    result = "1";
                else
                    result = (int.Parse(result) + 1).ToString();

                result = "{ \"resets\": " + result + "}";

                using (UnityWebRequest sendData = UnityWebRequest.Put(firebaseUrl + ".json", result))
                {
                    sendData.method = "PATCH";
                    sendData.SetRequestHeader("Content-Type", "application/json");

                    yield return sendData.SendWebRequest();
                    if (sendData.result != UnityWebRequest.Result.Success)
                        Debug.LogError($"Error sending data: {sendData.error}");
                }
            }
        }
        yield return null;

        sendingData = false;
    }

    IEnumerator PostSceneSwitchData_coroutine(int level)
    {
        sendingData = true;

        string value_string = level.ToString();
        string firebaseUrl = $"{url}gameData/levels/{value_string}/";
        using (UnityWebRequest request = UnityWebRequest.Get(firebaseUrl + "sceneSwitches.json"))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                string result = request.downloadHandler.text;
                if (result == "null")
                    result = "1";
                else
                    result = (int.Parse(result) + 1).ToString();

                result = "{ \"sceneSwitches\": " + result + "}";

                using (UnityWebRequest sendData = UnityWebRequest.Put(firebaseUrl + ".json", result))
                {
                    sendData.method = "PATCH";
                    sendData.SetRequestHeader("Content-Type", "application/json");

                    yield return sendData.SendWebRequest();
                    if (sendData.result != UnityWebRequest.Result.Success)
                        Debug.LogError($"Error sending data: {sendData.error}");
                }
            }
        }
        yield return null;

        sendingData = false;
    }

}
