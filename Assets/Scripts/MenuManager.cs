using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public string PlayerName;
    public string HighScorerName;
    public int HighScore;
    public TextMeshProUGUI NameInput;
    public TextMeshProUGUI HighScoreDisplay;
    public TextMeshProUGUI LeaderboardDisplay;

    public string[] Leaderboard;
    public int[] LeaderboardScores;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadHighScore();
        UpdateLeaderboardDisplay();

        HighScoreDisplay.text = "Best Score: " + HighScore + " by: " + HighScorerName;

    }

    public void StartNew() {
        PlayerName = NameInput.text;
        SceneManager.LoadScene(1);
    }

    public void Exit() {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    [System.Serializable]
    class SaveData {
        public string HighScorerName;
        public int HighScore;
        public string[] Leaderboard;
        public int[] LeaderboardScores;
    }

    public void SaveHighScore() {
        SaveData data = new SaveData();
        data.HighScorerName = HighScorerName;
        data.HighScore = HighScore;
        data.Leaderboard = Leaderboard;
        data.LeaderboardScores = LeaderboardScores;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void InsertIntoLeaderboard(int m_Points) {
        int i = 0;
        while (m_Points < LeaderboardScores[i]) {
            i++;
        }
        string currentString = PlayerName;
        int currentInt = m_Points;
        while (i < 10) {
            string tmpString = Leaderboard[i];
            int tmpInt = LeaderboardScores[i];
            Leaderboard[i] = currentString;
            LeaderboardScores[i] = currentInt;
            currentString = tmpString;
            currentInt = tmpInt;
            i++;
        }
    }

    public void UpdateLeaderboardDisplay() {
        string display = "Top 10:\n";
        for (int i = 0; i < 10; i++) {
            display += $"{i + 1}. {Leaderboard[i]} - {LeaderboardScores[i]}\n";
        }
        LeaderboardDisplay.text = display;
    }

    public void LoadHighScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            HighScorerName = data.HighScorerName;
            HighScore = data.HighScore;
            if (data.Leaderboard != null && data.Leaderboard.Length == 10) {
                Leaderboard = data.Leaderboard;
            }
            else {
                Leaderboard = new string[10];
                for (int i = 0; i < 10; i++) {
                    Leaderboard[i] = "---";
                }
            }
            if (data.LeaderboardScores != null && data.LeaderboardScores.Length == 10) {
                LeaderboardScores = data.LeaderboardScores;
            }
            else {
                LeaderboardScores = new int[10];
            }
        } else {
            Leaderboard = new string[10];
            LeaderboardScores = new int[10];

            for (int i = 0; i < 10; i++) {
                Leaderboard[i] = "---";
                LeaderboardScores[i] = 0;
            }
        }
    }
}
