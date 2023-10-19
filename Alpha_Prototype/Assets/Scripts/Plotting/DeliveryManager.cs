using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public GameObject DemandBox;
    public SpriteRenderer PackageSpriteRenderer;
    public Sprite Package;

    public Animator playerAnimator;

    public bool delivered;

    private void Start()
    {
        delivered = false;
        PackageSpriteRenderer.sprite = Package;
    }

    private void Update()
    {
        if(!delivered)
            DemandBox.transform.LookAt(Camera.main.transform);
    }

    public void makeDelivery()
    {
        playerAnimator.SetBool("Happy", true);
        delivered = true;
        DemandBox.SetActive(false);
    }
}
