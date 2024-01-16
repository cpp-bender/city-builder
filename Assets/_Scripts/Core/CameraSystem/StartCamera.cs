using UnityEngine;
using UnityEngine.UI;

public class StartCamera : MonoBehaviour
{
    [Header("CAMERA")]
    public Button button;

    [Header("Camera Datas")]
    public CameraData[] cameraDatas;

    private CameraData currentCameraData;
    private int cameraDataCount = 0;

    void Start()
    {
        InitCamera();
    }

    public void InitCamera()
    {
        currentCameraData = cameraDatas[0];
        currentCameraData.SetCamera();
    }

    public void ChangeCameraView()
    {
        //for (int i = 0; i < cameraDatas.Length; i++)
        //{
        //    cameraDatas[i].SetCamera();
        //    Debug.Log(i);
        //}
        cameraDataCount =  Mathf.Clamp(cameraDataCount, 0, cameraDatas.Length+1);
        cameraDataCount ++;
        cameraDatas[cameraDataCount].SetCamera();
    }
}
