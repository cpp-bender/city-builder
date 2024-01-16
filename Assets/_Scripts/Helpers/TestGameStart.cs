using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameStart : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        StartMove();
    }
    
    private void StartMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<CharacterMovement>().enabled = true;
            GetComponent<Animator>().SetTrigger("Running");
        }
    }
}
