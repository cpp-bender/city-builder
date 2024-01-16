using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City Builder/Level", fileName = "Level")]
public class LevelData : ScriptableObject
{
    [Header("GRID SETTINGS"), Space(2f)]
    [SerializeField] List<BaseGridSettings> gridSettings;

    [Header("SLOT TYPE SETTINGS")]
    [SerializeField] List<SlotTypeControl> slotTypes;

    [Header("Cam Settings")]
    [SerializeField] Vector3 initCamPos;
    [SerializeField] Vector3 initCamRot;
    [SerializeField] Vector3 lastCamPos;
    [SerializeField] Vector3 lastCamRot;
    [SerializeField] bool moveAcrossLevel = true;
    [SerializeField] List<CameraSettings> camSettings;

    [Header("MAP SETTINGS"), Space(2f)]
    [SerializeField] GameObject mapPrefab;

    [Header("SELECTABLE SETTINGS")]
    [SerializeField] List<SelectableData> selectables;

    [Header("DECK SETTINGS")]
    [SerializeField] Deck deckPrefab;
    [SerializeField] List<DeckData> decks;

    public List<BaseGridSettings> GridSettings { get => gridSettings; }
    public List<SelectableData> Selectables { get => selectables; }
    public List<DeckData> Decks { get => decks; }
    public Deck DeckPrefab { get => deckPrefab; }
    public GameObject MapPrefab { get => mapPrefab; }
    public List<SlotTypeControl> SlotTypes { get => slotTypes; }
    public List<CameraSettings> CamSettings { get => camSettings; }
    public Vector3 InitCamPos { get => initCamPos; }
    public Vector3 InitCamRot { get => initCamRot; }
    public bool MoveAcrossLevel { get => moveAcrossLevel; }
    public Vector3 LastCamPos { get => lastCamPos; set => lastCamPos = value; }
    public Vector3 LastCamRot { get => lastCamRot; set => lastCamRot = value; }
}
