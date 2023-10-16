using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public List<Vector2> Path;
    List<SpriteRenderer> TileSprites;

    public GameObject StartPoint;
    public GameObject EndPoint;

    public SpriteRenderer CursorIndicator;
    public LayerMask PlottingLayerMask;
    public Sprite ErrorSprite;

    int startOffset = 1;
    int endOffset = 1;

    private void Start()
    {
        Path = new List<Vector2>();
        TileSprites = new List<SpriteRenderer>();

        Path.Add(new Vector2(StartPoint.transform.position.x, StartPoint.transform.position.z));
        Path.Add(new Vector2(EndPoint.transform.position.x, EndPoint.transform.position.z));

        TileSprites.Add(StartPoint.GetComponentInChildren<SpriteRenderer>());
        TileSprites.Add(EndPoint.GetComponentInChildren<SpriteRenderer>());

        UpdateSprites();
    }

    List<Vector2> getPathNeighbours(int index)
    {
        List<Vector2> list = new List<Vector2>();
        list.Add(new Vector2(Path[index].x + 1, Path[index].y));
        list.Add(new Vector2(Path[index].x - 1, Path[index].y));
        list.Add(new Vector2(Path[index].x, Path[index].y + 1));
        list.Add(new Vector2(Path[index].x, Path[index].y - 1));

        return list;
    }

    void UpdateSprites()
    {
        Vector2 prev = Path[0];
        prev.x -= 1;

        Vector2 start, end;
        bool found_key;

        for (int i=0; i<Path.Count-1; i++)
        {
            start = prev - Path[i];
            end = Path[i+1] - Path[i];

            found_key = false;
            foreach(var key in dictionary)
            {
                if((key.start == start && key.end == end) || (key.start == end && key.end == start))
                {
                    TileSprites[i].transform.rotation = Quaternion.Euler(90.0f, key.rotation, 0.0f);
                    TileSprites[i].sprite = key.sprite;
                    found_key = true;
                    break;
                }
            }
            if(!found_key)
            {
                TileSprites[i].sprite = ErrorSprite;
            }

            prev = Path[i];
        }

        start = prev - Path[Path.Count - 1];
        end = -start;

        found_key = false;
        foreach (var key in dictionary)
        {
            if ((key.start == start && key.end == end) || (key.start == end && key.end == start))
            {
                TileSprites[Path.Count - 1].transform.rotation = Quaternion.Euler(90.0f, key.rotation, 0.0f);
                TileSprites[Path.Count - 1].sprite = key.sprite;
                found_key = true;
                break;
            }
        }
        if (!found_key)
        {
            TileSprites[Path.Count - 1].sprite = ErrorSprite;
        }
    }

    private void Update()
    {
        CursorIndicator.gameObject.SetActive(false);
        Ray cursor = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(cursor, out hit, 1000.0f, PlottingLayerMask))
        {
            CursorIndicator.gameObject.SetActive(true);

            GameObject tile = hit.transform.gameObject;
            Vector2 tile_position = new Vector2(tile.transform.position.x, tile.transform.position.z);

            List<Vector2> start_path_neighbours = getPathNeighbours(startOffset-1);
            List<Vector2> end_path_neighbours = getPathNeighbours(Path.Count-endOffset);

            CursorIndicator.transform.position = new Vector3(tile_position.x, CursorIndicator.transform.position.y, tile_position.y);

            if (start_path_neighbours.Contains(tile_position) || end_path_neighbours.Contains(tile_position))
            {
                CursorIndicator.color = Color.green;
            }
            else if (tile_position == Path[0] || tile_position == Path[Path.Count - 1])
            {
                CursorIndicator.color = Color.green;
                return;
            }
            else
            {
                CursorIndicator.color = Color.red;
                return;
            }

            if(Path.Contains(tile_position))
            {
                if ((Path[Path.Count - endOffset] == tile_position) || (Path[startOffset] == tile_position))
                {
                    Debug.Log("Should Delete?");
                }
                else
                {
                    //Invalid Path
                    CursorIndicator.color = Color.red;
                }
            }
            else
            {
                if (start_path_neighbours.Contains(tile_position))
                {
                    Path.Insert(startOffset, tile_position);
                    TileSprites.Insert(startOffset, tile.GetComponentInChildren<SpriteRenderer>());
                    startOffset++;
                }
                else
                {
                    Path.Insert(Path.Count - endOffset, tile_position);
                    TileSprites.Insert(Path.Count - endOffset, tile.GetComponentInChildren<SpriteRenderer>());
                    endOffset++;
                }

                UpdateSprites();
            }
        }
    }
}
