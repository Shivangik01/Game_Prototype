using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class QueueUI : MonoBehaviour
{
    public static QueueUI Instance = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public GameObject TilePrefab;
    public Transform ContainerTransform;

    List<Sprite> original_sprites;
    List<GameObject> spawnedTiles;

    Queue<int> pops;
    bool isPopping;

    private void Start()
    {
        isPopping = false;
        original_sprites = new List<Sprite>();
        pops = new Queue<int>();
        Queue<Vector2> tiles = SceneHandler.Instance.StackedItems;
        while (tiles.Count > 0)
        {
            Vector2 loc = tiles.Dequeue();
            int index = SceneHandler.Instance.DeliveryTargets.IndexOf(loc);
            original_sprites.Add(SceneHandler.Instance.DeliveryRequirements[index]);
        }

        spawnedTiles = new List<GameObject>();
        ResetUI();
    }

    public void ResetUI()
    {
        StopAllCoroutines();
        isPopping = false;
        foreach (GameObject g in spawnedTiles)
            Destroy(g);
        spawnedTiles.Clear();

        float yOffset = -20;
        foreach (Sprite sprite in original_sprites)
        {
            GameObject tile = Instantiate(TilePrefab, ContainerTransform);
            tile.GetComponent<Image>().sprite = sprite;
            RectTransform rectTransform = tile.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(50, yOffset, 0);
            rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);

            spawnedTiles.Add(tile);

            yOffset -= sprite.rect.height * tile.transform.localScale.x;
            yOffset -= 20;
        }
    }

    private void Update()
    {
        if(pops.Count > 0 && !isPopping)
        {
            pops.Dequeue();
            StartCoroutine(Popping());
        }
    }

    public void PopQueue()
    {
        pops.Enqueue(1);
    }

    IEnumerator Popping()
    {
        isPopping = true;
        if (spawnedTiles.Count != 0)
        {
            Destroy(spawnedTiles[0]);
            spawnedTiles.RemoveAt(0);
        }
        if(spawnedTiles.Count != 0)
        {
            
            float diff = -20 - spawnedTiles[0].GetComponent<RectTransform>().localPosition.y;
            while (spawnedTiles[0].GetComponent<RectTransform>().localPosition.y < -20.5f)
            {
                float delta = Time.deltaTime * diff * 1.0f;
                foreach (GameObject g in spawnedTiles)
                {
                    g.transform.localPosition = new Vector3(g.GetComponent<RectTransform>().localPosition.x, g.GetComponent<RectTransform>().localPosition.y + delta, g.GetComponent<RectTransform>().localPosition.z);
                }
                yield return null;
            }

            float yOffset = -20;
            foreach (GameObject g in spawnedTiles)
            {
                g.transform.localPosition = new Vector3(g.transform.localPosition.x, yOffset, g.transform.localPosition.z);

                yOffset -= g.GetComponent<Image>().sprite.rect.height * g.transform.localScale.x;
                yOffset -= 20;
            }

        }
        yield return null;
        isPopping = false;
    }
}
