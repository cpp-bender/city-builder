using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCenter : MonoBehaviour
{
    private float maxVolumeX = 0.3f;
    void Start()
    {
        // FindMeshCenter();
    }
    public void FindMeshCenter(Transform target)
    {
        Renderer[] meshes = GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer mesh in meshes)
        {
            bounds.Encapsulate(mesh.bounds);
        }

        Vector3 offsetForCenterAndPivot = bounds.center - transform.position;
        transform.position = target.position - offsetForCenterAndPivot;

        float boundRatio = maxVolumeX / bounds.size.x;
        transform.localScale *= boundRatio;

        Vector3 offsetForScaleChange = transform.position - target.position;
        transform.position = transform.position - offsetForScaleChange;
        
        transform.position -= offsetForCenterAndPivot * boundRatio;
        // transform.position += (offsetForCenterAndPivot * boundRatio).y * Vector3.up;
    }
}
