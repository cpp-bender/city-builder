using UnityEngine;
using System;

[Serializable]
public class SelectableSettings
{
    public Selectable selectable;
    public Vector3 initPos;
    public Vector3 initRot;
    public float initScale = 1f;
    public int count;
}