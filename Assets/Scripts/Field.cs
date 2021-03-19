using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] internal List<ChangebleAbst> _ChangeblesObj;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    
    void Update()
    {
        
    }

    internal void AddChangebleObj(ChangebleAbst obj)
    {
        _ChangeblesObj.Add(obj);
    }
}
