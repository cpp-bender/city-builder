using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class Deck : MonoBehaviour
{
    [Header("INITIAL SETTINGS")]
    public Vector3 initPos;
    public Vector3 initRot;
    public Vector3 initScale;

    public Transform[] activeDeckPoints;
    public List<DeckPoints> allDeckPoints = new List<DeckPoints>();

    public void FindPoints(int selectableCount)
    {
        foreach (var deck in allDeckPoints)
        {
            if(deck.deckPoints.Count == selectableCount)
            {
                activeDeckPoints = deck.deckPoints.ToArray();
            }
        }
    }

    public void SwitchCanvas(int selectableCount)
    {
        foreach (var deck in allDeckPoints)
        {
            if (deck.deckCanvas.Count == selectableCount)
            {
                foreach (var canvas in deck.deckCanvas)
                {
                    canvas.SetActive(true);
                }
            }
            else
            {
                foreach (var canvas in deck.deckCanvas)
                {
                    canvas.SetActive(false);
                }
            }
        }
    }

    public void DeckSwitch(bool switchDeck)
    {
        for (int i = 0; i < transform.childCount; i++) //Stage'e gore aktif
        {
            transform.GetChild(i).gameObject.SetActive(switchDeck);
        }

        GetComponent<MeshRenderer>().enabled = switchDeck;
    }

    public TextMeshProUGUI FindText(int activePointsIndex)
    {
        TextMeshProUGUI text = null;
        var activeTransform =  activeDeckPoints[activePointsIndex];

        foreach (var deckPoints in allDeckPoints)
        {
            if(deckPoints.deckPoints.Contains(activeTransform))
            {
                int activeIndex = deckPoints.deckPoints.IndexOf(activeTransform);
                text = deckPoints.deckCanvas[activeIndex].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        return text;
    }

    #region CHAINING METHODS
    public Deck SetPosition(Vector3 pos)
    {
        this.initPos = pos;
        transform.position = initPos;
        return this;
    }

    public Deck SetRotation(Vector3 rot)
    {
        this.initRot = rot;
        transform.rotation = Quaternion.Euler(rot);
        return this;
    }

    public Deck SetScale(Vector3 scale)
    {
        this.initScale = scale;
        transform.localScale = scale;
        return this;
    }
    #endregion
}

[Serializable]
public class DeckPoints
{
    public List<Transform> deckPoints;
    public List<GameObject> deckCanvas;
}
