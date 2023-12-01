using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlottingTutorial : MonoBehaviour
{
    [Header("Click Tutorial")]
    public float timeGap = 5f;
    public Image tutorialSprite;
    public Vector2 Offset;
    public float moveSpeed = 100.0f;
    public Gradient fallOffs;

    [Header("Queue Indicator")]
    public GameObject arrow;

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
        if (FindObjectOfType<DeliverSystem>().isDelivering)
            arrow.SetActive(true);
        else
            arrow.SetActive(false);

        if (connected)
            return;

        int currStart, currEnd;

        PlottingManager.Instance.getPathRaw(out currStart, out currEnd, out connected);
        bool isSmulating = PlayerController_new.Instance.isSimulating;
        if (connected || isSmulating)
            EndCoroutine();

        if (currStart == prevStart && currEnd == prevEnd && !isSmulating)
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

        pos1 = Camera.main.WorldToScreenPoint(new Vector3(pos1.x, 0.5f, pos1.y));
        pos2 = Camera.main.WorldToScreenPoint(new Vector3(pos2.x, 0.5f, pos2.y));

        tutorialSprite.gameObject.SetActive(true);
        Vector3 spritePos1, spritePos2;
        spritePos1 = new Vector3(pos1.x, pos1.y, 0) + new Vector3(Offset.x, Offset.y, 0);
        spritePos2 = new Vector3(pos2.x, pos2.y, 0) + new Vector3(Offset.x, Offset.y, 0);
        float totalDistance = Vector3.Distance(spritePos1, spritePos2);

        while (true)
        {
            tutorialSprite.transform.position = spritePos1;
            float startTime = Time.time;

            while (Vector3.Distance(tutorialSprite.transform.position, spritePos2) > 0.3f)
            {
                float distCovered = (Time.time - startTime) * moveSpeed;

                tutorialSprite.color = fallOffs.Evaluate(distCovered / totalDistance);
                tutorialSprite.transform.position = Vector3.Lerp(spritePos1, spritePos2, distCovered/totalDistance);
                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
