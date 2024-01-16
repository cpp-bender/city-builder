using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraFollow : MonoBehaviour
{
    public GameObject target;
    private Vector3 offset;
    
    void Start()
    {
        offset = target.transform.position - transform.position;
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position - offset, Time.deltaTime * 10f);
    }
}
