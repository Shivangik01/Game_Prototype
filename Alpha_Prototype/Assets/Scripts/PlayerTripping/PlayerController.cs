

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;

    public Transform Engine; // The train's engine part that you want to move and rotate.
    public Transform Carriage;

    // Start the movement along the path.
    public void MoveAlongPath()
    {
        StopAllCoroutines();
        StartCoroutine(TraverseaPath());
    }

    // Coroutine for moving along a path.
    IEnumerator TraverseaPath()
    {
        int index = 0;
        List<Vector2> traversalPath = PlottingManager.Instance.getTraversalPath();
        Engine.position = new Vector3(traversalPath[0].x, Engine.position.y, traversalPath[0].y); // Set initial position.

        // Set initial rotation for the Engine.
        Engine.rotation = Quaternion.Euler(-90, 0, 0);

        while (true)
        {
            if (index >= traversalPath.Count)
            {
                break; // Path completed, exit loop.
            }

            Vector3 targetPoint = new Vector3(traversalPath[index].x, Engine.position.y, traversalPath[index].y);

            if (Vector3.Distance(Engine.position, targetPoint) < 0.1f)
            {
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
                // This compensates for the model's forward direction being opposite to Unity's standard.
                toRotation *= Quaternion.Euler(0, 180, 0); // Flip the forward direction.

                // Extract the Euler angles and preserve the current X rotation of -90 degrees.
                Vector3 rotationAngles = toRotation.eulerAngles;
                rotationAngles.x = -90; // Keep the X rotation locked at -90 degrees.

                // Create a new Quaternion rotation from the modified Euler angles.
                toRotation = Quaternion.Euler(rotationAngles);

                // Smoothly rotate the Engine towards the calculated rotation.
                Engine.rotation = Quaternion.Slerp(Engine.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            // Move the Engine towards the target point.
            Engine.position = Vector3.MoveTowards(Engine.position, targetPoint, movementSpeed * Time.deltaTime);

            yield return null; // Proceed to the next frame before continuing the loop.
        }
    }
}
