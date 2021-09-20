using UnityEngine;
using UnityEngine.Events;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(typeof(RobotMovement), out Component component))
        {
            RobotMovement robot = component as RobotMovement;
            robot.Stop = true;
            GameObject.FindGameObjectWithTag("Finish").transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
