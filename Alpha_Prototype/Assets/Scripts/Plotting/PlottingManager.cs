using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct Combinations
{
    public Vector2 start, end;

    public float rotation;
    public Sprite sprite;
}
public class PlottingManager : MonoBehaviour
{
    public static PlottingManager Instance;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
        }
    }

    public List<Combinations> dictionary;
    
    bool isConnected = false;
    List<Vector2> Path;
    List<SpriteRenderer> TileSprites;


    [Header("Path Start/End")]
    public GameObject StartPoint;
    public GameObject EndPoint;

    [Header("Delivery Targets")]
    public Transform DeliveryTilesParent;
    List<GameObject> DeliveryTiles;

    Dictionary<Transform, List<Vector2>> Deliverables;

    [Header("Path Raycast Settings")]
    public SpriteRenderer CursorIndicator;
    public LayerMask PlottingLayerMask;
    public LayerMask DeliveryLayerMask;

    int startOffset = 0;
    int endOffset = 0;

    private void Start()
    {
        Path = new List<Vector2>
        {
            new Vector2(StartPoint.transform.position.x, StartPoint.transform.position.z),
            new Vector2(EndPoint.transform.position.x, EndPoint.transform.position.z)
        };

        TileSprites = new List<SpriteRenderer>
        {
            StartPoint.GetComponentInChildren<SpriteRenderer>(),
            EndPoint.GetComponentInChildren<SpriteRenderer>()
        };

        DeliveryTiles = new List<GameObject>();
        foreach(Transform child in DeliveryTilesParent)
        {
            if(child.gameObject.CompareTag("Targets"))
                DeliveryTiles.Add(child.gameObject);
        }

        Deliverables = new Dictionary<Transform, List<Vector2>>();

        updateSprites();
    }

    private List<Vector2> getNeighbours(int index)
    {
        List<Vector2> list = new List<Vector2>
        {
            new Vector2(Path[index].x + 1, Path[index].y),
            new Vector2(Path[index].x - 1, Path[index].y),
            new Vector2(Path[index].x, Path[index].y + 1),
            new Vector2(Path[index].x, Path[index].y - 1)
        };

        return list;
    }
    private void updateSprites()
    {
        Vector2 start, end;
        Vector2 prev = Path[0];
        prev.x -= 1;

        for(int i=0; i < startOffset; i++)
        {
            start = prev - Path[i];
            end = Path[i + 1] - Path[i];
            foreach(var entry in dictionary)
            {
                if((entry.start == start && entry.end == end) || (entry.start == end && entry.end == start))
                {
                    TileSprites[i].sprite = entry.sprite;
                    TileSprites[i].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                    break;
                }
            }

            prev = Path[i];
        }

        start = prev - Path[startOffset];
        
        if (!isConnected)
            end = start * -1;
        else
            end = Path[startOffset + 1] - Path[startOffset];

        foreach (var entry in dictionary)
        {
            if ((entry.start == start && entry.end == end) || (entry.start == end && entry.end == start))
            {
                TileSprites[startOffset].sprite = entry.sprite;
                TileSprites[startOffset].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                break;
            }
        }

        prev = Path[Path.Count - 1];
        prev.x += 1;

        for(int i = Path.Count-1; i > Path.Count - 1 - endOffset; i--)
        {
            start = prev - Path[i];
            end = Path[i - 1] - Path[i];

            foreach (var entry in dictionary)
            {
                if ((entry.start == start && entry.end == end) || (entry.start == end && entry.end == start))
                {
                    TileSprites[i].sprite = entry.sprite;
                    TileSprites[i].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                    break;
                }
            }

            prev = Path[i];
        }

        start = prev - Path[Path.Count - 1 - endOffset];
        if (!isConnected)
            end = start * -1;
        else
            end = Path[Path.Count - 1 - endOffset - 1] - Path[Path.Count - 1 - endOffset];
        foreach (var entry in dictionary)
        {
            if ((entry.start == start && entry.end == end) || (entry.start == end && entry.end == start))
            {
                TileSprites[Path.Count - 1 - endOffset].sprite = entry.sprite;
                TileSprites[Path.Count - 1 - endOffset].transform.rotation = Quaternion.Euler(90.0f, entry.rotation, 0);
                break;
            }
        }

    }

    private void AddPoint(int index, Vector2 position, SpriteRenderer renderer)
    {
        foreach (GameObject d in DeliveryTiles)
        {
            Vector2 pos = new Vector2(d.transform.position.x, d.transform.position.z);
            if (new Vector2(position.x + 1, position.y) == pos)
            {
                if (Deliverables.ContainsKey(d.transform))
                    Deliverables[d.transform].Add(position);
                else
                {
                    Deliverables[d.transform] = new List<Vector2>
                    {
                        position
                    };
                    d.transform.position = new Vector3(d.transform.position.x, 0.2f, d.transform.position.z);
                }
            }
            else if (new Vector2(position.x - 1, position.y) == pos)
            {
                if (Deliverables.ContainsKey(d.transform))
                    Deliverables[d.transform].Add(position);
                else
                {
                    Deliverables[d.transform] = new List<Vector2>
                    {
                        position
                    };
                    d.transform.position = new Vector3(d.transform.position.x, 0.2f, d.transform.position.z);
                }
            }
            else if (new Vector2(position.x, position.y + 1) == pos)
            {
                if (Deliverables.ContainsKey(d.transform))
                    Deliverables[d.transform].Add(position);
                else
                {
                    Deliverables[d.transform] = new List<Vector2>
                    {
                        position
                    };
                    d.transform.position = new Vector3(d.transform.position.x, 0.2f, d.transform.position.z);
                }
            }
            else if (new Vector2(position.x, position.y - 1) == pos)
            {
                if (Deliverables.ContainsKey(d.transform))
                    Deliverables[d.transform].Insert(Math.Min(3, Deliverables[d.transform].Count - 1), position);
                else
                {
                    Deliverables[d.transform] = new List<Vector2>
                    {
                        position
                    };
                    d.transform.position = new Vector3(d.transform.position.x, 0.2f, d.transform.position.z);
                }
            }
        }

        Path.Insert(index, position);
        TileSprites.Insert(index, renderer);
    }

    private void RemovePoint(int index)
    {
        List<Transform> removals = new List<Transform>();
        foreach(var entry in Deliverables)
        {
            if (entry.Value.Contains(Path[index]))
            {
                entry.Value.Remove(Path[index]);
                if (entry.Value.Count == 0)
                {
                    entry.Key.position = new Vector3(entry.Key.position.x, 0, entry.Key.position.z);
                    removals.Add(entry.Key);
                }
            }
        }
        foreach(var r in removals)
            Deliverables.Remove(r);

        Path.RemoveAt(index);
        TileSprites[index].sprite = null;
        TileSprites.RemoveAt(index);
    }

    private void debugPrintDeliveries()
    {
        foreach(var entry in Deliverables)
        {
            string printing = "";
            printing += entry.Key.name;
            printing += ": ";
            foreach(var p in entry.Value)
            {
                printing += "(" + p.x.ToString() + "," + p.y.ToString() + "), ";
            }
            Debug.Log(printing);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            debugPrintDeliveries();

        CursorIndicator.gameObject.SetActive(false);
        Ray cursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(cursor, out hit, 1000.0f, PlottingLayerMask))
        {
            if (!isConnected)
                CursorIndicator.gameObject.SetActive(true);
            else
                goto skipTileModifier;

            GameObject tile = hit.transform.gameObject;
            Vector2 tile_position = new Vector2(tile.transform.position.x, tile.transform.position.z);

            CursorIndicator.transform.position = new Vector3(tile_position.x, CursorIndicator.transform.position.y, tile_position.y);
            List<Vector2> start_neighbours = getNeighbours(startOffset);
            List<Vector2> end_neighbours = getNeighbours(Path.Count - 1 - endOffset);

            if(start_neighbours.Contains(tile_position) || end_neighbours.Contains(tile_position))
            {
                CursorIndicator.color = Color.green;
            }
            else if (Path[startOffset] == tile_position || Path[Path.Count - 1 - endOffset] == tile_position)
            {
                CursorIndicator.color = Color.green;
                goto skipTileModifier;
            }
            else
            {
                CursorIndicator.color = Color.red;
                goto skipTileModifier;
            }

            if(Input.GetMouseButton(0))
            {
                if(Path.Contains(tile_position))
                {
                    // may require to delete
                    if(startOffset-1 >= 0 && Path[startOffset-1] == tile_position)
                    {
                        RemovePoint(startOffset);
                        startOffset--;
                        updateSprites();
                    }
                    else if((Path.Count - 1 -(endOffset-1)) < Path.Count && Path[(Path.Count - 1 - (endOffset - 1))] == tile_position)
                    {
                        int index = Path.Count - 1 - endOffset;
                        RemovePoint(index);
                        endOffset--;
                        updateSprites();
                    }
                    // maybe invalid
                    else
                    {
                        CursorIndicator.color = Color.red;
                    }
                }
                else
                {
                    if(start_neighbours.Contains(tile_position))
                    {
                        AddPoint(startOffset + 1, tile_position, tile.GetComponentInChildren<SpriteRenderer>());
                        startOffset++;
                    }
                    else
                    {
                        int index = Path.Count - 1 - (endOffset);
                        AddPoint(index, tile_position, tile.GetComponentInChildren<SpriteRenderer>());                        
                        endOffset++;
                    }

                    if (start_neighbours.Contains(tile_position) && end_neighbours.Contains(tile_position))
                        isConnected = true;

                    updateSprites();
                }
            }
        }
        skipTileModifier:
        if (Physics.Raycast(cursor, out hit, 1000.0f, DeliveryLayerMask))
        {
            if(Input.GetMouseButtonDown(0))
                hit.transform.GetComponent<Animator>().SetTrigger("Squash");
        }
    }

    public void ResetPaths()
    {
        foreach(SpriteRenderer sr in TileSprites)
        {
            sr.sprite = null;
        }

        Path.Clear();
        Path.Add(new Vector2(StartPoint.transform.position.x, StartPoint.transform.position.z));
        Path.Add(new Vector2(EndPoint.transform.position.x, EndPoint.transform.position.z));

        TileSprites.Clear();
        TileSprites.Add(StartPoint.GetComponentInChildren<SpriteRenderer>());
        TileSprites.Add(EndPoint.GetComponentInChildren<SpriteRenderer>());

        startOffset = 0;
        endOffset = 0;
        isConnected = false;

        foreach (var entry in Deliverables)
        {
            entry.Key.position = new Vector3(entry.Key.position.x, 0.0f, entry.Key.position.z);
        }

        Deliverables.Clear();

        updateSprites();
    }

    public void getPathRaw(out int startIndex, out int endIndex, out bool connected)
    {
        startIndex = startOffset;
        endIndex = endOffset;
        connected = isConnected;
    }

    public void getOpenPoints(out Vector2 startPoint, out Vector2 endPoint)
    {
        startPoint = Path[startOffset];
        endPoint = Path[Path.Count - 1 - endOffset];
    }

    public List<Vector2> getTraversalPath()
    {
        List<Vector2> list = new List<Vector2>();

        int end = startOffset + 1;
        if (isConnected)
            end = Path.Count;

        for(int i=0; i<end; i++)
            list.Add(Path[i]);

        return list;
    }
}
