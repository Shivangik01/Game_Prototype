using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class TileManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static TileManager Instance;
    private List<Vector3> usedTilesIndexes = new();
    private List<int> tilesToBeDestroyed = new();
    private float lerpTime = 0;

    public void AddusedTiles(GameObject obj)
    {
        if (TileHandler.Instance.tiles.Contains(obj))
        {
            //int index = TileHandler.Instance.tiles.FindIndex(myobj=>myobj==obj);

            usedTilesIndexes.Add(obj.transform.position);
            //Destroy(TileHandler.Instance.tiles[index]);
        }

        Debug.Log("used tile count" + usedTilesIndexes.Count);


    }

    public void ResetusedTiles()
    {
        usedTilesIndexes.Clear();
        tilesToBeDestroyed.Clear();
        Debug.Log("cleared all tiles");
    }

    public void RemoveTiles(GameObject obj)
    {
        if (TileHandler.Instance.tiles.Contains(obj))
        {
            //int index = TileHandler.Instance.tiles.FindIndex(myobj => myobj == obj);

            usedTilesIndexes.Remove(obj.transform.position);
        }
        Debug.Log("used tile count" + usedTilesIndexes.Count);
    }

    public void SetDestroyedIndex()
    {
        int pathTobeDestroyed = (((usedTilesIndexes.Count - 2) * 30) / 100);
        var random = new System.Random();
        int index;
        Debug.Log("10% data" + pathTobeDestroyed);

        for (int i = 0; i < pathTobeDestroyed; i++)
        {
            do
            {
                index = random.Next(1, usedTilesIndexes.Count - 1);
            }
            while (tilesToBeDestroyed.Contains(index));
            tilesToBeDestroyed.Add(index);

        }
        Debug.Log("tile to be destryoed" + tilesToBeDestroyed.Count);

    }

    public IEnumerator DestroyedTiles()
    {
        foreach (var index in tilesToBeDestroyed)
        {
            foreach (var tile in TileHandler.Instance.tiles)
            {
                if (tile.transform.position == usedTilesIndexes[index])
                {
                    //Destroy(TileHandler.Instance.tiles[index]);
                    //tile.SetActive(false);
                    //tile.transform.position = new Vector3(0, Mathf.Lerp(tile.transform.position.y, tile.transform.position.y-2, 0 ), 0);
                    StartCoroutine(LerpToPosition(tile.transform));
                    yield return new WaitForSeconds(0.5f);
                    Debug.Log("destryoing tiles of index" + index);

                }

            }
        }

    }

    private IEnumerator LerpToPosition(Transform tileTransform)
    {
        tileTransform.gameObject.layer = LayerMask.GetMask("Ignore Raycast");
        Vector3 Gotoposition = new Vector3(tileTransform.position.x, tileTransform.position.y - 50.0f, tileTransform.position.z);
        float elapsedTime = 0;
        float waitTime = 5;
        Vector3 currentPos = tileTransform.position;

        while (elapsedTime < waitTime)
        {
            tileTransform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(tileTransform.gameObject);
    }


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
