using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int currentScore = 0;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText.text = currentScore.ToString();
    }
    public void ScoreUpdate(int amount)
    {
        currentScore += amount;
        scoreText.text = "Current Score:\n" + currentScore.ToString();
    }
}
