using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [Header("DEPENDENCIES"), Space(2f)]
    public GridManager gridManager;
    public List<LevelData> levels;
    public List<LevelData> hardLevels;

    [Header("DEBUG"), Space(2f)]
    public LevelData currentLevel;
    public BaseGridSettings currentGrid;
    public int inventoryCount;
    public GameObject decksParent;

    [HideInInspector]
    public int levelIndex;

    [HideInInspector]
    public int gridIndex;

    private List<GameObject> decks = new List<GameObject>();
    public List<List<Selectable>> allSelectables = new List<List<Selectable>>();
    private Camera cam;

    public GameObject deck;
    public Deck deckScript;

    protected override void Awake()
    {
        base.Awake();
        cam = Camera.main;
    }

    private void Start()
    {
        InitLevel();
    }

    private void InitLevel()
    {
        if (GameManager.instance.currentLevel <= 10)
        {
            currentLevel = levels[(GameManager.instance.currentLevel - 1) % levels.Count];
        }
        else
        {
            currentLevel = hardLevels[(GameManager.instance.currentLevel - 1) % hardLevels.Count];
        }


        currentGrid = currentLevel.GridSettings[gridIndex];

        InitCam();

        // MoveCamToFirstStage();

        InitAllGrids();

        InitSlotTypes();

        InitMap();

        inventoryCount = GetCurrentInventoryCount();

        // InitDecks();

        InitSelectables();
    }

    private void InitAllGrids()
    {
        for (int i = 0; i < currentLevel.GridSettings.Count; i++)
        {
            gridManager.InitGrid(currentLevel.GridSettings[i]);
        }
    }

    private void InitSlotTypes()
    {
        for (int i = 0; i < currentLevel.SlotTypes.Count; i++)
        {
            var cells = gridManager.GetCellsByIndex(i);

            for (int j = 0; j < currentLevel.SlotTypes[i].slotTypes.Count; j++)
            {
                cells[j].cellSlot = currentLevel.SlotTypes[i].slotTypes[j];
            }
        }
    }

    private void InitSelectables()
    {
        List<Selectable> selectables;
        for (int i = 0; i < currentLevel.Selectables.Count; i++)
        {
            selectables = new List<Selectable>();
            int selectableCountForStage = currentLevel.Selectables[i].selectableSettings.Count;
            deckScript = deck.GetComponent<Deck>();
            deckScript.FindPoints(selectableCountForStage);
            for (int j = 0; j < selectableCountForStage; j++)
            {
                var obj = currentLevel.Selectables[i].selectableSettings[j].selectable;
                var pos = currentLevel.Selectables[i].selectableSettings[j].initPos;
                var rot = currentLevel.Selectables[i].selectableSettings[j].initRot;
                var count = currentLevel.Selectables[i].selectableSettings[j].count;
                var scale = currentLevel.Selectables[i].selectableSettings[j].initScale;

                // Selectable selectable = Instantiate(obj, decksParent.transform);
                // selectable.SetCount(count).SetPosition(pos).SetRotation(rot).SetScale(scale);
                Selectable selectable = Instantiate(obj);

                Transform targetPoint = deckScript.activeDeckPoints[j];


                selectable.transform.parent = targetPoint;
                selectable.SetRotation(rot, deck.transform.forward, deck.transform.up * -1f);
                selectable.gameObject.AddComponent<MeshCenter>().FindMeshCenter(targetPoint);
                Vector3 localPos = selectable.transform.localPosition;
                localPos.z = 0f;
                selectable.transform.localPosition = localPos;

                selectable.SetCount(count);
                selectable.InitInventoryText(deckScript.FindText(j)); // Bu method zamani geldiginde call edilecek



                selectable.gameObject.SetActive(false);

                selectables.Add(selectable);
            }

            allSelectables.Add(selectables);
        }
    }

    public int GetCurrentInventoryCount()
    {
        int count = 0;

        for (int j = 0; j < currentLevel.Selectables[gridIndex].selectableSettings.Count; j++)
        {
            count += currentLevel.Selectables[gridIndex].selectableSettings[j].count;
        }

        return count;
    }

    private void InitDecks()
    {
        decksParent = new GameObject("Decks");

        for (int i = 0; i < currentLevel.Decks.Count; i++)
        {
            var obj = currentLevel.DeckPrefab;
            var pos = currentLevel.Decks[i].initPos;
            var rot = currentLevel.Decks[i].initRot;
            var scale = currentLevel.Decks[i].initScale;

            Deck deck = Instantiate(obj);

            deck.SetPosition(pos).SetRotation(rot).SetScale(scale);

            deck.gameObject.SetActive(false);
            decks.Add(deck.gameObject);
        }

    }

    private void InitMap()
    {
        Instantiate(currentLevel.MapPrefab);
    }

    public void IterateGrid()
    {
        currentGrid = currentLevel.GridSettings[++gridIndex];
        inventoryCount = GetCurrentInventoryCount();
    }

    public bool HasStage()
    {
        var lastGridIndex = currentLevel.GridSettings.Count;

        if (gridIndex == lastGridIndex - 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void InitCam()
    {
        Camera.main.transform.position = currentLevel.InitCamPos;
        Camera.main.transform.rotation = Quaternion.Euler(currentLevel.InitCamRot);
    }

    public void MoveCamToFirstStage()
    {
        Vector3 nextCamPos = currentLevel.CamSettings[gridIndex].pos;
        Vector3 nexCamRot = currentLevel.CamSettings[gridIndex].rot;

        var camSequence = DOTween.Sequence();

        Tween moveTween = cam.transform.DOMove(nextCamPos, .75f);
        Tween rotateTween = cam.transform.DORotate(nexCamRot, .75f);

        camSequence.Append(moveTween).Join(rotateTween)
            .OnComplete(() =>
            {
                deckScript.DeckSwitch(true);
                SelectablesSwitch(gridIndex);
                GridHighlighter.Instance.HighLightStageGrasses(gridIndex);
            })
            .Play();
    }

    public void MoveCamToNextStage()
    {
        Vector3 nextCamPos = currentLevel.CamSettings[gridIndex].pos;
        Vector3 nexCamRot = currentLevel.CamSettings[gridIndex].rot;

        var camSequence = DOTween.Sequence();

        Tween moveTween = Camera.main.transform.DOMove(nextCamPos, .5f);
        Tween rotateTween = Camera.main.transform.DORotate(nexCamRot, .5f);

        camSequence.Append(moveTween).Append(rotateTween)
            .SetDelay(1f)
            .OnStart(delegate
            {
                for (int i = 0; i < decks.Count; i++)
                {
                    decks[i].gameObject.SetActive(false);
                }
            })
            .OnComplete(() =>
            {
                // DescksSwitch(gridIndex); 
                SelectablesSwitch(gridIndex);
                GridHighlighter.Instance.HighLightStageGrasses(gridIndex);
            })
            .Play();
    }

    public void CloseOldGridDraggables()
    {
        var selectables = allSelectables[gridIndex];

        foreach (var selectable in selectables)
        {
            selectable.CloseDraggables();
        }
    }

    public void TurnOffDeck(int gridIndex)
    {
        // decks[gridIndex].gameObject.SetActive(false);
        deckScript.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutQuad).Play();
    }

    public void DescksSwitch(int index)
    {

        foreach (var deck in decks)
        {
            if (deck.gameObject.activeSelf)
                deck.gameObject.SetActive(false);
        }

        decks[gridIndex].gameObject.SetActive(true);
    }

    public void SelectablesSwitch(int index)
    {
        foreach (var selectables in allSelectables)
        {
            foreach (var selectable in selectables)
            {
                if (selectable.gameObject.activeSelf)
                    selectable.gameObject.SetActive(false);
            }
        }

        var tempSelectables = allSelectables[gridIndex];
        foreach (var tempSelectable in tempSelectables)
        {
            deckScript.SwitchCanvas(tempSelectables.Count);
            tempSelectable.gameObject.SetActive(true);
        }
    }

    public void MoveCamAcrossLevel()
    {
        if (!currentLevel.MoveAcrossLevel)
        {
            return;
        }

        var camSequence = DOTween.Sequence();


        Tween moveTween = cam.transform.DOMove(currentLevel.LastCamPos, 1f);
        Tween rotateTween = cam.transform.DORotate(currentLevel.LastCamRot, 1f);

        camSequence.Append(moveTween).Join(rotateTween).Play();

        //for (int i = 0; i < currentLevel.CamSettings.Count; i++)
        //{
        //    var pos = currentLevel.CamSettings[i].pos;
        //    var rot = currentLevel.CamSettings[i].rot;

        //    Tween moveTween = cam.transform.DOMove(pos, 2f).SetDelay(2f);
        //    Tween rotateTween = cam.transform.DORotate(rot, 2f);

        //    camSequence.Append(moveTween).Join(rotateTween);
        //}

        //camSequence.SetDelay(1f).SetLoops(-1, LoopType.Restart).Play();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelManager))]
public class SomeScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Skip Level"))
        {
            if (LevelManager.Instance.HasStage())
            {
                LevelManager.Instance.CloseOldGridDraggables();
                LevelManager.Instance.IterateGrid();
                LevelManager.Instance.MoveCamToNextStage();
            }
            else
            {
                GameManager.instance.LevelComplete();
            }
        }
    }
}
#endif
