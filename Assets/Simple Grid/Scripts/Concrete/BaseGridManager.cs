using System.Collections.Generic;
using UnityEngine;

public class BaseGridManager : MonoBehaviour
{
    public List<Cell> cells;

    /// <summary>
    /// Initialize grid according to the given grid settings
    /// </summary>
    /// <param name="gridSettings"></param>
    public void InitGrid(BaseGridSettings gridSettings)
    {
        GenerateQuadralXZGrid((QuadralGridSettings)gridSettings);
    }

    protected void GenerateQuadralXZGrid(QuadralGridSettings gridSettings)
    {
        var gridParent = new GameObject(gridSettings.ParentName);

        int index = 0;
        int height = gridSettings.Height;
        int width = gridSettings.Width;

        float widthOffset = gridSettings.WidthOffset;
        float heightOffset = gridSettings.HeightOffset;

        Vector3 initialPos = gridSettings.InitialPos;

        GameObject gridPrefab = gridSettings.GridPrefab;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 worldPos = initialPos + new Vector3(x * widthOffset, 0f, z * heightOffset);
                var gridObj = Instantiate(gridPrefab, worldPos, Quaternion.identity);
                gridObj.name = gridSettings.ChildName + $"{index}";
                gridObj.transform.SetParent(gridParent.transform);
                var cell = new Cell(worldPos);
                cells.Add(cell);
                gridObj.GetComponent<Grid>().SetText(index);
                index++;
            }
        }
        gridParent.transform.SetParent(transform);
    }
}
