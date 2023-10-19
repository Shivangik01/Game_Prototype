using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    //List<DeliverManager> deliveries;

    public void MoveAlongPath()
    {
        StopAllCoroutines();
        StartCoroutine(TraverseaPath());
    }

    IEnumerator TraverseaPath()
    {
        int index = 0;
        List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();
        transform.position = new Vector3(traversalPath[0].x, transform.position.y, traversalPath[0].y);
        Vector3 endPos = new Vector3(traversalPath[traversalPath.Count - 1].x, transform.position.y, traversalPath[traversalPath.Count - 1].y);
        while (true)
        {
            // Check if the player has reached the current target point
            Vector3 targetPoint = new Vector3(traversalPath[index].x, transform.position.y, traversalPath[index].y);
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                //if delivery is to be made Wait for 2 seconds
                // Move to the next point in the path
                index++;

                // If reached the end of the path, reset the index to loop the movement
                if (index >= traversalPath.Count)
                {
                    index = 0;
                    break;
                    //isMoving = false; // Stop moving when the path is completed
                }
            }

            // Move towards the current target point
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos;
    }
}