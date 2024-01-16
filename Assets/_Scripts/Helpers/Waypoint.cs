using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Waypoint : MonoBehaviour
{
    public Image img;
    public Transform target;
    public Transform character;
    public TextMeshProUGUI meter;
    public Vector3 offset;

    private void Update()
    {
        Navigation();
    }

    void Navigation()
    {
        float minX = img.GetPixelAdjustedRect().width / 1.5f;
        float maxX = Screen.width - minX;

        float minY = img.GetPixelAdjustedRect().height / 1.5f;
        float maxY = Screen.height - minY;

        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        // if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        // {
        //     if (pos.x < Screen.width / 2)
        //     {
        //         pos.x = maxX;
        //     }
        //     else
        //     {
        //         pos.x = minX;
        //     }
        // }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        
        Vector3 rawPos = Camera.main.WorldToScreenPoint(target.position + offset);

        if (rawPos.z < 0f)
        {
            pos.x = Screen.width - pos.x;
            pos.y = minY;
        }

        img.transform.position = pos;

        int distance = (int)Vector3.Distance(target.position, character.transform.position);
        meter.text = (distance).ToString() + "m";
    }
}
