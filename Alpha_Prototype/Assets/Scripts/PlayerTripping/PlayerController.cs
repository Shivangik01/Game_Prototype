using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;

    void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Adjust speeds for smoother motion.
    public float movementSpeed = 2f; // Decreased for more realistic train speed.
    public float rotationSpeed = 50f; // Decreased to smooth out turns.

    public Transform DeliveryTilesParent;

    public Transform Engine;
    public Transform Carriage;

    private Queue<Vector3> pathPositions;
    private Queue<Quaternion> pathRotations;
    private bool isEngineMoving;

    public GameObject pauseButton;
    public GameObject playButton;
    public float carriageOffset=0.1f;
    public Transform originalCamera;
    public Transform simulationCamera;

    // Reduced distance for closer follow.
    private float minDistanceToEngine = 1.0f; // This ensures the carriage remains close.

    public List<DeliveryManager> deliveries;
    public bool isSimulating;

    private void Start()
    {
        // Carriage.gameObject.SetActive(false);
        isSimulating = false;

        pathPositions = new Queue<Vector3>();
        pathRotations = new Queue<Quaternion>();
        isEngineMoving = false;

        foreach (Transform child in DeliveryTilesParent)
        {
            if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
            {
                deliveries.Add(child.gameObject.GetComponent<DeliveryManager>());
            }
        }
    }

    public void MoveAlongPath()
    {
        pathPositions.Clear(); // Clearing the previous path, if any.
        pathRotations.Clear(); // Clearing the previous rotations, if any.
        isEngineMoving = true; // Set the movement flag.

        GetComponent<DeliverSystem>().KillCoroutines();

        foreach (var e in PlottingManager.Instance.getDeliverables())
        {
            Animator anim = e.Key.GetComponent<DeliveryManager>().playerAnimator;
            anim.SetBool("Happy", false);
            anim.Rebind();
            anim.Update(0f);
        }

        StopAllCoroutines();
        StartCoroutine(CameraMove(originalCamera, simulationCamera));
        StartCoroutine(TraversePath());
       StartCoroutine(FollowEngine());
    }

    IEnumerator CameraMove(Transform A, Transform B)
    {
        Camera camera = Camera.main;
        camera.transform.position = A.position;
        camera.transform.rotation = A.rotation;
        
        float totalDistance = Vector3.Distance(A.position, B.position);

        float startTime = Time.time;
        while(Vector3.Distance(camera.transform.position, B.position) > 0.1f)
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

    public void ResetTrain()
    {
        GetComponent<DeliverSystem>().KillCoroutines();

        foreach (var e in PlottingManager.Instance.getDeliverables())
        {
            e.Key.GetComponent<DeliveryManager>().resetDelivery();
        }

        Engine.gameObject.SetActive(false);
        Carriage.gameObject.SetActive(false);
        StopAllCoroutines();

        StartCoroutine(CameraMove(simulationCamera, originalCamera));
        QueueUI.Instance.ResetUI();

        pauseButton.SetActive(false);
        playButton.SetActive(true);

        isSimulating = false;
    }
   

    IEnumerator TraversePath()
    {
        Queue<Vector2> deliveries_queue = new Queue<Vector2>(SceneHandler.Instance.StackedItems);
        Engine.gameObject.SetActive(true);
        Carriage.gameObject.SetActive(true);

        isSimulating = true;
        int index = 0;
        List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();

        // Set initial position for the Engine.
        Vector3 initialPosition = new Vector3(traversalPath[0].x, Engine.position.y, traversalPath[0].y);
        Engine.position = initialPosition;

        // Set initial rotation for the Engine. This rotation might change based on your model's orientation.
        Engine.rotation = Quaternion.Euler(-90, 0, -90);

        // Calculate the position just behind the engine. You might need to adjust the offset depending on the scale of your models.
         carriageOffset = 2.8f; // The distance between the engine and the carriage - This value can be adjusted.
        Vector3 behindEngine = initialPosition - Engine.forward * carriageOffset;

        // Set the Carriage's position and rotation to match the Engine's, but positioned behind.
     Carriage.position = new Vector3(behindEngine.x, behindEngine.y, behindEngine.z);
        // Carriage.position = behindEngine;
        Carriage.rotation = Engine.rotation;

        while (true)
        {
            if (index >= traversalPath.Count)
            {
                break; // Path completed, exit loop.
            }

            Vector3 targetPoint = new Vector3(traversalPath[index].x, Engine.position.y, traversalPath[index].y);

            if (Vector3.Distance(Engine.position, targetPoint) < 0.1f)
            {
                DeliveryManager deliveryTile = null;
                for(int x=-1; x<=1; x++)
                {
                    for(int y = -1; y<=1; y++)
                    {
                        if (x == y || (x == 1 && y == -1) || (y == 1 && x == -1))
                            continue;

                        Vector2 pos = new Vector2(targetPoint.x, targetPoint.z);
                        pos.x += x;
                        pos.y += y;

                        foreach(DeliveryManager d in deliveries)
                        {
                            Vector2 d_pos = new Vector2(d.transform.position.x, d.transform.position.z);
                            if (Vector2.Distance(d_pos, pos) <= 0.1f && deliveries_queue.Count>0 && deliveries_queue.Peek() == d_pos)
                            {
                                if (!d.delivered)
                                {
                                    deliveries_queue.Dequeue();
                                    QueueUI.Instance.PopQueue();
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

                if (deliveryTile != null)
                {
                    transform.GetComponent<DeliverSystem>().deliverObject(deliveryTile);
                    yield return new WaitForSeconds(2.0f);
                }
                index++; // Target point reached, proceed to next point.
                continue;
            }

            // Calculate the direction to the target point, ensuring no vertical movement.
            Vector3 directionToNextPoint = (targetPoint - Engine.position).normalized;
            directionToNextPoint.y = 0; // Maintain horizontal direction.

            if (directionToNextPoint != Vector3.zero)
            {
                // Find the rotation to the next point.
                Quaternion toRotation = Quaternion.LookRotation(directionToNextPoint);

                // Correct the forward direction due to model-world space differences.
                toRotation *= Quaternion.Euler(0, 180, 0); // Flip the forward direction.

                // Extract the Euler angles and preserve the current X rotation of -90 degrees.
                Vector3 rotationAngles = toRotation.eulerAngles;
                rotationAngles.x = -90; // Keep the X rotation locked at -90 degrees.

                // Create a new Quaternion rotation from the modified Euler angles.
                toRotation = Quaternion.Euler(rotationAngles);

                // Smoothly rotate the Engine towards the calculated rotation.
                Engine.rotation = Quaternion.Slerp(Engine.rotation, toRotation, Time.deltaTime * rotationSpeed);
            }

            // Move the Engine towards the target point.
            Engine.position = Vector3.MoveTowards(Engine.position, targetPoint, movementSpeed * Time.deltaTime);
            Carriage.position = Vector3.MoveTowards(Carriage.position, targetPoint, movementSpeed * Time.deltaTime);

            // Store the engine's current position and rotation.
            pathPositions.Enqueue(Engine.position);
            pathRotations.Enqueue(Engine.rotation); // Store the engine's current rotation.

            yield return null; // Proceed to the next frame before continuing the loop.
        }

        yield return new WaitForSeconds(3.0f);
        Engine.gameObject.SetActive(false);
        isSimulating = false;

        if (PlottingManager.Instance.isConnected && deliveries_queue.Count == 0)
        {
            //push the path data
            int a, b;
            List<Vector2> rawPath = PlottingManager.Instance.getRawPath(out a, out b);
            AnalyticsHandler.Instance.PostPathData(PlottingManager.Instance.getRawPath(out a, out b), SceneHandler.Instance.Packing_Level);
            SceneHandler.Instance.SwitchToPacking(true);
        }
        else
        {
            ResetTrain();
            isEngineMoving = false; 
        }
    }

IEnumerator FollowEngine()
{
    Carriage.gameObject.SetActive(true);

    Vector3 lastPosition = Engine.position - Engine.position * carriageOffset;
    Quaternion lastRotation = Engine.rotation;

    while (isEngineMoving || pathPositions.Count > 0)
    {
        if (pathPositions.Count > 0 && pathRotations.Count > 0)
        {
            Vector3 targetPosition = pathPositions.Peek();
            Quaternion targetRotation = pathRotations.Peek();

            float distanceToEngine = Vector3.Distance(Carriage.position, targetPosition);

            if (distanceToEngine < minDistanceToEngine)
            {
                Carriage.position = targetPosition;
                Carriage.rotation = targetRotation;
                lastPosition = pathPositions.Dequeue();
                lastRotation = pathRotations.Dequeue();
            }
            else
            {
                Carriage.position = Vector3.MoveTowards(Carriage.position, targetPosition - Engine.position * 0.01f, movementSpeed * Time.deltaTime);
                Carriage.rotation = Quaternion.Slerp(Carriage.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {

            // If the engine has stopped
            Vector3 LposOffset = new Vector3(Engine.position.x * 0.75f, Engine.position.y * 0.75f, Engine.position.z);
            
            // Carriage.position = Vector3.MoveTowards(Carriage.position, lastPosition - Engine.position * carriageOffset, movementSpeed * Time.deltaTime);
            Carriage.position = Vector3.MoveTowards(Carriage.position, lastPosition - LposOffset, movementSpeed * Time.deltaTime);

            Carriage.rotation = Quaternion.Slerp(Carriage.rotation, lastRotation, Time.deltaTime * rotationSpeed);
            // Carriage.rotation = Quaternion.Slerp(Carriage.rotation, lastRotation * Quaternion.Euler(0, -Engine.rotation.eulerAngles.y * 0.2f, 0), Time.deltaTime * rotationSpeed);

        }

        yield return null; 
    }

    // Carriage.gameObject.SetActive(false);
    Carriage.position = lastPosition;
    Carriage.rotation = lastRotation;
}

}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerController : MonoBehaviour
// {
//     public static PlayerController Instance = null;

//     void Awake()
//     {
//         if (Instance == null)
//             Instance = this;
//         else
//         {
//             Destroy(this.gameObject);
//         }
//     }

//     public float movementSpeed = 2f;
//     public float rotationSpeed = 50f;

//     public Transform DeliveryTilesParent;

//     public Transform Engine;
//     public Transform Carriage;

//     private Queue<Vector3> pathPositions;
//     private Queue<Quaternion> pathRotations;
//     private bool isEngineMoving;
//     private bool isCarriageMoving;

//     public GameObject pauseButton;
//     public GameObject playButton;

//     private float minDistanceToEngine = 2f;

//     public List<DeliveryManager> deliveries;
//     public bool isSimulating;

//     private void Start()
//     {
//         isSimulating = false;

//         pathPositions = new Queue<Vector3>();
//         pathRotations = new Queue<Quaternion>();
//         isEngineMoving = false;
//         isCarriageMoving = false;

//         foreach (Transform child in DeliveryTilesParent)
//         {
//             if (child.gameObject.CompareTag("Targets") && child.gameObject.activeSelf)
//             {
//                 deliveries.Add(child.gameObject.GetComponent<DeliveryManager>());
//             }
//         }
//     }

//     public void MoveAlongPath()
//     {
//         pathPositions.Clear();
//         pathRotations.Clear();
//         isEngineMoving = true;
//         isCarriageMoving = true;

//         GetComponent<DeliverSystem>().KillCoroutines();

//         foreach (var e in PlottingManager.Instance.getDeliverables())
//         {
//             Animator anim = e.Key.GetComponent<DeliveryManager>().playerAnimator;
//             anim.SetBool("Happy", false);
//             anim.Rebind();
//             anim.Update(0f);
//         }

//         StopAllCoroutines();
//         StartCoroutine(TraversePath());
//         StartCoroutine(FollowEngine());
//     }

//     public void ResetTrain()
//     {
//         GetComponent<DeliverSystem>().KillCoroutines();

//         foreach (var e in PlottingManager.Instance.getDeliverables())
//         {
//             e.Key.GetComponent<DeliveryManager>().resetDelivery();
//         }

//         Engine.gameObject.SetActive(false);
//         Carriage.gameObject.SetActive(false);
//         StopAllCoroutines();

//         QueueUI.Instance.ResetUI();

//         pauseButton.SetActive(false);
//         playButton.SetActive(true);

//         isSimulating = false;
//     }

//     IEnumerator TraversePath()
//     {
//         Queue<Vector2> deliveries_queue = new Queue<Vector2>(SceneHandler.Instance.StackedItems);
//         Engine.gameObject.SetActive(true);
//         isSimulating = true;
//         int index = 0;
//         List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();

//         Vector3 initialPosition = new Vector3(traversalPath[0].x, Engine.position.y, traversalPath[0].y);
//         Engine.position = initialPosition;

//         Engine.rotation = Quaternion.Euler(-90, 0, 0);

//         float carriageOffset = 3.0f;
//         Vector3 behindEngine = initialPosition - Engine.forward * carriageOffset;

//         Carriage.position = new Vector3(behindEngine.x, Carriage.position.y, behindEngine.z);
//         Carriage.rotation = Engine.rotation;

//         while (true)
//         {
//             if (index >= traversalPath.Count)
//             {
//                 break;
//             }

//             Vector3 targetPoint = new Vector3(traversalPath[index].x, Engine.position.y, traversalPath[index].y);

//             if (Vector3.Distance(Engine.position, targetPoint) < 0.1f)
//             {
//                 DeliveryManager deliveryTile = null;
//                 for (int x = -1; x <= 1; x++)
//                 {
//                     for (int y = -1; y <= 1; y++)
//                     {
//                         if (x == y || (x == 1 && y == -1) || (y == 1 && x == -1))
//                             continue;

//                         Vector2 pos = new Vector2(targetPoint.x, targetPoint.z);
//                         pos.x += x;
//                         pos.y += y;

//                         foreach (DeliveryManager d in deliveries)
//                         {
//                             Vector2 d_pos = new Vector2(d.transform.position.x, d.transform.position.z);
//                             if (Vector2.Distance(d_pos, pos) <= 0.1f && deliveries_queue.Count > 0 && deliveries_queue.Peek() == d_pos)
//                             {
//                                 if (!d.delivered)
//                                 {
//                                     deliveries_queue.Dequeue();
//                                     QueueUI.Instance.PopQueue();
//                                     deliveryTile = d;
//                                     break;
//                                 }
//                             }
//                         }

//                         if (deliveryTile != null)
//                             break;
//                     }

//                     if (deliveryTile != null)
//                         break;
//                 }

//                 if (deliveryTile != null)
//                 {
//                     transform.GetComponent<DeliverSystem>().deliverObject(deliveryTile);
//                     yield return new WaitForSeconds(2.0f);
//                 }
//                 index++;
//                 continue;
//             }

//             Vector3 directionToNextPoint = (targetPoint - Engine.position).normalized;
//             directionToNextPoint.y = 0;

//             if (directionToNextPoint != Vector3.zero)
//             {
//                 Quaternion toRotation = Quaternion.LookRotation(directionToNextPoint);
//                 toRotation *= Quaternion.Euler(0, 180, 0);
//                 Vector3 rotationAngles = toRotation.eulerAngles;
//                 rotationAngles.x = -90;
//                 toRotation = Quaternion.Euler(rotationAngles);

//                 Engine.rotation = Quaternion.Slerp(Engine.rotation, toRotation, Time.deltaTime * rotationSpeed);
//             }

//             Engine.position = Vector3.MoveTowards(Engine.position, targetPoint, movementSpeed * Time.deltaTime);

//             pathPositions.Enqueue(Engine.position);
//             pathRotations.Enqueue(Engine.rotation);

//             yield return null;
//         }

//         yield return new WaitForSeconds(3.0f);
//         Engine.gameObject.SetActive(false);
//         isSimulating = false;

//         if (PlottingManager.Instance.isConnected && deliveries_queue.Count == 0)
//         {
//             int a, b;
//             List<Vector2> rawPath = PlottingManager.Instance.getRawPath(out a, out b);
//             AnalyticsHandler.Instance.PostPathData(rawPath, SceneHandler.Instance.Packing_Level);
//             SceneHandler.Instance.SwitchToPacking(true);
//         }

//         ResetTrain();
//         isEngineMoving = false;
//         isCarriageMoving = false;
//     }

    
//  IEnumerator FollowEngine()
//     {
//         Carriage.gameObject.SetActive(true);
//         // Initial last position and rotation.
//         Vector3 lastPosition = Engine.position;
//         Quaternion lastRotation = Engine.rotation;

//         while (isEngineMoving && isCarriageMoving || pathPositions.Count > 0)
//         {
//             if (pathPositions.Count > 0 && pathRotations.Count > 0)
//             {
//                 Vector3 targetPosition = pathPositions.Peek();
//                 Quaternion targetRotation = pathRotations.Peek();

//                 float distanceToEngine = Vector3.Distance(Carriage.position, targetPosition);

//                 // If the carriage is within a certain distance, align it directly behind the engine.
//                 if (distanceToEngine < minDistanceToEngine)
//                 {
//                     Carriage.position = targetPosition;
//                     Carriage.rotation = targetRotation;
//                     lastPosition = pathPositions.Dequeue();
//                     lastRotation = pathRotations.Dequeue();
//                 }
//                 else
//                 {
//                     Carriage.position = Vector3.MoveTowards(Carriage.position, targetPosition, movementSpeed * Time.deltaTime);
//                     Carriage.rotation = Quaternion.Slerp(Carriage.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//                 }
//             }
//             else
//             {
//                 // If the engine has stopped, align the carriage with the engine's final position and rotation.
//                 Carriage.position = Vector3.MoveTowards(Carriage.position, lastPosition, movementSpeed * Time.deltaTime);
//                 Carriage.rotation = Quaternion.Slerp(Carriage.rotation, lastRotation, Time.deltaTime * rotationSpeed);
//             }

//             yield return null; // Wait for the next frame.
//         }
//         Carriage.gameObject.SetActive(false);

//         // Ensure the carriage stops at the exact position and rotation of the engine.
//         Carriage.position = lastPosition;
//         Carriage.rotation = lastRotation;

//     }
// }
// //----