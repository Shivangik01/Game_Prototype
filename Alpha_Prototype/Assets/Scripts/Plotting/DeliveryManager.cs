using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public GameObject DemandBox;
    public SpriteRenderer PackageSpriteRenderer;
    public Sprite Package;
    public GameObject deliveryLocation;

    public Animator playerAnimator;

    public bool delivered;

    private void Start()
    {
        delivered = false;
        PackageSpriteRenderer.sprite = Package;
    }

    private void Update()
    {
        if (!delivered)
        {
            float xRot = Camera.main.transform.rotation.eulerAngles.x;
            DemandBox.transform.rotation = Quaternion.Euler(xRot, 0, 0);
        }
    }

    public void makeDelivery()
    {
        audioManager.instance.playHappy();
        playerAnimator.SetBool("Happy", true);
        delivered = true;
        DemandBox.SetActive(false);
    }

    public void resetDelivery()
    {
        playerAnimator.SetBool("Happy", false);
        playerAnimator.SetBool("Sad", false);
        playerAnimator.Rebind();
        playerAnimator.Update(0f);
        delivered = false;
        DemandBox.SetActive(true);
    }

    public void makeSad()
    {
        audioManager.instance.playSad();
        playerAnimator.SetBool("Sad", true);
        delivered = true;
        DemandBox.SetActive(false);
    }
}