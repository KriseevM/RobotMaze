using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotVision : MonoBehaviour
{
    [SerializeField] private Receptor _rightReceptor, _forwardReceptor;
    [SerializeField] private UnityEvent _rightReceptorNotActive, _forwardReceptorNotActive, _allReceptorsActive;

    public void CheckReceptors()
    {
        if (_rightReceptor.Active == false)
        {
            Debug.Log("Right");
            _rightReceptorNotActive?.Invoke();
        }
        else if (_forwardReceptor.Active == false)
        {
            Debug.Log("Forward");
            _forwardReceptorNotActive?.Invoke();
        }
        else
        {
            Debug.Log("All");
            _allReceptorsActive?.Invoke();
        }
    }
}
