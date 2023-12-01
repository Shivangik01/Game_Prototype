using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackageTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalPosition;
    public Vector2 deliveryRepresentations;

    public List<Transform> offsets;

    public GameObject spotlight;
    public GameObject deliverText;

    void Start()
    {

        spotlight.SetActive(false);
        deliverText.SetActive(false);

        if (SceneHandler.Instance.Delivered.Contains(deliveryRepresentations))
        {
            Destroy(this.gameObject);
        }
        else if (SceneHandler.Instance.StackedItemsPositions.ContainsKey(deliveryRepresentations))
        {
            transform.position = SceneHandler.Instance.StackedItemsPositions[deliveryRepresentations];
            Initialize();
        }
    }

    public void Initialize()
    {
        Vector2 finalPos = transform.position;

        List<Vector2> checkers = new List<Vector2>();
        foreach (Transform transform_offset in offsets)
        {
            Vector2 off = transform_offset.position;
            checkers.Add(off);
        }

        //Debug.Log(GridManager.Instance.gameObject.name);
        if (GridManager.Instance.inGrid(checkers, out finalPos))
        {
            GridManager.Instance.markOccupied(checkers, true);
        }
    }

    Transform parentAfterDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        spotlight.SetActive(true);
        spotlight.transform.position = new Vector3(deliveryRepresentations.x, spotlight.transform.position.y, deliveryRepresentations.y);

        deliverText.SetActive(true);

        Vector2 finalPos = transform.position;

        List<Vector2> checkers = new List<Vector2>();
        foreach (Transform transform_offset in offsets)
        {
            Vector2 off = transform_offset.position;
            checkers.Add(off);
        }

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        if (GridManager.Instance.inGrid(checkers, out finalPos, true))
        {
            GridManager.Instance.markOccupied(checkers, false);
            SceneHandler.Instance.StackedItemsPositions.Remove(deliveryRepresentations);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        spotlight.SetActive(false);
        deliverText.SetActive(false);

        List<Vector2> checkers = new List<Vector2>();
        foreach (Transform transform_offset in offsets)
        {
            Vector2 off = transform_offset.position;
            checkers.Add(off);
        }

        Vector2 finalPos = checkers[0];
        Vector2 offset = new Vector2(transform.position.x, transform.position.y) - finalPos;
        if (GridManager.Instance.inGrid(checkers, out finalPos))
        {
            GridManager.Instance.markOccupied(checkers, true);
            transform.position = finalPos + offset;
            SceneHandler.Instance.StackedItemsPositions.Add(deliveryRepresentations, transform.position);
        }
        else
        {
            transform.position = originalPosition.position;
        }

        transform.SetParent(parentAfterDrag);
    }

    private void OnDrawGizmos()
    {
        foreach(var off in offsets)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(off.position, 1.0f);
        }    
    }
}
