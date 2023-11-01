using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverSystem : MonoBehaviour
{
    public float time = 3.0f;
    public float Amplidute = 3.0f;
    public GameObject DeliveryObject;
    public Transform startPosition;

    List<GameObject> instantiated;

    private void Start()
    {
        instantiated = new List<GameObject>();
    }

    public void deliverObject(DeliveryManager deliveryTarget)
    {
        StartCoroutine(DeliverPackage(startPosition, deliveryTarget));
    }

    public void deliverObject(RobberManager deliveryTarget)
    {
        StartCoroutine(DeliverPackage(startPosition, deliveryTarget));
    }

    public void KillCoroutines()
    {
        StopAllCoroutines();
        foreach (GameObject obj in instantiated)
        {
            if(obj != null)
            {
                Destroy(obj);
            }
        }
        instantiated.Clear();
    }

    IEnumerator DeliverPackage(Transform A, DeliveryManager B)
    {
        Transform B_loc = B.deliveryLocation.transform;
        float TotalDistance = Vector3.Distance(A.position, B_loc.position);
        float speed = TotalDistance / time;
        GameObject package = Instantiate(DeliveryObject);
        instantiated.Add(package);
        package.transform.position = A.position;
        float startTime = Time.time;
        
        Vector3 A_pos = new Vector3(A.position.x, 0, A.position.z);
        Vector3 B_pos = new Vector3(B_loc.position.x, 0, B_loc.position.z);

        while (Vector3.Distance(package.transform.position, B_loc.position) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            //distCovered = Mathf.Clamp01(distCovered);
            float yOffset = Mathf.Sin(Mathf.PI * (distCovered / TotalDistance)) * Amplidute;
            
            package.transform.position = Vector3.Lerp(A_pos, B_pos, distCovered / TotalDistance);
            package.transform.position = new Vector3(package.transform.position.x, B_loc.position.y + yOffset, package.transform.position.z);

            yield return null;
        }
        package.transform.position = B_loc.position;
        B.makeDelivery();

        Destroy(package.gameObject, 2.0f);

        yield return null;
    }

    IEnumerator DeliverPackage(Transform A, RobberManager B)
    {
        Transform B_loc = B.deliveryLocation.transform;
        float TotalDistance = Vector3.Distance(A.position, B_loc.position);
        float speed = TotalDistance / time;
        GameObject package = Instantiate(DeliveryObject);
        instantiated.Add(package);
        package.transform.position = A.position;
        float startTime = Time.time;

        Vector3 A_pos = new Vector3(A.position.x, 0, A.position.z);
        Vector3 B_pos = new Vector3(B_loc.position.x, 0, B_loc.position.z);

        while (Vector3.Distance(package.transform.position, B_loc.position) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * speed;
            //distCovered = Mathf.Clamp01(distCovered);
            float yOffset = Mathf.Sin(Mathf.PI * (distCovered / TotalDistance)) * Amplidute;

            package.transform.position = Vector3.Lerp(A_pos, B_pos, distCovered / TotalDistance);
            package.transform.position = new Vector3(package.transform.position.x, B_loc.position.y + yOffset, package.transform.position.z);

            yield return null;
        }
        package.transform.position = B_loc.position;
        B.makeDelivery();

        Destroy(package.gameObject, 2.0f);

        yield return null;
    }

}
