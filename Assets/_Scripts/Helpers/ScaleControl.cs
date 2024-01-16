using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    public bool isActive = true;
    public float scaleValue = 1f;

    private void Start()
    {
        
    }

    private void OnValidate()
    {
        if (isActive)
        {
            gameObject.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
    }
}