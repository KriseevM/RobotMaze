using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField]
    private RobotMovement _movement;
    private void Reset()
    {
        _movement = GetComponent<RobotMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _movement.MoveForward();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _movement.RotateOnRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _movement.RotateOnLeft();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}
