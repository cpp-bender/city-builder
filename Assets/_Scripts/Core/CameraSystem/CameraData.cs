using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "City Builder/Camera System", fileName = "Camera Settings")]
public class CameraData : ScriptableObject
{
    public CameraSystem cameraSystem;

    public void SetCamera()
    {
        var mainCam = Camera.main;

        mainCam.transform.DOMove(cameraSystem.worldPos, 1f);
        mainCam.transform.DORotate(cameraSystem.rotation, 1f);
    }
}
