using DG.Tweening;
using DG.Tweening.Core.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ElephantSDK;

[DefaultExecutionOrder(-200)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public bool singleSceneForAllLevels;
    public int startLevelCountForLoop;
    public bool isSDKComplete;
    public bool isThisLoaderScene;
    
    public GameObject[] levels;
    [HideInInspector]
    public int levelCountOfSDK;

    private const string level = "level";
    private const string gem = "gem";

    public int currentLevel;
    public int gemCountInLevel;
    public int gemCountTotal;
    public int gemCountTotalTemp;
    public int gemMultiplier;
    [HideInInspector]
    public bool isMultiplied;

    public bool isLevelStarted;
    public bool isLevelCompleted;
    public bool isLevelFailed;
    
    private List<int> levelNumbers;
    private int[] randomLevels;
    private int totalLevelCount;
    public GameObject loaderPanel;

    void Awake()
    {
        CreateInstance();

        InitDOTween();

        CheckSDKCompletion();

        RandomizeLevels();
        
        AssignSaveLoadParameters();
        
        SelectLoadType();
    }
    void Start()
    {
        isLevelStarted = false;
        gemMultiplier = 1;
        isMultiplied = false;
        
        if (isSDKComplete)
        {
            //Debug.LogError("!!! Uncomment the line below after installing SDKs");
            Elephant.LevelStarted(currentLevel);
        }
    }
    private void CreateInstance()
    {
        // Create instance on awake
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void CheckSDKCompletion()
    {
        // Allocate first indexes in the build settings for SDK scenes
        if (isSDKComplete)
        {
            levelCountOfSDK = 2;
        }
        else
        {
            levelCountOfSDK = 0;
        }
    }
    private void RandomizeLevels()
    {
        // Decide total level count
        if (singleSceneForAllLevels)
        {
            totalLevelCount = levels.Length;
        }
        else
        {
            totalLevelCount = SceneManager.sceneCountInBuildSettings - levelCountOfSDK;
        }
        
        // Limit starting level count for the loop after main levels
        if (totalLevelCount <= startLevelCountForLoop)
        {
            startLevelCountForLoop =  Mathf.Clamp(totalLevelCount - 1, 1, 100);
            // Debug.LogError("Start level of the loop can't be the last level");
        }
        
        // Define necessary lists for randomization
        levelNumbers = new List<int>();

        if (singleSceneForAllLevels)
        {
            for (int i = startLevelCountForLoop; i < levels.Length + 1; i++)
            {
                levelNumbers.Add(i);
            }
            
            randomLevels = new int[levels.Length - startLevelCountForLoop + 1];
        }
        else
        {
            for (int i = startLevelCountForLoop; i < totalLevelCount + 1; i++)
            {
                levelNumbers.Add(i);
            }
            
            randomLevels = new int[totalLevelCount - startLevelCountForLoop + 1];
        }
        
        // Store current state of the RNG
        Random.State originalRandomState = Random.state;

        if (singleSceneForAllLevels)
        {
            // Use a specific seed to get the same outcome every time
            Random.InitState(levels.Length);
        }
        else
        {
            // Use a specific seed to get the same outcome every time
            Random.InitState(totalLevelCount);
        }

        // Randomize level list for after manin levels
        for (int i = 0; i < randomLevels.Length; i++)
        {
            int randomIndex = Random.Range(0, levelNumbers.Count);

            if (levelNumbers.Count > 1)
            {
                while (randomIndex == randomLevels.Length - 1)
                {
                    randomIndex = Random.Range(0, levelNumbers.Count);
                }
            }
            
            randomLevels[i] = levelNumbers[randomIndex];
            
            levelNumbers.RemoveAt(randomIndex);
        }
        
        // Return the RNG to how it was before
        Random.state = originalRandomState;
    }
    private void AssignSaveLoadParameters()
    {
        // Set current level and gem count numbers by loading saved data
        if (!PlayerPrefs.HasKey(level))
        {
            PlayerPrefs.SetInt(level, 1);
        }

        if (!PlayerPrefs.HasKey(gem))
        {
            PlayerPrefs.SetInt(gem, 0);
        }

        currentLevel = PlayerPrefs.GetInt(level);
        gemCountTotal = PlayerPrefs.GetInt(gem);
    }
    private void SelectLoadType()
    {
        // Decide load method based on level management system
        if (isThisLoaderScene)
        {
            loaderPanel.SetActive(true);
            LevelLoad();
        }
        else if (singleSceneForAllLevels)
        {
            SelectLevel();
        }
    }
    private void LevelLoad()
    {
        // Decide which scene to load
        if (currentLevel > totalLevelCount)
        {
            currentLevel = randomLevels[(currentLevel - (startLevelCountForLoop - 1) - 1) % randomLevels.Length];
        }

        if (singleSceneForAllLevels && !isThisLoaderScene)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else if (singleSceneForAllLevels && isThisLoaderScene)
        {
            SceneManager.LoadSceneAsync(levelCountOfSDK);
        }
        else
        {
            SceneManager.LoadSceneAsync(currentLevel + (levelCountOfSDK - 1));
        }
    }
    private void SelectLevel()
    {
        // Decide which level to load on the same scene
        if (currentLevel > totalLevelCount)
        {
            currentLevel = randomLevels[(currentLevel - (startLevelCountForLoop - 1) - 1) % randomLevels.Length];
        }
        
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }
            
        levels[currentLevel - 1].SetActive(true);
        
        AssignSaveLoadParameters();
    }
    public void LevelComplete()
    {
        if (!(isLevelCompleted || isLevelFailed))
        {
            if (isSDKComplete)
            {
                // Debug.LogError("!!! Uncomment the line below after installing SDKs");
                Elephant.LevelCompleted(currentLevel);
            }
            
            UIManager.instance.LevelCompleteForUI(1.5f);
            currentLevel++;
            gemCountTotalTemp = gemCountTotal;
            gemCountTotal += gemCountInLevel;
            Save();
            isLevelCompleted = true;
        }
    }
    public void LevelComplete(int multiplier)
    {
        if (!(isLevelCompleted || isLevelFailed))
        {
            if (isSDKComplete)
            {
                // Debug.LogError("!!! Uncomment the line below after installing SDKs");
                Elephant.LevelCompleted(currentLevel);
            }
            
            UIManager.instance.LevelCompleteForUI(0.5f);
            currentLevel++;
            gemMultiplier = multiplier;
            gemCountInLevel *= gemMultiplier;
            isMultiplied = true;
            UIManager.instance.multiplierText.GetComponent<TextMeshProUGUI>().text = gemMultiplier + "X";
            gemCountTotalTemp = gemCountTotal;
            gemCountTotal += gemCountInLevel;
            Save();
            isLevelCompleted = true;
        }
    }
    public void LevelFail()
    {
        if (!(isLevelFailed || isLevelCompleted))
        {
            if (isSDKComplete)
            {
                // Debug.LogError("!!! Uncomment the line below after installing SDKs");
                Elephant.LevelFailed(currentLevel);
            }
            
            UIManager.instance.LevelFailForUI(0.5f);
            isLevelFailed = true;
        }
    }
    public void CollectGem(Vector3 position, int count)
    {
        if (UIManager.instance.gameHasGems)
        {
            Vector3 worldToScreen = Camera.main.WorldToScreenPoint(position);
            GameObject flyingGem = Instantiate(UIManager.instance.gemIcons[0], worldToScreen, Quaternion.identity, transform);
            flyingGem.transform.GetChild(0).gameObject.SetActive(false);
            // flyingGem.GetComponent<Image>().enabled = false;
            flyingGem.AddComponent<GemIcon>().FlyGem(0, 2f, 1f);
            gemCountInLevel += count;
        }
    }

    public void InitDOTween()
    {
        //Default All DOTween Global Settings
        DOTween.Init(true, true, LogBehaviour.Default);
        DOTween.defaultAutoPlay = AutoPlay.None;
        DOTween.maxSmoothUnscaledTime = .15f;
        DOTween.nestedTweenFailureBehaviour = NestedTweenFailureBehaviour.TryToPreserveSequence;
        DOTween.showUnityEditorReport = false;
        DOTween.timeScale = 1f;
        DOTween.useSafeMode = true;
        DOTween.useSmoothDeltaTime = false;
        DOTween.SetTweensCapacity(200, 50);

        //Default All DOTween Tween Settings
        DOTween.defaultAutoKill = false;
        DOTween.defaultEaseOvershootOrAmplitude = 1.70158f;
        DOTween.defaultEasePeriod = 0f;
        DOTween.defaultEaseType = Ease.Linear;
        DOTween.defaultLoopType = LoopType.Restart;
        DOTween.defaultRecyclable = false;
        DOTween.defaultTimeScaleIndependent = false;
        DOTween.defaultUpdateType = UpdateType.Normal;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(level, currentLevel);
        PlayerPrefs.SetInt(gem, gemCountTotal);
    }
    public void RestartLevel()
    {
        LevelLoad();
    }
    public void NextLevel()
    {
        LevelLoad();
    }
}
