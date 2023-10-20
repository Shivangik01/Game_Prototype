using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

//add visibility parameter based on delivered scene


// hashmap with levels at start
// based on row column, check if the immediate left right up wtv is free
// place there

public class DynamicDragging : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    SceneHandler sceneHandlerScript = new SceneHandler();

    public Button deliver;

    private bool hasObject = false;

    Transform parentAfterDrag;

    private Dictionary<int, float> gridRows;
    private Dictionary<int, float> gridColumns;
    public GridLayoutGroup gridLayoutGroup;

    static List<List<char>> notAllowed = new List<List<char>> {};

    Dictionary<string, List<float>> pairs = new Dictionary<string, List<float>>();


    public int rows;
    public int columns;

    List<Sprite> occupiedList = new List<Sprite> {};

    public Vector3 lastPosition;
    private Vector3 startPosition;
    public string piece;

    public static bool isOccupied = false;

    List<List<char>> toPass = new List<List<char>> { };





    public void Start()
    {
        deliver.interactable = false;

        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;
        float gridX = gridLayoutGroup.transform.position.x;
        float gridY = gridLayoutGroup.transform.position.y;

        gridRows = new Dictionary<int, float>();
        gridColumns = new Dictionary<int, float>();

        // populate rows and columns
        // even split and give; odd center is same

        Debug.Log(gridX);
        Debug.Log(gridY);

        float colSpace;
        float rowSpace;

        float rowSize;
        float colSize;

        if (rows % 2 == 0)
        {
            rowSpace = 0.5f;
        }
        else
        {
            rowSpace = 0;
        }

        if (columns % 2 == 0)
        {
            colSpace = 0.5f;
        }
        else
        {
            colSpace = 0f;
        }


        if (rows % 2 == 0)
        {
            rowSize = 0.5f;
        }
        else
        {
            rowSize = 0;
        }

        if (columns % 2 == 0)
        {
            colSize = 0.5f;
        }
        else
        {
            colSize = 0f;
        }

        for (int i = 0; i <rows/2; i++)
        {
            gridRows[i] = gridY - (((float)(rows / 2 - i - rowSize))*cellSize.y + ((float)(rows / 2 - i - rowSpace)) * spacing.y);
 
            Debug.Log(i + "r" + gridRows[i]);

        }
        for (int i = rows / 2; i < rows; i++)
        {
            if (i - rows / 2 > 0 || rows % 2 == 0)
            {
                gridRows[i] = gridY + ((float)(i - rowSize - 1)) * cellSize.x + ((float)(i - 1 - rowSpace)) * spacing.x;

                //Debug.Log(((float)(i - rows / 2 - 0.5f)));
                //Debug.Log(((float)(i - rows / 2 - 0.5f))*cellSize.y);
                //Debug.Log(((float)(i - rows / 2 - rowSize)) * cellSize.x + ((float)(i - rows / 2 - rowSpace)) * spacing.x);
            }
            else
            {
                gridRows[i] = gridY;
            }
            Debug.Log(i +"r"+gridRows[i]);
        }

        for (int i = 0; i < columns / 2; i++)
        {
            gridColumns[i] = gridX - (((float)(columns / 2 - i - colSize)) * cellSize.x + ((float)(columns / 2 - i - colSpace)) * spacing.x);
            
            Debug.Log(i + "c" + gridColumns[i]);
        }
        for (int i = columns / 2; i < columns; i++)
        {
            if (i - columns / 2 > 0 || columns%2==0)
            {
                gridColumns[i] = gridX + ((float)(i - columns / 2 - colSize)) * cellSize.x + ((float)(i - columns / 2 - colSpace)) * spacing.x;
            }
            else
            {
                gridColumns[i] = gridX;
            }
            Debug.Log(i+"c"+gridColumns[i]);
        }


        // Display the combined list of values
        foreach (var x in gridColumns)
        {
            foreach (var y in gridRows)
            {
                Debug.Log(x.Key);
                Debug.Log(y.Key);
                pairs[(x.Key).ToString() + (y.Key).ToString()] = new List<float> { x.Value, y.Value };

            }
        }


    }

    public void Update()
    {
        //Debug.Log(Input.mousePosition);
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

        Debug.Log(gridX);
        Debug.Log(gridY);

        if (hasObject == false && eventData.pointerEnter.name == "z-shape")
        {
            startPosition = new Vector3(gridX - 170, gridY, 0);
            hasObject = true;

        }

        if (hasObject == false && eventData.pointerEnter.name == "inverted-r-shape")
        {

            startPosition = new Vector3(gridX - 170, gridY - 20, 0);
            hasObject = true;

        }

        if (hasObject == false && eventData.pointerEnter.name == "new-shape")
        {
            startPosition = new Vector3(gridX - 350, gridY - 20, 0);
            hasObject = true;

        }
    }




    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");

        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;
        hasObject = false;
        transform.SetParent(parentAfterDrag);

        List<List<float>> objectPoints = new List<List<float>> { };

        if (eventData.pointerEnter.name == "inverted-r-shape")
        {
            objectPoints.Add(new List<float>{Input.mousePosition.x - cellSize.y / 2 , Input.mousePosition.y + cellSize.x / 2 });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y + cellSize.x / 2 });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y - cellSize.x / 2 });

        }

        if (eventData.pointerEnter.name == "new-shape")
        {
            objectPoints.Add(new List<float> { Input.mousePosition.x, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y , Input.mousePosition.y + cellSize.x  });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y , Input.mousePosition.y});
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y , Input.mousePosition.y});
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y , Input.mousePosition.y - cellSize.x  });


        }


        int i = 0;
        List<float> placement = new List<float> {};
        List<int> fits = new List<int> {};



        foreach (var p1 in objectPoints)
        {
            fits.Add(0);
            foreach (var p2 in pairs)
            {
                //check not allowed boxes
                if(Vector2.Distance(new Vector2(p1[0], p1[1]), new Vector2(p2.Value[0], p2.Value[1])) <= 22 && notAllowed.Contains(new List<char> {p2.Key[0],p2.Key[1]})==false)
                {
                    
                    fits[i] = 1;
                    if (i == 0)
                    {
                        placement.Add(p2.Value[0]);
                        placement.Add(p2.Value[1]);
                    }



                    toPass.Add(new List<char> { p2.Key[0], p2.Key[1] });
                    Debug.Log(p2.Key[0] + ',' + p2.Key[1]);
                    
                    break;
                }
            }

            i += 1;

        }

        if (fits.All(item => item == 1))
        {
            if (eventData.pointerEnter.name == "inverted-r-shape")
            {
                transform.position = new Vector3(placement[0] + cellSize.y / 2 + spacing.y / 2, placement[1] - cellSize.x / 2 - spacing.x / 2, 0);          
            }
            else if(eventData.pointerEnter.name == "new-shape")
            {
                transform.position = new Vector3(placement[0], placement[1], 0);
            }

            occupiedList.Add(transform.GetComponent<Image>().sprite);


            foreach(var val in toPass)
            {
                notAllowed.Add(new List<char> { val[0], val[1] });
            }

            Debug.Log(notAllowed.Count);
            toPass = new List<List<char>> { };

            foreach (var s in notAllowed)
            {
                Debug.Log(s[0] + ',' + s[1]);
            }


        }
        
        else
        {
            transform.position = startPosition;

             if (occupiedList.Contains(transform.GetComponent<Image>().sprite))
              {
                occupiedList.Remove(transform.GetComponent<Image>().sprite);
                foreach (var val in toPass)
                {
                    if(notAllowed.Contains(new List<char> { val[0], val[1] }))
                    notAllowed.Remove(new List<char> { val[0], val[1] });
                }
            }
            
        }

        foreach(var s in occupiedList)
        {
            
        }
        //sceneHandlerScript.stackedPackages.Add(var);





        if (occupiedList.Count < 1)
        {
            deliver.interactable = false;
        }
        else
        {
            deliver.interactable = true;
        }

    }
}
