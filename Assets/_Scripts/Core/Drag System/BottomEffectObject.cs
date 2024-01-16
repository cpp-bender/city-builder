using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomEffectObject : MonoBehaviour
{
    [SerializeField] MeshRenderer render;
    [SerializeField] Material rightMaterial;
    [SerializeField] Material wrongMaterial;


    public void TransformSetting(Vector3 localPosition, Vector3 localScale)
    {
        transform.localPosition = localPosition;
        transform.localScale = localScale;
    }

    public void SwitchMaterial(bool value)
    {
        render.material = value ? rightMaterial : wrongMaterial;
    }
}
