using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI squadText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    public bool IsPlaying = true;

    private int score = 0;
    private int wave = 1;

    void Awake()
    {
        Instance = this;
        gameOverPanel.SetActive(false);
    }

    public void AddScore(int amount)
    {
        score = score + amount;
        scoreText.text = "Score: " + score;
    }

    public void UpdateHUD()
    {
        squadText.text = "Squad: " + SquadManager.Instance.UnitCount;
    }

    public void OnWaveChanged(int w)
    {
        wave = w;
        waveText.text = "Wave " + wave;
    }

    public void TriggerGameOver()
    {
        IsPlaying = false;
        Time.timeScale = 0f;
        finalScoreText.text = "Score: " + score;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}