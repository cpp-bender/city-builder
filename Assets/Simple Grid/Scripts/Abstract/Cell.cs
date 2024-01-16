using UnityEngine;
using System;

[Serializable]
public class Cell
{
    public Vector3 worldPos;
    public SlotType cellSlot;

    public Cell()
    {

    }

    public Cell(Vector3 worldPos)
    {
        this.worldPos = worldPos;
    }

    public Cell SetSlotType(SlotType slotType)
    {
        this.cellSlot = slotType;
        return this;
    }
}
