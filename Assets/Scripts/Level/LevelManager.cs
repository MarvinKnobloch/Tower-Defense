using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform[] enemyWayPoints;

    [Space]
    [SerializeField] private LevelObj[] levels;
    [SerializeField] private float timeOnGameStart;
    [SerializeField] private float timeBetweenLevels;
    private float betweenLevelesTime;
    [SerializeField] private int currentLevel;
    private TextMeshProUGUI levelTimerText;

    [Space]
    [SerializeField] private int enemiesAlive;
    [SerializeField] private int activeSpawners;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        currentLevel = 1;
    }
    private void Start()
    {
        if (PlayerUI.Instance != null)
        {
            levelTimerText = PlayerUI.Instance.levelTimer.GetComponentInChildren<TextMeshProUGUI>();
        }
        StartCoroutine(WaitForNextRound(timeOnGameStart));
    }
    private void OnEnable()
    {
        Enemy.enemyHasDied += CheckForNextRound;
    }
    private void OnDisable()
    {
        Enemy.enemyHasDied -= CheckForNextRound;
    }
    public Vector3 GetWayPoint(int number)
    {
        return enemyWayPoints[number].position;
    }
    private IEnumerator WaitForNextRound(float time)
    {
        betweenLevelesTime = time;
        if(PlayerUI.Instance != null)
        {
            PlayerUI.Instance.levelTimer.SetActive(true);
            levelTimerText.text = betweenLevelesTime.ToString("0");
        }
        while (betweenLevelesTime >= 0)
        {
            betweenLevelesTime -= Time.deltaTime;
            levelTimerText.text = betweenLevelesTime.ToString("0");
            yield return null;
        }

        PlayerUI.Instance.levelTimer.SetActive(false);

        if (currentLevel < levels.Length)
        {
            for (int i = 0; i < levels[currentLevel].levelEnemyValues.Length; i++)
            {
                activeSpawners++;
                StartCoroutine(StartEnemySpawn(levels[currentLevel].levelEnemyValues[i].amount, levels[currentLevel].levelEnemyValues[i].spawnRate, levels[currentLevel].levelEnemyValues[i].enemy));
            }
        }
        currentLevel++;
    }
    private IEnumerator StartEnemySpawn(int spawnAmount, float spawnRate, GameObject prefab)
    {
        int amount = 0;

        while (amount < spawnAmount)
        {
            enemiesAlive++;
            Instantiate(prefab, enemyWayPoints[0].position, Quaternion.identity);
            amount++;

            if(amount >= spawnAmount) activeSpawners--;

            yield return new WaitForSeconds(spawnRate);
        }
    }
    private void CheckForNextRound()
    {
        enemiesAlive--;
        if (enemiesAlive == 0 && activeSpawners == 0)
        {
            if (currentLevel < levels.Length)
            {
                StartCoroutine(WaitForNextRound(timeBetweenLevels));
            }
        }
    }
}
