using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public Vector2Int Min;
    public Vector2Int Max;

    public List<Vector2> DeliveryTargets;
    public List<Sprite> DeliveryRequirements;

    public int Packing_Level;
    public int Plotting_Level;
    public int New_Level;

    public List<Vector2> Path;
    public int startOffset, endOffset;
    public List<Vector2> Delivered;

    public Queue<Vector2> StackedItems;
    public Dictionary<Vector2, Vector2> StackedItemsPositions;


    public static SceneHandler Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StackedItems = new Queue<Vector2>();
            StackedItemsPositions = new Dictionary<Vector2, Vector2>();
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
            }
        }

        DontDestroyOnLoad(gameObject);
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
            while (StackedItems.Count > 0)
            {
                Delivered.Add(StackedItems.Peek());
                StackedItemsPositions.Remove(StackedItems.Peek());
                StackedItems.Dequeue();
            }
            StackedItems.Clear();
            StackedItemsPositions.Clear();

            Path.Clear();
        }
        else
        {
            Path = PlottingManager.Instance.getRawPath(out startOffset, out endOffset);
        }
        if (Delivered.Count == DeliveryTargets.Count)
            SceneManager.LoadScene(New_Level);
        else
            SceneManager.LoadScene(Packing_Level);
    }
}