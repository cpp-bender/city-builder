using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 0.25f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Moving();
    }

    public void Moving()
    {
        if (GameManager.instance.isLevelStarted && !(GameManager.instance.isLevelCompleted || GameManager.instance.isLevelFailed))
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            rb.MovePosition(rb.transform.position + transform.forward * vertical * speed + transform.right * horizontal * speed);
        }
    }
}
