using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
            {
                Vector2 pos = new Vector2(child.transform.position.x, child.transform.position.z);
                if (SceneHandler.Instance.Delivered.Contains(pos))
                {
                    List<Animator> anims = child.GetComponentsInChildren<Animator>().ToList();
                    foreach (Animator anim in anims)
                    {
                        if (anim.parameterCount == 2)
                        {
                            anim.SetBool("Happy", true);
                        }
                    }
                }
            }
        }

        List<GameObject> tiles = GameObject.FindGameObjectsWithTag("Tiles").ToList();
        foreach (GameObject tile in tiles) {
            Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
            if (SceneHandler.Instance.deletedTiles.Contains(pos))
                Destroy(tile.gameObject);
        }
    }
}
