using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTripping : MonoBehaviour
{

    public float horizontalInp;
    public float verticalInp;
    void Start()
    {

    }

    void Update()
    {
        horizontalInp = Input.GetAxis("Horizontal");
        verticalInp = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * verticalInp * 10);
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInp * 10);
    }

}