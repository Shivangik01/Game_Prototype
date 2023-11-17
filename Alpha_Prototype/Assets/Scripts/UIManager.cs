using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void ResetPaths()
    {
        AnalyticsHandler.Instance.PostResetUsage(SceneHandler.Instance.Packing_Level);

        SceneHandler.Instance.UI_resetCounts += 1;

        int a, b;
        List<Vector2> values = PlottingManager.Instance.getRawPath(out a, out b);
        if(values.Count > 2 )
            AnalyticsHandler.Instance.PostPathData(values, SceneHandler.Instance.Packing_Level);

        PlayerController_new.Instance.ResetTrain();
        PlottingManager.Instance.ResetPaths();

    }

    public void fastForwardTrain()
    {
        PlayerController_new.Instance.FastForwardTrain();
    }

    public void SwitchPacking(bool state = false)
    {
        SceneHandler.Instance.SwitchToPacking(state);
    }

    public void SwitchPlotting()
    {
        SceneHandler.Instance.SwitchToPlotting();
    }

    public void FadeInInfo(CanvasGroup info)
    {
        info.alpha = 0;
        StartCoroutine(ShowObject(info));
    }

    public void FadeOutInfo(CanvasGroup info)
    {
        StartCoroutine(HideObject(info));
    }

    public void SetActive(GameObject button)
    {
        button.gameObject.SetActive(true);
    }
    public void SetInactive(GameObject button)
    {
        button.gameObject.SetActive(false);
    }

    IEnumerator ShowObject(CanvasGroup image)
    {
        image.alpha = 0.0f;
        while(image.alpha < 0.9f)
        {
            image.alpha += Time.deltaTime * 5.0f;
            yield return null;
        }
        image.alpha = 1.0f;
    }

    IEnumerator HideObject(CanvasGroup image)
    {
        image.alpha = 1.0f;
        while (image.alpha > 0.1f)
        {
            image.alpha -= Time.deltaTime * 5.0f;
            yield return null;
        }
        image.alpha = 0.0f;
    }
    public void LevelSelection()
    {
        GetComponent<Animator>().SetTrigger("levels");
    }
    public void MainTitle()
    {
        GetComponent<Animator>().SetTrigger("startup");
    }

    public void loadLevel(int level_number)
    {
        SceneManager.LoadScene(level_number);
    }

    public void reloadLevel()
    {
        SceneHandler.Instance.ResetToPacking();
    }
}
