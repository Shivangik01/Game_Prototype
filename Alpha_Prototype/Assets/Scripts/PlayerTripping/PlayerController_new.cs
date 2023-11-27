using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_new : MonoBehaviour
{
    public static PlayerController_new Instance = null;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
        }
    }

    [Header("Train Attribs")]
    public float normal_RotationSpeed = 1.5f;
    public float normal_speed = 2f;
    public float fast_RotationSpeed = 3f;
    public float fast_speed = 4f;
    public Transform Engine;
    public Transform Carriage;

    float RotationSpeed;
    float speed;

    Vector3 enginePos;
    Vector3 carriagePos;
    Quaternion engineRot;
    Quaternion carriageRot;

    [Header("References")]
    public Transform DeliveryTilesParent;
    public Animator DoorAnim;

    [Header("UI")]
    public GameObject pauseButton;
    public GameObject playButton;
    public LevelFinishedUI LevelComplete;

    public Camera mainCamera;

    [Header("Simulation Cameras")]
    public Transform originalCamera;
    public Transform simulationCamera;

    public bool isSimulating;

    List<DeliveryManager> deliveries;
    List<RobberManager> robberies;

    List<DeliveryManager> deliveriesMade;

    private void Start()
    {
        speed = normal_speed;
        RotationSpeed = normal_RotationSpeed;

        enginePos = Engine.transform.position;
        engineRot = Engine.transform.rotation;

        carriagePos = Carriage.transform.position;
        carriageRot = Carriage.transform.rotation;

        Carriage.gameObject.SetActive(false);

        deliveriesMade = new List<DeliveryManager>();
        deliveries = new List<DeliveryManager>();
        robberies = new List<RobberManager>();

        isSimulating = false;

        foreach (Transform child in DeliveryTilesParent)
        {
            if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
            {
                deliveries.Add(child.gameObject.GetComponent<DeliveryManager>());
            }
            else if (child.gameObject.CompareTag("Robber") && child.gameObject.activeSelf)
            {
                robberies.Add(child.gameObject.GetComponent<RobberManager>());
            }
        }
    }

    public void MoveAlongPath()
    {
        GetComponent<DeliverSystem>().KillCoroutines();

        StopAllCoroutines();
        StartCoroutine(CameraMove(originalCamera, simulationCamera));

        StartCoroutine(Simulate());
    }

    public void ResetTrain()
    {
        speed = normal_speed;
        RotationSpeed = normal_RotationSpeed;

        GetComponent<DeliverSystem>().KillCoroutines();
        StopAllCoroutines();

        DoorAnim.SetBool("Open", false);
        QueueUI.Instance.ResetUI();

        foreach (var target in deliveriesMade)
        {
            target.resetDelivery();
        }
        foreach (var robber in robberies)
        {
            robber.resetDelivery();
        }

        Engine.transform.position = enginePos;
        Engine.transform.rotation = engineRot;

        Carriage.transform.position = carriagePos;
        Carriage.transform.rotation = carriageRot;

        if (isSimulating)
            StartCoroutine(CameraMove(simulationCamera, originalCamera));

        pauseButton.SetActive(false);
        playButton.SetActive(true);

        isSimulating = false;
    }

    public void FastForwardTrain()
    {
        speed = fast_speed;
        RotationSpeed = fast_RotationSpeed;
    }

    IEnumerator CameraMove(Transform A, Transform B)
    {
        Camera camera = Camera.main;
        camera.transform.position = A.position;
        camera.transform.rotation = A.rotation;

        float totalDistance = Vector3.Distance(A.position, B.position);

        float startTime = Time.time;
        while (Vector3.Distance(camera.transform.position, B.position) > 0.1f)
        {
            float distCovered = (Time.time - startTime) * 5.0f;
            float t = distCovered / totalDistance;
            t = Mathf.Clamp01(t);

            camera.transform.position = Vector3.Lerp(A.position, B.position, t);
            camera.transform.rotation = Quaternion.Lerp(A.rotation, B.rotation, t);

            yield return null;
        }

        camera.transform.position = B.position;
        camera.transform.rotation = B.rotation;

        yield return null;
    }

    IEnumerator Simulate()
    {
        isSimulating = true;

        deliveriesMade.Clear();
        Queue<Vector2> deliveries_queue = new Queue<Vector2>(SceneHandler.Instance.StackedItems);
        List<Vector2> path = PlottingManager.Instance.getTraversalPath();

        float y_pos = Engine.transform.position.y;

        Carriage.transform.position = new Vector3(path[0].x, y_pos, path[0].y);
        Engine.transform.position = new Vector3(path[1].x, y_pos, path[1].y);

        int includeLastValue = 0;
        if (PlottingManager.Instance.isConnected)
            includeLastValue = -1;

        bool stolen = false;

        Vector3 engine_prev = new Vector3(path[1].x, y_pos, path[1].y);
        Vector3 carriage_prev = new Vector3(path[0].x, y_pos, path[0].y);
        for (int i = 1; i < path.Count + includeLastValue; i++)
        {
            Vector3 engine_curr = new Vector3(path[i].x, y_pos, path[i].y);
            Vector3 carriage_curr = new Vector3(path[i-1].x, y_pos, path[i-1].y);

            Quaternion engine_prevRot = Engine.transform.rotation;
            Quaternion engine_nextRot;

            Quaternion carriage_prevRot = Carriage.transform.rotation;
            Quaternion carriage_nextRot = engine_prevRot;

            if(i + 1 < path.Count)
            {
                Vector3 forward = new Vector3(path[i + 1].x, y_pos, path[i + 1].y) - engine_curr;
                forward.Normalize();
                Vector3 euler = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
                engine_nextRot = Quaternion.Euler(euler.x, euler.y + 180, euler.z);
            }
            else
            {
                engine_nextRot = engine_prevRot;
            }

            float startTime = Time.time;
            float totalDistance = Vector3.Distance(engine_curr, engine_prev);

            while (Vector3.Distance(Engine.transform.position, engine_curr) > 0.01f)
            {
                float dist_covered = (Time.time - startTime) * speed;
                float t = dist_covered / totalDistance;
                t = Mathf.Clamp01(t);
                Engine.transform.position = Vector3.Lerp(engine_prev, engine_curr, t);
                Engine.transform.rotation = Quaternion.Slerp(Engine.transform.rotation, engine_nextRot, Time.deltaTime * RotationSpeed);

                Carriage.transform.position = Vector3.Lerp(carriage_prev, carriage_curr, t);
                Carriage.transform.rotation = Quaternion.Slerp(Carriage.transform.rotation, carriage_nextRot, Time.deltaTime * RotationSpeed);

                yield return null;
            }
            Engine.transform.position = engine_curr;
            Carriage.transform.position = carriage_curr;

            RobberManager robberTile = null;
            DeliveryManager deliveryTile = null;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == y || (x == 1 && y == -1) || (y == 1 && x == -1))
                        continue;

                    Vector2 pos = new Vector2(engine_curr.x, engine_curr.z);
                    pos.x += x;
                    pos.y += y;

                    foreach (DeliveryManager d in deliveries)
                    {
                        Vector2 d_pos = new Vector2(d.transform.position.x, d.transform.position.z);
                        if (Vector2.Distance(d_pos, pos) <= 0.1f && deliveries_queue.Count > 0 && deliveries_queue.Peek() == d_pos)
                        {
                            if (!d.delivered)
                            {
                                deliveryTile = d;
                                break;
                            }
                        }
                    }

                    if (deliveryTile != null)
                        break;
                }

                if (deliveryTile != null)
                    break;
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == y || (x == 1 && y == -1) || (y == 1 && x == -1))
                        continue;

                    Vector2 pos = new Vector2(engine_curr.x, engine_curr.z);
                    pos.x += x;
                    pos.y += y;

                    foreach (RobberManager d in robberies)
                    {
                        Vector2 d_pos = new Vector2(d.transform.position.x, d.transform.position.z);
                        if (Vector2.Distance(d_pos, pos) <= 0.1f && deliveries_queue.Count > 0)
                        {
                            if (!d.delivered)
                            {
                                d.deliverPersonPosition = deliveries_queue.Peek();
                                robberTile = d;
                                AnalyticsHandler.Instance.PostRobberInteractions(d_pos, SceneHandler.Instance.Packing_Level);
                                break;
                            }
                        }
                    }

                    if (robberTile != null)
                        break;
                }

                if (robberTile != null)
                    break;
            }

            if (robberTile != null)
            {
                stolen = true;
                deliveries_queue.Dequeue();
                QueueUI.Instance.PopQueue();
                transform.GetComponent<DeliverSystem>().deliverObject(robberTile);
                Color darkRed = new Color(180f / 255f, 0f / 255f, 1f / 255f);
                Color originalColor = mainCamera.backgroundColor;
                mainCamera.backgroundColor = darkRed;
                yield return new WaitForSeconds(2.0f);
                mainCamera.backgroundColor = originalColor;
            }
            else if (deliveryTile != null)
            {
                deliveriesMade.Add(deliveryTile);
                deliveries_queue.Dequeue();
                QueueUI.Instance.PopQueue();
                transform.GetComponent<DeliverSystem>().deliverObject(deliveryTile);
                yield return new WaitForSeconds(2.0f);

            }

            if (deliveries_queue.Count == 0 && !stolen)
            {
                includeLastValue = 0;
                DoorAnim.SetBool("Open", true);
            }

            engine_prev = engine_curr;
            carriage_prev = carriage_curr;
        }

        if (PlottingManager.Instance.isConnected && deliveries_queue.Count == 0 && !stolen)
        {
            int a, b;
            List<Vector2> rawPath = PlottingManager.Instance.getRawPath(out a, out b);
            AnalyticsHandler.Instance.PostPathData(PlottingManager.Instance.getRawPath(out a, out b), SceneHandler.Instance.Packing_Level);

            yield return new WaitForSeconds(1.0f);

            if (SceneHandler.Instance.Delivered.Count + SceneHandler.Instance.StackedItems.Count == SceneHandler.Instance.DeliveryTargets.Count)
            {
                LevelComplete.gameObject.SetActive(true);
                LevelComplete.showData();

                removeRobberDemandBox();
                AnalyticsHandler.Instance.PostCompletion(SceneHandler.Instance.Packing_Level);
            }
            else
                SceneHandler.Instance.SwitchToPacking(true);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
            ResetTrain();
        }

        yield return null;
    }

    private void removeRobberDemandBox()
    {
        foreach (RobberManager d in robberies)
        {
            d.levelComplete();
        }
    }
}