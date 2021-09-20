using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receptor : MonoBehaviour
{
    public bool Active { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        Active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Active = false;
    }
}
