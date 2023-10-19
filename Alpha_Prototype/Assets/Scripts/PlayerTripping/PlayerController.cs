using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    //List<DeliverManager> deliveries;

    public void MoveAlongPath()
    {
        StopAllCoroutines();
        StartCoroutine(TraverseaPath());
    }

    IEnumerator TraverseaPath()
    {
        //int index = 0;
        //List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();
        //transform.position = new Vector3(traversalPath[0].x, transform.position.y, traversalPath[0].y);
        //Vector3 endPos = new Vector3(traversalPath[traversalPath.Count - 1].x, transform.position.y, traversalPath[traversalPath.Count - 1].y);
        //while (true)
        //{
        //    // Check if the player has reached the current target point
        //    Vector3 targetPoint = new Vector3(traversalPath[index].x, transform.position.y, traversalPath[index].y);
        //    if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        //    {
        //        //if delivery is to be made Wait for 2 seconds
        //        // Move to the next point in the path
        //        index++;

        //        // If reached the end of the path, reset the index to loop the movement
        //        if (index >= traversalPath.Count)
        //        {
        //            index = 0;
        //            break;
        //            //isMoving = false; // Stop moving when the path is completed
        //        }
        //    }

        //    // Move towards the current target point
        //    transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);
        //    yield return null;
        //}

        int index = 0;
        List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();

        // Set initial position at the starting point
        transform.position = new Vector3(traversalPath[0].x, transform.position.y, traversalPath[0].y);

        while (true)
        {
            if (index >= traversalPath.Count)
            {
                // End of the path reached, exit the loop
                break;
            }

            // Calculate the current target point in the path
            Vector3 targetPoint = new Vector3(traversalPath[index].x, transform.position.y, traversalPath[index].y);

            // Check if the player has reached the current target point
            if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
            {
                // Reached the current point, move to the next point in the path
                index++;
                continue;  // Skip the rest of the loop to avoid "MoveTowards" the same point.
            }

            // Calculate the direction to the target point
            Vector3 directionToNextPoint = (targetPoint - transform.position).normalized;

            // If you need the player to move along the vertical axis (y), remove the '.y' assignment below
            directionToNextPoint.y = 0;  // Keep the direction strictly horizontal

            // Rotate the player to face the target direction
            if (directionToNextPoint != Vector3.zero)  // Prevents the character from looking in the 'zero' direction, which can cause issues.
            {
                Quaternion toRotation = Quaternion.LookRotation(directionToNextPoint, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            // Move the player towards the target point
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, movementSpeed * Time.deltaTime);

            yield return null; // Wait until next frame
        }

        //transform.position = endPos;
    }
}