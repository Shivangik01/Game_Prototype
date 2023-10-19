using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public void ResetPaths()
    {
        PlayerController.Instance.gameObject.GetComponent<DeliverSystem>().KillCoroutines();
        
        foreach (var e in PlottingManager.Instance.getDeliverables())
        {
            Animator anim = e.Key.GetComponent<DeliveryManager>().playerAnimator;
            anim.SetBool("Happy", false);
            anim.Rebind();
            anim.Update(0f);
        }
        
        PlottingManager.Instance.ResetPaths();
        PlayerController.Instance.ResetTrain();
    }

    public void SwitchPacking()
    {
        SceneHandler.Instance.SwitchToPacking();
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
}
