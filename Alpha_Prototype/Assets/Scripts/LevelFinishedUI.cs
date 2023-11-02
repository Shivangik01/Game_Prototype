using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelFinishedUI : MonoBehaviour
{
    public TextMeshProUGUI resetText;
    public TextMeshProUGUI stagesText;
    public TextMeshProUGUI packagesText;
    public Animator animator;
    public void showData()
    {
        animator.enabled = true;

        stagesText.text = "You delivered packages in "+(SceneHandler.Instance.UI_stagesCount+1).ToString()+" stages";
        packagesText.text = "You delivered "+(Math.Max(SceneHandler.Instance.UI_packedItems, SceneHandler.Instance.StackedItems.Count)).ToString()+" Packages in a go";
        resetText.text = "You made "+(SceneHandler.Instance.UI_resetCounts).ToString()+" Resets";
    }
}
