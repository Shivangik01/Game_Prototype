using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Button deliver;
    private Vector3 gridPoint;
    public Vector3 lastPosition;
    private Vector3 startPosition;
    public string piece;

    public GridLayoutGroup gridLayoutGroup;

    private bool hasObject = false;
    public static bool isOccupied = false;

    private static string occupiedBy;

    private float snapSensivity = 50.0f;

    Transform parentAfterDrag;

    public void Start()
    {
        deliver.interactable = false;
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

        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;
        float gridX = gridLayoutGroup.transform.position.x;
        float gridY = gridLayoutGroup.transform.position.y;

        Debug.Log("Position of cell : (" + gridX + ", " + gridY + ")");

        float posX = gridX + spacing.x + cellSize.x/2;
        float posY = gridY - spacing.y/2 - cellSize.y/2;


        if (hasObject == false && eventData.pointerEnter.name == "z-shape")
        {

            gridPoint = new Vector3(gridX, gridY, 0);
            startPosition = new Vector3(gridX-170, gridY, 0);
            hasObject = true;

        }

        if (hasObject==false && eventData.pointerEnter.name == "inverted-r-shape")
        {
            gridPoint = new Vector3(posX, posY, 0);
            startPosition = new Vector3(gridX - 170, gridY-20, 0);
            hasObject = true;

        }

        if (hasObject == false && eventData.pointerEnter.name == "new-shape")
        {
            gridPoint = new Vector3(gridX, gridY, 0);
            startPosition = new Vector3(gridX - 350, gridY-20, 0);
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

        if (isOccupied==false && Vector3.Distance(gridPoint, lastPosition) < snapSensivity)
        {
            transform.position = gridPoint;

            isOccupied = true;
            occupiedBy = eventData.pointerEnter.name;

            deliver.interactable = true;
        }
        else
        {
            transform.position = startPosition;
            if (piece == occupiedBy)
            {
                isOccupied = false;
                occupiedBy = "";

                deliver.interactable = false;
            }
        }
    }
}
        