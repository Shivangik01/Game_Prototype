using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartup : MonoBehaviour
{
    void Start()
    {
        if(SceneHandler.Instance != null)
        {
            GetComponent<Animator>().SetTrigger("force");
            Destroy(SceneHandler.Instance.gameObject);
        }
    }
}
