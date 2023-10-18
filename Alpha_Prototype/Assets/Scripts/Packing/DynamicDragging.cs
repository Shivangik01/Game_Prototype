using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//traverse the grid add the destinations in a set and return.
//add visibility parameter based on delivered scene


// hashmap with levels at start
// based on row column, check if the immediate left right up wtv is free
// place there

public class DynamicDragging : MonoBehaviour
{
    public Button deliver;

    private bool hasObject = false;

    private float snapSensivity = 50.0f;

    Transform parentAfterDrag;

    private Dictionary<int, float> gridRows;
    private Dictionary<int, float> gridColumns;
    public GridLayoutGroup gridLayoutGroup;

    bool[,] occupied;

    public int rows;
    public int columns;

    List<string> occupiedList = new List<string> {};

    private Vector3 gridPoint;
    public Vector3 lastPosition;
    private Vector3 startPosition;
    public string piece;

    public static bool isOccupied = false;

    private static string occupiedBy;

   

    public void Start()
    {
        deliver.interactable = false;

        occupied = new bool[rows, columns];

        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;
        float gridX = gridLayoutGroup.transform.position.x;
        float gridY = gridLayoutGroup.transform.position.y;

        gridRows = new Dictionary<int, float>();
        gridColumns = new Dictionary<int, float>();

        // populate rows and columns
        // even split and give; odd center is same

        for (int i = 0; i <rows/2; i++)
        {
            gridRows[i] = gridY - (rows / 2 - i - 1 / 2)*cellSize.y + (rows / 2 - i - 1 / 2) * spacing.y;
        }
        for (int i = rows / 2; i < rows; i++)
        {
            if (i - rows / 2 > 0)
            {
                gridRows[i] = gridY + (i - rows / 2 - 1 / 2) * cellSize.x + (i - rows / 2 - 1 / 2) * spacing.x;
            }
            else
            {
                gridRows[i] = gridY;
            }
        }

        for (int i = 0; i < columns / 2; i++)
        {
            gridColumns[i] = gridX - (columns / 2 - i - 1 / 2) * cellSize.x + (columns / 2 - i - 1 / 2) * spacing.x;
        }
        for (int i = columns / 2; i < columns; i++)
        {
            if (i - columns / 2 > 0)
            {
                gridColumns[i] = gridX + (i - columns / 2 - 1 / 2) * cellSize.x + (i - columns / 2 - 1 / 2) * spacing.x;
            }
            else
            {
                gridColumns[i] = gridX;
            }
        }

    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }



    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        Debug.Log(Input.mousePosition);
        transform.position = Input.mousePosition;
        lastPosition = Input.mousePosition;
        //Debug.Log(eventData.pointerEnter.name);

        piece = eventData.pointerEnter.name;

        float gridX = gridLayoutGroup.transform.position.x;
        float gridY = gridLayoutGroup.transform.position.y;

        Debug.Log("Position of cell : (" + gridX + ", " + gridY + ")");


        if (hasObject == false && eventData.pointerEnter.name == "z-shape")
        {
            //gridPoint = new Vector3(gridX, gridY, 0);
            startPosition = new Vector3(gridX - 170, gridY, 0);
            hasObject = true;

        }

        if (hasObject == false && eventData.pointerEnter.name == "inverted-r-shape")
        {
            //float posX = gridX + spacing.x + cellSize.x / 2;
            //float posY = gridY - spacing.y / 2 - cellSize.y / 2;
            //gridPoint = new Vector3(posX, posY, 0);
            startPosition = new Vector3(gridX - 170, gridY - 20, 0);
            hasObject = true;

        }

        if (hasObject == false && eventData.pointerEnter.name == "new-shape")
        {
            //gridPoint = new Vector3(gridX, gridY, 0);
            startPosition = new Vector3(gridX - 350, gridY - 20, 0);
            hasObject = true;

        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        Debug.Log(occupiedBy);
        Debug.Log(isOccupied);
        Debug.Log(piece);

        hasObject = false;
        transform.SetParent(parentAfterDrag);

        int tr=-1;
        int tc=-1;

        foreach (var row in gridRows)
        {
            if(row.Value<lastPosition.y && lastPosition.y - row.Value <= 30)
            {
                tr = row.Key;
            }
        }

        foreach (var col in gridColumns)
        {
            if (col.Value < lastPosition.x && lastPosition.x - col.Value <= 30)
            {
                tc = col.Key;
            }
        }


        if (tr > 0 && tc > 0 &&
            tr - 1 >= 0 && tr - 1 < rows && tr >= 0 && tr < rows &&
            tc >= 0 && tc < columns && tc + 1 >= 0 && tc + 1 < columns &&
            occupied[tr - 1,tc]==false && !occupied[tr - 1,tc + 1] && !occupied[tr,tc + 1])
        {
            transform.position = new Vector3((gridRows[tr]+gridRows[tr-1])/2, (gridColumns[tc] + gridColumns[tc + 1]) / 2, 0);
            occupied[tr - 1, tc] = true;
            occupied[tr - 1, tc + 1] = true;
            occupied[tr, tc + 1] = true;

        }
        else
        {
            transform.position = startPosition;

            if (eventData.pointerEnter.name == "inverted-r-shape")
            {
                if (occupiedList.Contains("inverted-r-shape"))
                {
                    occupiedList.Remove("inverted-r-shape");
                }
            }
        }


        if (occupiedList.Count < 1)
        {
            deliver.interactable = false;
        }
        else
        {
            deliver.interactable = true;
        }





        //if (isOccupied == false && Vector3.Distance(gridPoint, lastPosition) < snapSensivity)
        //{
        //    transform.position = gridPoint;

        //    isOccupied = true;
        //    occupiedBy = eventData.pointerEnter.name;

        //    deliver.interactable = true;
        //}
        //else
        //{
        //    transform.position = startPosition;
        //    if (piece == occupiedBy)
        //    {
        //        isOccupied = false;
        //        occupiedBy = "";

        //        deliver.interactable = false;
        //    }
        //}
    }
}
