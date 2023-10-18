using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlottingTutorial : MonoBehaviour
{
    public SpriteRenderer tutorialSprite;
    public float zOffset = -2.5f;

    public float timeGap = 5f;
    int prevStart, prevEnd;
    bool connected;

    float cur_time;
    bool inCouroutine = false;

    void Start()
    {
        prevStart = 0;
        prevEnd = 0;
        connected = false;

        cur_time = 0;
    }

    void Update()
    {
        if (connected)
            return;

        if (tutorialSprite.gameObject.activeSelf)        
            tutorialSprite.transform.LookAt(Camera.main.transform);
        int currStart, currEnd;

        PlottingManager.Instance.getPathRaw(out currStart, out currEnd, out connected);
        
        if (connected)
            EndCoroutine();

        if (currStart == prevStart && currEnd == prevEnd)
            cur_time += Time.deltaTime;
        else
            cur_time = 0;

        if(cur_time-timeGap > 0)
        {
            if(!inCouroutine)
            {
                inCouroutine = true;
                StartCoroutine(ShowHint());
            }    
        }
        else
        {
            EndCoroutine();
        }

        prevEnd = currEnd;
        prevStart = currStart;
    }

    void EndCoroutine()
    {
        if(inCouroutine)
        {
            StopAllCoroutines();
            tutorialSprite.gameObject.SetActive(false);
            inCouroutine = false;
            cur_time = 0;
        }
    }

    IEnumerator ShowHint()
    {
        Vector2 pos1, pos2;
        PlottingManager.Instance.getOpenPoints(out pos1, out pos2);
        tutorialSprite.gameObject.SetActive(true);        
        Vector3 spritePos1, spritePos2;
        spritePos1 = new Vector3(pos1.x, tutorialSprite.transform.position.y, pos1.y + zOffset);
        spritePos2 = new Vector3(pos2.x, tutorialSprite.transform.position.y, pos2.y + zOffset);
        
        while (true)
        {
            tutorialSprite.transform.position = spritePos1;
            float alphaFactor = 0;
            tutorialSprite.color = new Color(1, 1, 1, alphaFactor);

            while (Vector3.Distance(tutorialSprite.transform.position, spritePos2) > 0.3f)
            {
                alphaFactor += Time.deltaTime;
                alphaFactor = Mathf.Clamp01(alphaFactor);
                tutorialSprite.color = new Color(1, 1, 1, alphaFactor);
                tutorialSprite.transform.position = Vector3.Lerp(tutorialSprite.transform.position, spritePos2, Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
