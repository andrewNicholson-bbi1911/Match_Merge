using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLook : MonoBehaviour
{
    private Transform _transform;
    private Quaternion _startQuaternion;
    private void Awake()
    {
        _transform = transform;
        _startQuaternion = _transform.rotation;
    }
    void Update()
    {
        _transform.rotation = _startQuaternion;
    }
}
