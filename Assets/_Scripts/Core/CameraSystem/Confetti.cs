using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confetti : SingletonMonoBehaviour<Confetti>
{
    public GameObject[] confettis;

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        
    }

    public void PlayConfettis()
    {
        for (int i = 0; i < confettis.Length; i++)
        {
            confettis[i].SetActive(true);
        }
    }
    
}
