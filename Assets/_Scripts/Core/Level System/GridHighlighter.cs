using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridHighlighter : SingletonMonoBehaviour<GridHighlighter>
{
    public Color color;
    public List<Grasses> levelGridGrasses;

    static readonly int shPorpColor = Shader.PropertyToID("_BaseColor");

    protected override void Awake()
    {
        base.Awake();
    }

    public void HighLightStageGrasses(int stageIndex)
    {
        var stageGrassesMeshes = levelGridGrasses[stageIndex].grassMeshList;

        foreach (var grassMesh in stageGrassesMeshes)
        {
            Color startColor = grassMesh.material.color;
            Sequence sequence = DOTween.Sequence();

            Tween loopTween = grassMesh.material.DOColor(color, shPorpColor, 0.5f).SetLoops(6, LoopType.Yoyo);
            Tween startColorTween = grassMesh.material.DOColor(startColor, shPorpColor, 0.5f);

            sequence.Append(loopTween).Append(startColorTween).Play();
        }
    }

}

[Serializable]
public class Grasses
{
    public List<MeshRenderer> grassMeshList;
}