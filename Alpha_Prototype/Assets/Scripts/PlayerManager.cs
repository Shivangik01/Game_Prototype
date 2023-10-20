using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    private Image imageComponent;
    public Vector2 deliverPosition;

    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneHandler.Instance.Delivered.Contains(deliverPosition))
        {
            Debug.Log("deliveerfd");
            imageComponent.enabled=false;
        }
        if (SceneHandler.Instance.StackedItems.Contains(deliverPosition))
        {
            Debug.Log("StackedIetm");
            imageComponent.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 getDeliveryPosition()
    {
        return deliverPosition;
    }

}
