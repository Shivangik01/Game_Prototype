using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    
    public bool isConnected = false;
    List<Vector2> Path;
    List<SpriteRenderer> TileSprites;


    [Header("Path Start/End")]
    public GameObject StartPoint;
    public GameObject EndPoint;
    public int farOffset;

    [Header("Delivery Targets")]
    public Transform DeliveryTilesParent;
    List<GameObject> DeliveryTiles;
    List<GameObject> RobberTiles;

    Dictionary<Transform, List<Vector2>> Deliverables;

    [Header("Path Raycast Settings")]
    public SpriteRenderer CursorIndicator;
    public LayerMask PlottingLayerMask;
    public LayerMask DeliveryLayerMask;

    int startOffset = 0;
    int endOffset = 0;

    Vector2Int startAdder;
    Vector2Int endAdder;

    private void Start()
    {
        if (SceneHandler.Instance.Path.Count == 0)
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
        }
        else
        {
            Path = SceneHandler.Instance.Path;
            TileSprites = new List<SpriteRenderer>();

            startOffset = SceneHandler.Instance.startOffset;
            endOffset = SceneHandler.Instance.endOffset;
            isConnected = SceneHandler.Instance.PathConnected;
        }

        DeliveryTiles = new List<GameObject>();
        RobberTiles = new List<GameObject>();
        foreach (Transform child in DeliveryTilesParent)
        {
            if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
            {
                DeliveryTiles.Add(child.gameObject);
                Vector2 pos = new Vector2(child.position.x, child.position.z);
                if(SceneHandler.Instance.Delivered.Contains(pos))
                    child.gameObject.GetComponent<DeliveryManager>().makeDelivery();
                else
                {
                    int index = SceneHandler.Instance.DeliveryTargets.IndexOf(pos);
                    child.gameObject.GetComponent<DeliveryManager>().Package = SceneHandler.Instance.DeliveryRequirements[index];
                    child.gameObject.GetComponent<DeliveryManager>().PackageSpriteRenderer.sprite = SceneHandler.Instance.DeliveryRequirements[index];
                }
            }
            else if (child.gameObject.CompareTag("Robber") && child.gameObject.activeSelf)
                RobberTiles.Add(child.gameObject);
        }

        Deliverables = new Dictionary<Transform, List<Vector2>>();

        if(SceneHandler.Instance.Path.Count > 0)
        {
            foreach (var position in Path)
            {
                TileSprites.Add(null);

                foreach (GameObject d in DeliveryTiles)
                {
                    if (d.GetComponent<DeliveryManager>().delivered == true)
                        continue;
                    Vector2 pos = new Vector2(d.transform.position.x, d.transform.position.z);
                    if (new Vector2(position.x + 1, position.y) == pos)
                    {
                        if (Deliverables.ContainsKey(d.transform))
                            Deliverables[d.transform].Add(position);
                        else
                        {
                            Deliverables[d.transform] = new List<Vector2> {position};
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
                foreach (GameObject d in RobberTiles)
                {
                    Vector2 pos = new Vector2(d.transform.position.x, d.transform.position.z);
                    if (new Vector2(position.x + 1, position.y) == pos)
                    {
                        if (Deliverables.ContainsKey(d.transform))
                            Deliverables[d.transform].Add(position);
                        else
                        {
                            Deliverables[d.transform] = new List<Vector2> { position };
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

            }

            MeshRenderer[] meshes = DeliveryTilesParent.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer child in meshes)
            {
                Vector2 pos = new Vector2(child.transform.position.x, child.transform.position.z);
                if (Path.Contains(pos))
                {
                    int index = Path.IndexOf(pos);
                    TileSprites[index] = child.GetComponentInChildren<SpriteRenderer>();
                }
            }
        }

        startAdder = Vector2Int.zero;
        if (StartPoint.transform.position.x == SceneHandler.Instance.Min.x)
            startAdder.x = -1;
        else if (StartPoint.transform.position.x == SceneHandler.Instance.Max.x)
            startAdder.x = 1;
        else if (StartPoint.transform.position.z == SceneHandler.Instance.Min.y)
            startAdder.y = -1;
        else if (StartPoint.transform.position.z == SceneHandler.Instance.Max.y)
            startAdder.y = 1;

        endAdder = Vector2Int.zero;
        if (EndPoint.transform.position.x == SceneHandler.Instance.Min.x)
            endAdder.x = -1;
        else if (EndPoint.transform.position.x == SceneHandler.Instance.Max.x)
            endAdder.x = 1;
        else if (EndPoint.transform.position.z == SceneHandler.Instance.Min.y)
            endAdder.y = -1;
        else if (EndPoint.transform.position.z == SceneHandler.Instance.Max.y)
            endAdder.y = 1;

        updateSprites();

        {
            float x = StartPoint.transform.position.x + startAdder.x;
            float y = StartPoint.transform.position.y + 0.001f;
            float z = StartPoint.transform.position.z + startAdder.y;

            for(int i=0; i<farOffset; i++)
            {
                GameObject empty = new GameObject();
                empty.transform.parent = this.transform;
                empty.AddComponent<SpriteRenderer>().sprite = dictionary[0].sprite;
                empty.transform.position = new Vector3(x, y, z);
                if (startAdder.y == 0)
                    empty.transform.rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                else
                    empty.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                x += startAdder.x;
                z += startAdder.y;
            }
        }

        {
            float x = EndPoint.transform.position.x + endAdder.x;
            float y = EndPoint.transform.position.y + 0.001f;
            float z = EndPoint.transform.position.z + endAdder.y;

            for (int i = 0; i < farOffset; i++)
            {
                GameObject empty = new GameObject();
                empty.transform.parent = this.transform;
                empty.AddComponent<SpriteRenderer>().sprite = dictionary[0].sprite;
                empty.transform.position = new Vector3(x, y, z);
                if(endAdder.y == 0)
                   empty.transform.rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                else
                    empty.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                x += endAdder.x;
                z += endAdder.y;
            }
        }
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
        prev.x += startAdder.x;
        prev.y += startAdder.y;

        for (int i=0; i < startOffset; i++)
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
        prev.x += endAdder.x;
        prev.y += endAdder.y;

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
            if (d.GetComponent<DeliveryManager>().delivered == true)
                continue;
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
        foreach (GameObject d in RobberTiles)
        {
            Vector2 pos = new Vector2(d.transform.position.x, d.transform.position.z);
            if (new Vector2(position.x + 1, position.y) == pos)
            {
                if (Deliverables.ContainsKey(d.transform))
                    Deliverables[d.transform].Add(position);
                else
                {
                    Deliverables[d.transform] = new List<Vector2> { position };
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


    private void Update()
    {
        CursorIndicator.gameObject.SetActive(false);
        if (PlayerController_new.Instance.isSimulating)
            return;

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

        Vector2 pos = Vector2.zero;
        pos.x = StartPoint.transform.position.x + startAdder.x;
        pos.y = StartPoint.transform.position.z + startAdder.y;

        list.Insert(0, pos);

        pos.x += startAdder.x;
        pos.y += startAdder.y;

        list.Insert(0, pos);

        if (isConnected)
        {
            pos = new Vector2(EndPoint.transform.position.x + (endAdder.x * 6), EndPoint.transform.position.z + (endAdder.y * 6));
            list.Add(pos);
        }
        return list;
    }

    public List<Vector2> getRawPath(out int startOffset, out int endOffset)
    {
        startOffset = this.startOffset;
        endOffset = this.endOffset;
        return Path;
    }

    public Dictionary<Transform, List<Vector2>> getDeliverables()
    {
        return Deliverables;
    }
}