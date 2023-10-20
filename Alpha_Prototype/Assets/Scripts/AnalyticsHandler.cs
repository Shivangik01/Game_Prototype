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

    //IEnumerator PostPathData_coroutine(List<Vector2> path, int level)
    //{
    //    sendingData = true;
    //    string URL = url + "levels/" + level.ToString() + ".json";

    //    using (UnityWebRequest response = UnityWebRequest.Get(URL))
    //    {
    //        yield return response.SendWebRequest();
    //        if (response.responseCode == 200)
    //        {
    //            Debug.Log(response.downloadHandler.text);
                
    //        }
    //    }
    //    sendingData = false;
    //    yield return null;
    //}

    IEnumerator PostPathData_coroutine(List<Vector2> path, int level)
    {
        // Indicate that data is being sent.
        sendingData = true;

        // Convert the path data to JSON format.
        // Here, you may need to create a wrapper class if your data structure is complex.
        string jsonData = JsonUtility.ToJson(path); // This might require adjustments based on your data structure.

        // Construct the URL for the Firebase database.
        // Ensure this URL points to the correct location in your database (i.e., 'levels/level_x').
        string firebaseUrl = $"{url}levels/{level}.json";

        // Create a PUT request with the Firebase URL and the JSON payload.
        using (UnityWebRequest request = UnityWebRequest.Put(firebaseUrl, jsonData))
        {
            // Set the content type of the request.
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request and wait for a response.
            yield return request.SendWebRequest();

            // Check for errors in the request.
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending data: {request.error}");
            }
            else
            {
                // If successful, log the response.
                Debug.Log("Data sent successfully. Response: " + request.downloadHandler.text);
            }
        }

        // Data has been sent, so we're no longer in the sending state.
        sendingData = false;
    }

}
