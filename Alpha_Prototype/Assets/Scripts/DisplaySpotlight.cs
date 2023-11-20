using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySpotlight : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject spotlight;
    public GameObject dark_bckgrnd;
    void Start()
    {
        //spotlight = transform.Find("spotlight").gameObject;
       // dark_bckgrnd = transform.Find("dark_bg").gameObject;
        spotlight.SetActive(true);
        dark_bckgrnd.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the screen point where the user clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits a collider
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the collider belongs to the clicked object
                if (hit.collider.gameObject == gameObject)
                {
                    // Make the object disappear
                    spotlight.SetActive(false);
                    dark_bckgrnd.SetActive(false);
                }
            }
        }
        
    }
}
