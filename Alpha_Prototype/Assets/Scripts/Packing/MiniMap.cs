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
        List<Vector2> tilesToBeColoured = SceneHandler.Instance.UsedTiles;
        Material material = Resources.Load("UsedTile", typeof(Material)) as Material;

        foreach (var tbc in tilesToBeColoured)
        {

            foreach (var tile in tiles)
            {
                if (!tile)
                    continue;
                Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                if (pos == tbc)
                {
                    Debug.Log("inside if" + tbc);
                    tile.GetComponent<Renderer>().material = material;

                }
            }
        }

        foreach (GameObject tile in tiles) {
            Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
            if (SceneHandler.Instance.deletedTiles.Contains(pos))
                Destroy(tile.gameObject);
        }
    }
}
