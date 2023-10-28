using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public List<Vector2> gridPoints;
    public List<bool> occupied;

    public GridLayoutGroup gridLayout;

    public static GridManager Instance = null;

    public Button deliverButton;
    


    int items = 0;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        items = 0;
        deliverButton.interactable = false;
    }

    private void Start()
    {
        occupied = new List<bool>();

        foreach (Transform child in gridLayout.transform)
        {
            if (child.childCount > 0)
                occupied.Add(true);
            else
                occupied.Add(false);
        }
    }


    public List<Vector2> getTiles()
    {
        if (gridPoints == null)
            gridPoints = new List<Vector2>();
        else
            gridPoints.Clear();
        foreach (Transform child in gridLayout.transform)
        {
            Vector2 pos = child.transform.position;
            gridPoints.Add(pos);
        }

        return gridPoints;
    }

    public bool inGrid(List<Vector2> positions, out Vector2 lockedPos, bool bypassOccupied = false)
    {
        getTiles();

        int count = 0;
        Vector2 gridPoint = Vector2.zero;
        for(int i=0; i<gridPoints.Count; i++)
        {
            if (!bypassOccupied)
            {
                if (occupied[i])
                    continue;
            }

            for(int j=0; j<positions.Count; j++)
            {
                Vector2 pos = positions[j];
                if(Vector2.Distance(pos, gridPoints[i]) < gridLayout.cellSize.x/2)
                {
                    count += 1;
                    if (j == 0)
                        gridPoint = gridPoints[i];
                    break;
                }
            }
        }

        bool answer = false;
        

        if (count == positions.Count)
        {
            answer = true;
            lockedPos = gridPoint;
        }
        else
        {
            lockedPos = Vector2.zero;
        }
        
        return answer;
    }

    public void markOccupied(List<Vector2> positions, bool set)
    {
        if (set)
            items+=1;
        else
            items-=1;

        if (items > 0)
        {
            deliverButton.interactable = true;
        }
        else
        {
            deliverButton.interactable = false;
        }
        for (int i = 0; i < gridPoints.Count; i++)
            {
                for (int j = 0; j < positions.Count; j++)
                {
                    Vector2 pos = positions[j];
                    if (Vector2.Distance(pos, gridPoints[i]) < gridLayout.cellSize.x / 2)
                    {
                        occupied[i] = set;
                        break;
                    }
                }
            }
    }
}
