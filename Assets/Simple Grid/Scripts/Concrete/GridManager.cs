using System.Collections.Generic;
using UnityEngine;

public class GridManager : BaseGridManager
{
    [Header("DEBUG")]
    public LayerMask cellMask;

    [Header("DEPENDENCIES")]
    public LevelManager levelManager;

    private const float RAYDISTANCE = 25f;

    private void Awake()
    {
        cellMask = LayerMask.GetMask(Layers.DRAGGABLE);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        foreach (var cell in cells)
        {
            Vector3 rayStartPos = cell.worldPos + Vector3.down * 2f;
            Vector3 rayEndPos = rayStartPos + Vector3.up * RAYDISTANCE;
            Vector3 rayDir = Vector3.up;

            Debug.DrawLine(rayStartPos, rayEndPos, Color.green);

            var ray = new Ray(rayStartPos, rayDir);

            RaycastHit[] hits = Physics.RaycastAll(ray, RAYDISTANCE, cellMask);

            if (hits.Length == 1)
            {
                Debug.DrawLine(rayStartPos, rayEndPos, Color.black);
            }

            if (hits.Length > 1)
            {
                Debug.DrawLine(rayStartPos, rayEndPos, Color.cyan);
            }
        }
#endif
    }

    public bool CanSnap()
    {
        foreach (var cell in GetCurrentCells())
        {
            Vector3 rayStartPos = cell.worldPos + Vector3.down * 2f;
            Vector3 rayEndPos = rayStartPos + Vector3.up * RAYDISTANCE;
            Vector3 rayDir = Vector3.up;

            Debug.DrawLine(rayStartPos, rayEndPos, Color.green);

            var ray = new Ray(rayStartPos, rayDir);

            RaycastHit[] hits = Physics.RaycastAll(ray, RAYDISTANCE, cellMask);

            if (hits.Length > 1)
            {
                Debug.DrawLine(rayStartPos, rayEndPos, Color.cyan);
                return false;
            }
        }
        return true;
    }

    public void CheckGameOver()
    {
        var colliders = new List<Collider>();

        foreach (var cell in GetCurrentCells())
        {
            Vector3 rayStartPos = cell.worldPos + Vector3.down * 2f;
            Vector3 rayEndPos = rayStartPos + Vector3.up * RAYDISTANCE;
            Vector3 rayDir = Vector3.up;

            Debug.DrawLine(rayStartPos, rayEndPos, Color.green);

            var ray = new Ray(rayStartPos, rayDir);

            RaycastHit[] hits = Physics.RaycastAll(ray, RAYDISTANCE, cellMask);

            if (hits.Length > 0)
            {
                if (!colliders.Contains(hits[0].collider))
                {
                    colliders.Add(hits[0].collider);
                }
            }
        }

        if (colliders.Count == levelManager.GetCurrentInventoryCount())
        {
            if (levelManager.HasStage())
            {
                LevelManager.Instance.CloseOldGridDraggables();
                levelManager.IterateGrid();
                levelManager.MoveCamToNextStage();
            }
            else
            {
                LevelManager.Instance.MoveCamAcrossLevel();
                LevelManager.Instance.TurnOffDeck(LevelManager.Instance.gridIndex);
                GameManager.instance.LevelComplete();
                Confetti.Instance.PlayConfettis();
            }
        }
    }

    public Vector3 GetNearestCellPos(Vector3 worldPos, float snapThreshold, out bool canSnap)
    {
        Vector3 snapPos = default(Vector3);

        var currentCells = GetCurrentCells();

        canSnap = false;

        for (int i = 0; i < currentCells.Count; i++)
        {

            if (currentCells[i].cellSlot == SlotType.Placeable)
            {
                worldPos = new Vector3(worldPos.x, 0f, worldPos.z);

                var dist = Vector3.Distance(worldPos, currentCells[i].worldPos);
                //Debug.Log(i + ":" + dist);
                if (dist < snapThreshold)
                {

                    canSnap = true;
                    snapPos = currentCells[i].worldPos;
                }
            }
        }
        return snapPos;
    }

    public BaseGridSettings GetCurrentGridSettings()
    {
        return LevelManager.Instance.currentGrid;
    }

    private List<Cell> GetCurrentCells()
    {
        var cells = new List<Cell>();

        int startIndex = 0;
        var gridIndex = LevelManager.Instance.gridIndex;

        for (int i = 1; i <= gridIndex; i++)
        {
            startIndex += levelManager.currentLevel.GridSettings[i - 1].Width * levelManager.currentLevel.GridSettings[i - 1].Height;
        }

        var currentGrid = LevelManager.Instance.currentLevel.GridSettings[gridIndex];

        var endIndex = startIndex + currentGrid.Width * currentGrid.Height;

        //Debug.Log(startIndex + "-" + endIndex);

        for (int i = startIndex; i < endIndex; i++)
        {
            cells.Add(this.cells[i]);
        }

        return cells;
    }

    public List<Cell> GetCellsByIndex(int index)
    {
        var cells = new List<Cell>();

        int startIndex = 0;

        for (int i = 1; i <= index; i++)
        {
            startIndex += levelManager.currentLevel.GridSettings[i - 1].Width * levelManager.currentLevel.GridSettings[i - 1].Height;
        }

        var currentGrid = LevelManager.Instance.currentLevel.GridSettings[index];

        var endIndex = startIndex + currentGrid.Width * currentGrid.Height;

        for (int i = startIndex; i < endIndex; i++)
        {
            cells.Add(this.cells[i]);
        }

        return cells;
    }
}
