using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public Vector2Int Min;
    public Vector2Int Max;

    public List<Vector2> DeliveryTargets;
    public List<Sprite> DeliveryRequirements;

    public int Packing_Level;
    public int Plotting_Level;

    public List<Vector2> Path;
    public int startOffset, endOffset;
    public List<Vector2> Delivered;

    public static SceneHandler Instance = null;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            if(Instance.Packing_Level == Packing_Level)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SwitchToPlotting()
    {
        SceneManager.LoadScene(Plotting_Level);
    }

    public void SwitchToPacking()
    {
        Path = PlottingManager.Instance.getRawPath(out startOffset, out endOffset);
        SceneManager.LoadScene(Packing_Level);
    }
}