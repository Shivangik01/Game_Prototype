using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public static TileHandler Instance;
    public List<GameObject> tiles = new List<GameObject>();


    public void SetAllTiles()
    {

        tiles = GameObject.FindGameObjectsWithTag("tiles").ToList();

        Debug.Log("no of tiles is" + tiles.Count);
    }

    public void ResetAllTiles()
    {
        tiles.Clear();
        Debug.Log("Tiles Reset");
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
