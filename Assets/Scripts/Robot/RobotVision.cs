using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotVision : MonoBehaviour
{
    public bool[] _receptorStates;
    private Receptor[] _receptors;

    public bool[] ReceptorStates => _receptorStates;

    public void Reset()
    {
        _receptors = GetComponentsInChildren<Receptor>();
        _receptorStates = new bool[_receptors.Length];
    }

    public void CheckReceptors()
    {
        for (int i = 0; i < _receptors.Length; ++i)
        {
            _receptorStates[i] = _receptors[i].Active;
        }
    }
}
