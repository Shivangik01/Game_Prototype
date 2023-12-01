using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LevelFinishedUI : MonoBehaviour
{
    public TextMeshProUGUI resetText;
    public TextMeshProUGUI stagesText;
    public TextMeshProUGUI highScoreText;
    public Animator animator;
    public void showData()
    {
        int startOffset, endOffset;
        int PathCount = SceneHandler.Instance.UI_score + PlottingManager.Instance.getRawPath(out startOffset, out endOffset).Count;
        AnalyticsHandler.Instance.PostScore(SceneHandler.Instance.Packing_Level, PathCount);

        if (PathCount < SceneHandler.Instance.highScore)
            SceneHandler.Instance.highScore = PathCount;

        highScoreText.text = "Tracks Used: " + PathCount.ToString() + "\nBest track usage: " + SceneHandler.Instance.highScore.ToString();

        animator.enabled = true;

        stagesText.text = "You delivered packages in "+(SceneHandler.Instance.UI_stagesCount+1).ToString()+" stages";
        //packagesText.text = "You delivered "+(Math.Max(SceneHandler.Instance.UI_packedItems, SceneHandler.Instance.StackedItems.Count)).ToString()+" Packages in a go";
        resetText.text = "You made "+(SceneHandler.Instance.UI_resetCounts).ToString()+" Resets";
    }
}
