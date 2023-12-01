using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void ResetPaths()
    {
        audioManager.instance.playButton();

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
        audioManager.instance.playButton();
        PlayerController_new.Instance.FastForwardTrain();
    }

    public void SwitchPacking(bool state = false)
    {
        audioManager.instance.playButton();
        SceneHandler.Instance.SwitchToPacking(state);
    }

    public void SwitchPlotting()
    {
        audioManager.instance.playButton();
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
        audioManager.instance.playButton();
        GetComponent<Animator>().SetTrigger("levels");
    }
    public void MainTitle()
    {
        audioManager.instance.playButton();
        GetComponent<Animator>().SetTrigger("startup");
    }

    public void loadLevel(int level_number)
    {
        audioManager.instance.playButton();
        SceneManager.LoadScene(level_number);
        Scene currentScene = SceneManager.GetActiveScene();

        // Use GameObject.Find to find a specific object by name
        GameObject robberObject = GameObject.Find("RobberInfo");

        if( robberObject){
            Debug.Log("found robber object ");
        }
    }

    public void reloadLevel()
    {
        audioManager.instance.playButton();
        SceneHandler.Instance.ResetToPacking();
    }
}
