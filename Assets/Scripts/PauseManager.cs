using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public Button pauseButton;
    public GameObject pausePanel;
    public Button resumeButton;
    public Button restartButtonPause;

    public bool IsPaused = false;

    void Awake()
    {
        Instance = this;
        pausePanel.SetActive(false);
    }

    void Start()
    {
        pauseButton.onClick.AddListener(Pause);
        resumeButton.onClick.AddListener(Resume);
        restartButtonPause.onClick.AddListener(GameManager.Instance.RestartGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        // Don't pause if the game is already over
        if (GameManager.Instance.IsPlaying == false) return;

        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}