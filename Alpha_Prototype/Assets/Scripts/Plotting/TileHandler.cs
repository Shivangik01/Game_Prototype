using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct arrangement
{
    public List<Vector2> points;
    public float rotation;
    public Sprite sprite;
};

public class TileHandler : MonoBehaviour
{
    public static TileHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    List<GameObject> tiles;
    public List<arrangement> dictionary;

    Vector2Int startAdder;
    Vector2Int endAdder;

    public void Initiate()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tiles").ToList();

        ColorDirtTiles(tiles);

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

    public void ColorDirtTiles(List<GameObject> Tiles)
    {
        startAdder = Vector2Int.zero;
        if (PlottingManager.Instance)
        {
            if (PlottingManager.Instance.StartPoint.transform.position.x == SceneHandler.Instance.Min.x)
                startAdder.x = -1;
            else if (PlottingManager.Instance.StartPoint.transform.position.x == SceneHandler.Instance.Max.x)
                startAdder.x = 1;
            else if (PlottingManager.Instance.StartPoint.transform.position.z == SceneHandler.Instance.Min.y)
                startAdder.y = -1;
            else if (PlottingManager.Instance.StartPoint.transform.position.z == SceneHandler.Instance.Max.y)
                startAdder.y = 1;
        }
        else
        {
            if (FindObjectOfType<MiniMap>().StartPoint.transform.position.x == SceneHandler.Instance.Min.x)
                startAdder.x = -1;
            else if (FindObjectOfType<MiniMap>().StartPoint.transform.position.x == SceneHandler.Instance.Max.x)
                startAdder.x = 1;
            else if (FindObjectOfType<MiniMap>().StartPoint.transform.position.z == SceneHandler.Instance.Min.y)
                startAdder.y = -1;
            else if (FindObjectOfType<MiniMap>().StartPoint.transform.position.z == SceneHandler.Instance.Max.y)
                startAdder.y = 1;
        }

        endAdder = Vector2Int.zero;
        if (PlottingManager.Instance)
        {
            if (PlottingManager.Instance.EndPoint.transform.position.x == SceneHandler.Instance.Min.x)
                endAdder.x = -1;
            else if (PlottingManager.Instance.EndPoint.transform.position.x == SceneHandler.Instance.Max.x)
                endAdder.x = 1;
            else if (PlottingManager.Instance.EndPoint.transform.position.z == SceneHandler.Instance.Min.y)
                endAdder.y = -1;
            else if (PlottingManager.Instance.EndPoint.transform.position.z == SceneHandler.Instance.Max.y)
                endAdder.y = 1;
        }
        else
        {
            if (FindObjectOfType<MiniMap>().EndPoint.transform.position.x == SceneHandler.Instance.Min.x)
                endAdder.x = -1;
            else if (FindObjectOfType<MiniMap>().EndPoint.transform.position.x == SceneHandler.Instance.Max.x)
                endAdder.x = 1;
            else if (FindObjectOfType<MiniMap>().EndPoint.transform.position.z == SceneHandler.Instance.Min.y)
                endAdder.y = -1;
            else if (FindObjectOfType<MiniMap>().EndPoint.transform.position.z == SceneHandler.Instance.Max.y)
                endAdder.y = 1;
        }
        List<List<Vector2>> pathsToBeColoured = SceneHandler.Instance.UsedTiles;

        foreach (var path in pathsToBeColoured)
        {
            Vector2 start, end;
            Vector2 prev = path[0];
            prev.x += startAdder.x;
            prev.y += startAdder.y;

            for (int i=0; i< path.Count-1; i++)
            {
                start = prev - path[i];
                end = path[i + 1] - path[i];

                foreach (var tile in Tiles)
                {
                    if (!tile)
                        continue;
                    Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                    if (pos == path[i])
                    {
                        List<Vector2> tilePoints = new List<Vector2>{ start, end };
                        if(tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite)
                        {
                            foreach (var entry in dictionary)
                            {
                                if(entry.sprite == tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite)
                                {
                                    if(entry.rotation == tile.GetComponentsInChildren<SpriteRenderer>()[1].transform.rotation.eulerAngles.y)
                                    {
                                        foreach (var p in entry.points)
                                        {
                                            if(!tilePoints.Contains(p))
                                                tilePoints.Add(p);
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        foreach (var entry in dictionary)
                        {
                            int pointCount = 0;
                            foreach(var p in entry.points)
                                if(tilePoints.Contains(p))
                                    pointCount++;
                            
                            if(pointCount == entry.points.Count && tilePoints.Count == entry.points.Count)
                            {
                                tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite = entry.sprite;
                                tile.GetComponentsInChildren<SpriteRenderer>()[1].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                                break;
                            }
                        }
                    }
                }

                prev = path[i];
            }
            prev = path[path.Count - 1];
            prev.x += endAdder.x;
            prev.y += endAdder.y;

            start = prev - path[path.Count - 1];
            end = path[path.Count - 2] - path[path.Count - 1];
            foreach (var tile in Tiles)
            {
                if (!tile)
                    continue;
                Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.z);
                if (pos == path[path.Count - 1])
                {
                    List<Vector2> tilePoints = new List<Vector2> { start, end };
                    if (tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite)
                    {
                        foreach (var entry in dictionary)
                        {
                            if (entry.sprite == tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite)
                            {
                                if (entry.rotation == tile.GetComponentsInChildren<SpriteRenderer>()[1].transform.rotation.eulerAngles.y)
                                {
                                    foreach (var p in entry.points)
                                    {
                                        if (!tilePoints.Contains(p))
                                            tilePoints.Add(p);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var entry in dictionary)
                    {
                        int pointCount = 0;
                        foreach (var p in entry.points)
                            if (tilePoints.Contains(p))
                                pointCount++;

                        if (pointCount == entry.points.Count)
                        {
                            tile.GetComponentsInChildren<SpriteRenderer>()[1].sprite = entry.sprite;
                            tile.GetComponentsInChildren<SpriteRenderer>()[1].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                            break;
                        }
                    }
                }
            }
        }
    }

    IEnumerator DestroyTiles()
    {
        List<Vector2> tilesToBeDestroyed = SceneHandler.Instance.deletedTiles;

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
