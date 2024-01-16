using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DummyElephant : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForLevelPass());
    }

    IEnumerator WaitForLevelPass()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
