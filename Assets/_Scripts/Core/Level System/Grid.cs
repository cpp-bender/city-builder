using UnityEngine;
using TMPro;

public class Grid : MonoBehaviour
{
    [Header("COMPONENTS")]
    public TextMeshProUGUI textMesh;

    public void SetText(int count)
    {
        textMesh.text = count.ToString();
    }
}
