using System;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Mathematics;

public class Selectable : MonoBehaviour, ISelectable
{
    [Header("INITIAL SETTINGS"), Space(2f)]
    public Vector3 initialPos;
    public Vector3 initialRot;
    public int count;

    [Header("DEPENDENCIES")]
    public Draggable draggablePrefab;

    private Draggable draggable;
    public List<Draggable> draggables = new List<Draggable>();
    
    private GameObject inventoryCount;
    private TextMeshProUGUI text;

    private void Start()
    {
        UpdateCountText();
    }

    public void Select()
    {
        CreateDraggable();

    }

    public void DeSelect()
    {

    }

    private void CreateDraggable()
    {
        if (count <= 0)
        {
            return;
        }

        UpdateCount(-1);

        draggable = Instantiate(draggablePrefab, transform.position, transform.rotation);
        draggable.selectable = this;
        draggable.transform.localScale = transform.localScale;
        // draggable.transform.eulerAngles = draggable.initRot;
        //draggable.transform.DORotate(draggable.initRot, 0.25f).Play();
        draggable.transform.DORotate(CalculateInitRot(), 0.25f).Play();
        draggable.transform.DOMoveY(draggable.posY, 0.25f).Play();
        draggable.transform.DOScale(Vector3.one, 0.25f).Play();
        draggable.transform.parent = null;

        draggable.SubscribeToSelectable();
    }

    public void UpdateCount(int value)
    {
        count += value;

        if (count == 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);

        UpdateCountText();
    }

    public void CloseDraggables()
    {
        foreach (var draggable in draggables)
        {
            draggable.CloseSelectableProperty();
        }
    }

    public void InitInventoryText(TextMeshProUGUI text)
    {
        this.text = text;

        UpdateCountText();
    }

    private void UpdateCountText()
    {
        String countStr = count.ToString() + "x";

        if (count <= 0)
        {
            countStr = "";
        }

        text.SetText(countStr);
    }

    private Vector3 CalculateInitRot()
    {
        var selectableYRotation = transform.eulerAngles.y;

        var remainder = selectableYRotation % 90f;
        var roundedRotation = selectableYRotation - remainder;

        return new Vector3(0f, roundedRotation, 0f);
    }

    #region CHAINING METHODS

    public Selectable SetCount(int count)
    {
        this.count = count;
        return this;
    }

    public Selectable SetPosition(Vector3 pos)
    {
        this.initialPos = pos;
        transform.position = initialPos;
        return this;
    }

    public Selectable SetRotation(Vector3 rot, Vector3 up, Vector3 forward)
    {
        this.initialRot = rot;
        // transform.localRotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        transform.up = up;
        transform.Rotate(rot, Space.Self);
        // transform.rotation = Quaternion.LookRotation(forward, up);
        return this;
    }

    public Selectable SetScale(float scale)
    {
        transform.localScale *= scale;
        return this;
    }
    #endregion
}
