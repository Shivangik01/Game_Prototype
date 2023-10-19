using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float horizontalInp;
    public float verticalInp;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInp = Input.GetAxis("Horizontal");
        verticalInp = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * verticalInp * 10);
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInp * 10);
    }
}
