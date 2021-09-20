using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class RobotMovement : MonoBehaviour
{
    public bool Stop = false;
    [SerializeField] private UnityEvent _endMovment;

    private Vector3 _forwardVector = new Vector3(0, 0, 0);
    private Vector3 _rotateVector = new Vector3(0, 0, 0);
    private int _movemetsCount = 0;

    private Tween _tween;

    private List<Tween> _tweens = new List<Tween>();

    private void EndMovment()
    {
        if (Stop == false)
            _endMovment?.Invoke();
    }

    public void RotateOnLeft()
    {
        _rotateVector += new Vector3(0, -90, 0);
        _tween = transform.DORotate(_rotateVector, 0.2f, RotateMode.Fast);
        _tween.OnComplete(EndMovment);
    }

    public void RotateOnRight()
    {
        _rotateVector -= new Vector3(0, -90, 0);
        _tween = transform.DORotate(_rotateVector, 0.2f, RotateMode.Fast);
        _tween.OnComplete(EndMovment);
    }

    public void RotateOnRightAndMoveForward()
    {
        _rotateVector -= new Vector3(0, -90, 0);
        _tween = transform.DORotate(_rotateVector, 0.2f, RotateMode.Fast);
        _tween.OnComplete(MoveForward);
    }

    public void MoveForward()
    {
        _forwardVector += transform.right * 12;
        _tween = transform.DOMove(new Vector3(_forwardVector.x, transform.position.y, _forwardVector.z), 0.5f);
        _tween.OnComplete(EndMovment);
    }

    private void Start()
    {

        //MoveForward();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndMovment();
            //  Debug.Log("1");
            // MoveForward();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("2");
            RotateOnLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("3");
            RotateOnRight();
        }
    }
}
