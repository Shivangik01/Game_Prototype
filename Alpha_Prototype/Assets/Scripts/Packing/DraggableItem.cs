using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//check stacked and delivered items

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Button deliver;
    public Vector3 lastPosition; // where to keep it last
    public Vector3 startPosition; //og position
    public Image imageOutline;

    public GridLayoutGroup gridLayoutGroup;

    private bool hasObject = false;

    public static List<Sprite> occupiedBy = new List<Sprite>();
    public List<Vector2> gridPoints = new List<Vector2> {};
    static List<int> notAllowed = new List<int> {};
    List<int> toPass = new List<int> { };

    Transform parentAfterDrag;

    public static DraggableItem Instance;

    public GameObject spotlight;

    public static Dictionary<Sprite, List<int>> positions = new Dictionary<Sprite, List<int>>();


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void Start()
    {
        deliver.interactable = false;
        spotlight.SetActive(false);
        
        if (SceneHandler.Instance.Delivered.Count>0)
        {
            PlayerManager[] scriptInstances = FindObjectsOfType<PlayerManager>();

            foreach (var scriptInstance in scriptInstances)
            {
                Image spriteRenderer = scriptInstance.GetComponent<Image>();
                if (SceneHandler.Instance.Delivered.Contains(scriptInstance.getDeliveryPosition()))
                {
                    occupiedBy.Add(spriteRenderer.sprite);
                }
            }
        }

        if (occupiedBy.Count < 1)
        {
            deliver.interactable = false;
            notAllowed = new List<int> { };
        }
        else
        {
            deliver.interactable = true;
            foreach (var val in notAllowed)
            {
                Debug.Log(val);
            }
        }


        

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");
        spotlight.SetActive(true);
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        if (occupiedBy.Contains(transform.GetComponent<Image>().sprite))
        {
            occupiedBy.Remove(transform.GetComponent<Image>().sprite);
            foreach (var val in positions[transform.GetComponent<Image>().sprite])
            {
                if (notAllowed.Contains(val))
                    notAllowed.Remove(val);
            }
            positions.Remove(transform.GetComponent<Image>().sprite);

        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        spotlight.SetActive(true);
        moveSpotlightToDeliveryPosition();

        //Debug.Log(Input.mousePosition);
        transform.position = Input.mousePosition;
        lastPosition = Input.mousePosition;

        if (hasObject == false)
        {            
            hasObject = true;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        spotlight.SetActive(false);
        gridPoints=getGridPoints();

        hasObject = false;
        transform.SetParent(parentAfterDrag);

        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

        List<List<float>> objectPoints = new List<List<float>> {};

        if (eventData.pointerEnter.name == "z-shape")
        {
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y / 2, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y / 2, Input.mousePosition.y + cellSize.x / 2 });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y - cellSize.x / 2 });

        }

        if (eventData.pointerEnter.name == "inverted-r-shape")
        {
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y / 2, Input.mousePosition.y + cellSize.x / 2 });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y + cellSize.x / 2 });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y / 2, Input.mousePosition.y - cellSize.x / 2 });

        }

        if (eventData.pointerEnter.name == "new-shape")
        {
            objectPoints.Add(new List<float> { Input.mousePosition.x, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y, Input.mousePosition.y + cellSize.x });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x - cellSize.y, Input.mousePosition.y });
            objectPoints.Add(new List<float> { Input.mousePosition.x + cellSize.y, Input.mousePosition.y - cellSize.x });


        }

        PlayerManager objectScript = transform.GetComponent<Image>().GetComponent<PlayerManager>();

        int i = 0;
        Vector2 placement = new Vector2(objectScript.startPosition.x, objectScript.startPosition.y);
        int fits = 0;
        toPass = new List<int> {};

        foreach (var p1 in objectPoints)
        {
            for(int j=0; j<gridPoints.Count;j++)
            {
                //check not allowed boxes
                if (Vector2.Distance(new Vector2(p1[0], p1[1]), gridPoints[j]) <= 22 && notAllowed.Contains(j) == false)
                {

                    fits += 1;
                    if (i == 0)
                    {
                        placement=gridPoints[j];
                        i += 1;
                    }

                    toPass.Add(j);
                    break;
                }
            }
        }

        if (fits==objectPoints.Count)
        {
            if (eventData.pointerEnter.name == "inverted-r-shape")
            {
                transform.position = new Vector3(placement.x + cellSize.y / 2 + spacing.y / 2, placement.y - cellSize.x / 2 - spacing.x / 2, 0);
            }
            if (eventData.pointerEnter.name == "z-shape")
            {
                transform.position = new Vector3(placement.x + cellSize.y / 2, placement.y, 0);
            }
            else if (eventData.pointerEnter.name == "new-shape")
            {
                transform.position = new Vector3(placement.x, placement.y, 0);
            }

            occupiedBy.Add(transform.GetComponent<Image>().sprite);
            //which cells occupied by the sprite
            positions.Add(transform.GetComponent<Image>().sprite, toPass);


            foreach (var val in toPass)
            {
                notAllowed.Add(val);
            }

            

        }

        else
        {
            
            transform.position = objectScript.startPosition;

            if (occupiedBy.Contains(transform.GetComponent<Image>().sprite))
            {
                occupiedBy.Remove(transform.GetComponent<Image>().sprite);
                foreach (var val in positions[transform.GetComponent<Image>().sprite])
                {
                    if (notAllowed.Contains(val))
                        notAllowed.Remove(val);
                }
                positions.Remove(transform.GetComponent<Image>().sprite);

            }

        }

        if (occupiedBy.Count < 1)
        {
            deliver.interactable = false;
        }
        else
        {
            deliver.interactable = true;
        }

    }

    public List<Vector2> getTiles()
    {
        List<Vector2> items = new List<Vector2> { };
        PlayerManager[] scriptInstances = FindObjectsOfType<PlayerManager>();

        foreach (var scriptInstance in scriptInstances)
        {
            Image spriteRenderer = scriptInstance.GetComponent<Image>();
            if (occupiedBy.Contains(spriteRenderer.sprite))
            {
                items.Add(scriptInstance.getDeliveryPosition());
            }
        }
            return items;
    }

    public Dictionary<Vector2,Vector2> getPositions()
    {
        Dictionary<Vector2,Vector2> packagePositions = new Dictionary<Vector2,Vector2> { };
        gridPoints = getGridPoints();
        Vector3 temp = new Vector3();
        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

        PlayerManager[] scriptInstances = FindObjectsOfType<PlayerManager>();

        foreach (var scriptInstance in scriptInstances)
        {
            Image spriteRenderer = scriptInstance.GetComponent<Image>();
            if (occupiedBy.Contains(spriteRenderer.sprite))
            {

                if (spriteRenderer.name == "inverted-r-shape")
                {
                    temp = new Vector3(gridPoints[positions[spriteRenderer.sprite][0]].x + cellSize.y / 2 + spacing.y / 2, gridPoints[positions[spriteRenderer.sprite][0]].y - cellSize.x / 2 - spacing.x / 2, 0);
                }
                if (spriteRenderer.name == "z-shape")
                {
                    temp = new Vector3(gridPoints[positions[spriteRenderer.sprite][0]].x + cellSize.y / 2, gridPoints[positions[spriteRenderer.sprite][0]].y, 0);
                }
                else if (spriteRenderer.name == "new-shape")
                {
                    temp = new Vector3(gridPoints[positions[spriteRenderer.sprite][0]].x, gridPoints[positions[spriteRenderer.sprite][0]].y, 0);
                }

                packagePositions.Add(scriptInstance.getDeliveryPosition(),temp);
                //Debug.Log(gridPoints[positions[spriteRenderer.sprite][0]]);
            }
        }

        return packagePositions;
    }

    public List<Vector2> getGridPoints()
    {
        float gridX = gridLayoutGroup.transform.position.x;
        float gridY = gridLayoutGroup.transform.position.y;
        List<Vector2> gp = new List<Vector2> {};


        for (int i = 0; i < gridLayoutGroup.transform.childCount; i++)
        {
            Transform childLayoutElement = gridLayoutGroup.transform.GetChild(i);

            // Get the child layout element's local position.
            Vector2 childPosition = childLayoutElement.localPosition;

            //Debug.Log(childPosition.x + gridX);
            //Debug.Log(gridY-childPosition.y);
            //Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(childPosition.x + gridX, gridY - childPosition.y, 0)));
            //gp.Add(Camera.main.ScreenToWorldPoint(new Vector3(childPosition.x + gridX, gridY - childPosition.y, 0)));
            gp.Add(new Vector3(childPosition.x + gridX, gridY - childPosition.y, 0));

        }

        return gp;
    }

    private void moveSpotlightToDeliveryPosition()
    {
        PlayerManager scriptInstance = GetComponent<PlayerManager>();
        if (scriptInstance != null)
        {
            Vector2 deliveryPosition = scriptInstance.getDeliveryPosition();
            spotlight.transform.position = new Vector3(deliveryPosition.x, 0, deliveryPosition.y-0.5f);
        }
    }

}
        