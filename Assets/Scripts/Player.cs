using UnityEngine;

public class Player : MonoBehaviour
{
    private float rayDistance;
    private bool canJump;

    [SerializeField]
    private float speed;

    private float moveHorizontal;
    private float moveVertical;

    private InputData inputData;
    private InputRecorder inputRecorder;


    public void Movement()
    {
        // Get current velocity for rigidbody
        Vector3 currentVel = GetComponent<Rigidbody>().velocity;
        Vector3 newVel;


        canJump = CanPlayerJump();
        inputRecorder.ResolveInput(canJump);

        if (canJump)
        {
            ResolveInput();
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            newVel = new Vector3(movement.x * speed, currentVel.y, movement.z * speed);
        }
        else
        {
            newVel =  new Vector3(currentVel.x, currentVel.y, currentVel.z);
        }

        ResolveMovement(newVel);
    }

    private void ResolveMovement(Vector3 vel)
    {
        GetComponent<Rigidbody>().velocity = vel;
    }

    private bool CanPlayerJump()
    {
        // Can jump?
        rayDistance = GetComponent<Collider>().bounds.extents.y + 0.1f;
        Ray ray = new Ray();
        ray.origin = GetComponent<Collider>().bounds.center;
        ray.direction = Vector3.down;

        if (Physics.Raycast(ray, rayDistance))
            return true;

        return false;
    }
}

