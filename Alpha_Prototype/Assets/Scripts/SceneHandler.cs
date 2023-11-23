using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public Vector2Int Min;
    public Vector2Int Max;

    public List<Vector2> DeliveryTargets;
    public List<Sprite> DeliveryRequirements;
    public List<Vector2> UsedTiles;

    public int Packing_Level;
    public int Plotting_Level;
    public int New_Level;

    public List<Vector2> Path;
    public int startOffset, endOffset;
    public bool PathConnected;
    public List<Vector2> Delivered;

    public Queue<Vector2> StackedItems;
    public Dictionary<Vector2, Vector2> StackedItemsPositions;

    public List<Vector2> deletedTiles;
    public bool showDeletion;

    public static SceneHandler Instance = null;

    //UI Info
    public int UI_resetCounts = 0;
    public int UI_packedItems = 0;
    public int UI_stagesCount = 0;

    public float startLevelTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StackedItems = new Queue<Vector2>();
            StackedItemsPositions = new Dictionary<Vector2, Vector2>();
            deletedTiles = new List<Vector2>();
            showDeletion = true;
            PathConnected = false;
            startLevelTime = Time.time;

            Invoke("AttemptsCntr", 0.03f);
        }
        else
        {
            if (Instance.Packing_Level == Packing_Level)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
                StackedItems = new Queue<Vector2>();
                StackedItemsPositions = new Dictionary<Vector2, Vector2>();
                deletedTiles = new List<Vector2>();
                showDeletion = true;
                PathConnected = false;
                startLevelTime = Time.time;
                Invoke("AttemptsCntr", 0.03f);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    void AttemptsCntr()
    {
        AnalyticsHandler.Instance.PostAttempts(Packing_Level);
    }

    public void SwitchToPlotting()
    {
        StackedItems.Clear();
        List<Vector2> positions = new List<Vector2>();

        foreach (var entry in StackedItemsPositions)
            positions.Add(entry.Value);

        positions.Sort((a, b) => {
            return a.y.CompareTo(b.y);
        });
        positions.Reverse();

        for (int idx = 0; idx < positions.Count; idx++)
        {
            foreach (var entry in StackedItemsPositions)
            {
                if (entry.Value == positions[idx])
                {
                    StackedItems.Enqueue(entry.Key);
                    break;
                }
            }
        }

        SceneManager.LoadScene(Plotting_Level);
    }

    public void SwitchToPacking(bool deleteQueue)
    {
        if (deleteQueue)
        {
            UI_packedItems = Math.Max(UI_packedItems, StackedItems.Count);
            UI_stagesCount += 1;
            while (StackedItems.Count > 0)
            {
                Delivered.Add(StackedItems.Peek());
                StackedItemsPositions.Remove(StackedItems.Peek());
                StackedItems.Dequeue();
            }
            StackedItems.Clear();
            StackedItemsPositions.Clear();
            showDeletion = true;
            SelectRandomDeleteTiles();
            Path.Clear();
            PathConnected = false;

            AnalyticsHandler.Instance.PostSceneUsage(Packing_Level);
        }
        else
        {
            Path = PlottingManager.Instance.getRawPath(out startOffset, out endOffset);
            PathConnected = PlottingManager.Instance.isConnected;
            showDeletion = false;
        }
        if (Delivered.Count == DeliveryTargets.Count)
            SceneManager.LoadScene(New_Level);
        else
            SceneManager.LoadScene(Packing_Level);
    }

    public void ResetToPacking()
    {
        StackedItems.Clear();
        StackedItemsPositions.Clear();
        Path.Clear();
        Delivered.Clear();
        showDeletion = true;
        PathConnected = false;
        UI_resetCounts = 0;
        UI_packedItems = 0;
        UI_stagesCount = 0;

        startLevelTime = Time.time;
        deletedTiles.Clear();

        AnalyticsHandler.Instance.PostAttempts(Packing_Level);

        Invoke("AttemptsCntr", 0.03f);
        SceneManager.LoadScene(Packing_Level);
    }

    void SelectRandomDeleteTiles()
    {
        List<Vector2> path = new List<Vector2>(PlottingManager.Instance.getRawPath(out startOffset, out endOffset));
        UsedTiles = path;
        path.RemoveAt(0);
        path.RemoveAt(path.Count - 1);
        int count = Math.Max(4, (path.Count * (70/100)));
        for (int i = 0; i < count; i++)
        {
            do
            {
                Vector2 pos = path[UnityEngine.Random.Range(0, path.Count)];
                if (!deletedTiles.Contains(pos))
                {
                    deletedTiles.Add(pos);
                    break;
                }

            } while (true);
        }
    }
}