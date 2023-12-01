using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobberManager : MonoBehaviour
{
    public GameObject DemandBox;

    public GameObject deliveryLocation;

    public Animator playerAnimator;

    public bool delivered;

    public List<DeliveryManager> deliveries;

    public Transform DeliveryTilesParent;

    public Vector2 deliverPersonPosition = Vector2.negativeInfinity;

    private void Start()
    {
        delivered = false;
        foreach (Transform child in DeliveryTilesParent)
        {
            if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
            {
                deliveries.Add(child.gameObject.GetComponent<DeliveryManager>());
            }
        }


    }

    public void makeDelivery()
    {
        playerAnimator.SetBool("Happy", true);
        DemandBox.SetActive(false);
        delivered = true;
        foreach (var target in deliveries)
        {
            Vector2 pos = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.z);
            if (pos == deliverPersonPosition)
            {
                target.makeSad();
                break;
            }

        }
    }

    public void resetDelivery()
    {
        playerAnimator.SetBool("Happy", false);
        DemandBox.SetActive(false);
        playerAnimator.Rebind();
        playerAnimator.Update(0f);
        delivered = false;
        foreach (var target in deliveries)
        {
            Vector2 pos = new Vector2(target.gameObject.transform.position.x, target.gameObject.transform.position.z);
            if (pos == deliverPersonPosition)
            {
                target.resetDelivery();
                break;
            }

        }
        deliverPersonPosition = Vector2.negativeInfinity;
    }

    public void levelComplete()
    {
        DemandBox.SetActive(false);
    }
}