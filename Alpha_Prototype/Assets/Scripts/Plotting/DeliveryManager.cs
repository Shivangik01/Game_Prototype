using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public GameObject DemandBox;
    public SpriteRenderer PackageSpriteRenderer;
    public Sprite Package;

    private void Start()
    {
        PackageSpriteRenderer.sprite = Package;
    }

    private void Update()
    {
        DemandBox.transform.LookAt(Camera.main.transform);
    }
}
