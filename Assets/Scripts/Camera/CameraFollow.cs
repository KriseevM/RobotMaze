using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject _followed;
    private Camera _camera;
    private Vector3 _delta = new Vector3(0, 0, 0);

    private void FollowTo(GameObject followed)
    {
        if (followed == null)
            return;

        if (followed.activeInHierarchy == false)
        {
            followed = null;
            return;
        }

        Vector3 xyzPosition = Vector3.Lerp(transform.position, followed.transform.position + _delta, Time.deltaTime);
        transform.position = new Vector3(xyzPosition.x, transform.position.y, xyzPosition.z);
    }

    private void Start()
    {
        _camera = GetComponent(typeof(Camera)) as Camera;
        _delta = _camera.transform.position - _followed.transform.position;
    }

    private void Update()
    {
        FollowTo(_followed);
    }
}
