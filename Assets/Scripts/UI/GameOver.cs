using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using LootLocker.Requests;

public class GameOver : MonoBehaviour
{
    int leaderboardID = 25376;

    [SerializeField] private TextMeshProUGUI finalSocretext;

    [SerializeField] private Transform rankPrefabRoot;
    [SerializeField] private GameObject rankPrefab;

    private bool commitedScore;
    [SerializeField] private TMP_InputField inputField;

    public void SetGameoverScreen(int newScore)
    {
        StartCoroutine(SetScoreText(newScore));
        StartCoroutine(IngameLeaderboardUpdate());
    }
    public IEnumerator IngameLeaderboardUpdate()
    {
        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardID.ToString(), 10, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] members = response.items;

                for (int i = 0; i < members.Length; i++)
                {
                    GameObject prefab = Instantiate(rankPrefab, rankPrefabRoot);
                    prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = members[i].rank + ". ";
                    if(members[i].player.name != "")
                    {
                        prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += members[i].player.name;
                    }
                    else
                    {
                        prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += members[i].player.id;
                    }
                    prefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = members[i].score.ToString();
                }
                done = true;
            }
            else
            {
                Debug.Log("load leaderboard failed");
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SetScoreText(int newScore)
    {
        bool done = false;
        string playerID = PlayerPrefs.GetString("PlayerID");
        LootLockerSDKManager.GetMemberRank(leaderboardID.ToString(), playerID, (response) =>
        {
            if (response.success)
            {
                if (newScore <= response.score)
                {
                    finalSocretext.text = "Final Score:\n" + newScore.ToString();
                }
                else
                {
                    finalSocretext.text = "New HighScore!\n" + newScore.ToString();
                }
                done = true;
            }
            else
            {
                finalSocretext.text = "New HighScore!\n" + newScore.ToString();
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }
    public void CommitScore()
    {
        if (inputField.text == string.Empty)
        {
            Debug.Log("inputField is empty");
            return; 
        }

        Debug.Log(commitedScore);
        if (commitedScore == false)
        {
            LootLockerSDKManager.SetPlayerName(inputField.text, (respone) =>
                {
                    if (respone.success)
                    {
                        Debug.Log("Changed name");
                    }
                    else
                    {
                        Debug.Log("Changed name failed");
                    }
                });
            StartCoroutine(PlayerUI.Instance.leaderboardController.LeaderboardUpdate(PlayerUI.Instance.scoreManager.currentScore));
            commitedScore = true;
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
}
