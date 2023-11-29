using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    List<GameObject> tiles;

   

    public void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tiles").ToList();
        if (SceneHandler.Instance.showDeletion)
        {
            StartCoroutine(DestroyTiles());
        }
        else
        {
            List<Vector2> tilesToBeDestroyed = SceneHandler.Instance.deletedTiles;
           
         
            foreach (Vector2 tbd in tilesToBeDestroyed)
            {  
                foreach (var tile in tiles)
                {
                    Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                    if (pos == tbd)
                        Destroy(tile.gameObject);
                }

            }
            
        }
    }

    IEnumerator DestroyTiles()
    {
        List<Vector2> tilesToBeDestroyed = SceneHandler.Instance.deletedTiles;
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

        foreach (Vector2 tbd in tilesToBeDestroyed)
        {
            foreach (var tile in tiles)
            {
                if (!tile)
                    continue;

                
                Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                if (pos == tbd)
                {
                    tile.gameObject.layer = LayerMask.GetMask("Ignore Raycast");
                }
            }

        }

        foreach (Vector2 tbd in tilesToBeDestroyed)
        {
            foreach (var tile in tiles)
            {
                if (!tile)
                    continue;

                Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                if (pos == tbd)
                {
                    StartCoroutine(DropTile(tile.transform));
                    yield return new WaitForSeconds(0.5f);
                }
            }

        }


    }

    IEnumerator DropTile(Transform tile)
    {
        tile.gameObject.layer = LayerMask.GetMask("Ignore Raycast");
        Vector3 Gotoposition = new Vector3(tile.position.x, tile.position.y - 10.0f, tile.position.z);
        float elapsedTime = 0;
        float waitTime = 5;
        Vector3 currentPos = tile.position;
        while (elapsedTime < waitTime)
        {
            tile.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(tile.gameObject);
    }

}
