using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemIcon : MonoBehaviour
{
    private bool letFlyGem;
    private float timerFlyGem;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 startScale;
    private Vector3 endScale;
    private float speed;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        FlyingGem();
    }

    public void FlyGem(int gemIndex, float flySpeed, float scale)
    {
        letFlyGem = true;
        timerFlyGem = 0f;
        speed = flySpeed;
        startPos = transform.position;
        startScale = transform.localScale;
        endScale = startScale * scale;
        endPos = UIManager.instance.gemIcons[gemIndex].transform.position;
    }
    
    private void FlyingGem()
    {
        if (letFlyGem)
        {
            timerFlyGem += Time.deltaTime * speed;

            transform.position = Vector3.Lerp(startPos, endPos, timerFlyGem);
            transform.localScale = Vector3.Lerp(startScale, endScale, timerFlyGem);

            if (timerFlyGem >= 1f)
            {
                UIManager.instance.RefreshGemCountInLevel();
                letFlyGem = false;
                gameObject.SetActive(false);
            }
        }
    }
}
