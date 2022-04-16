using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool GameCanPause = true;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject controlMenuUI;
    [SerializeField] GameObject soundMenuUI;
    [SerializeField] GameObject mapMenuUI;
    [SerializeField] GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && GameCanPause)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        pauseMenuUI.SetActive(true);
        controlMenuUI.SetActive(false);
        soundMenuUI.SetActive(false);
        backButton.SetActive(false);
        mapMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadControlsMenu()
    {
        Debug.Log("Loading Controls Menu...");
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(true);
        backButton.SetActive(true);
    }

    public void LoadSoundMenu()
    {
        Debug.Log("Loading Sound Menu...");
        pauseMenuUI.SetActive(false);
        soundMenuUI.SetActive(true);
        backButton.SetActive(true);
    }

    public void LoadMapMenu()
    {
        Debug.Log("Loading Sound Menu...");
        pauseMenuUI.SetActive(false);
        mapMenuUI.SetActive(true);
        backButton.SetActive(true);
    }

    public void QuitGame()
    {
        Resume();
        Debug.Log("To Start Screen");
        StartCoroutine(VariableManager.Instance.transition.ExitToStartScene());
    }

    public void BackToPause()
    {
        Debug.Log("Loading Pause Menu...");
        controlMenuUI.SetActive(false);
        soundMenuUI.SetActive(false);
        mapMenuUI.SetActive(false);
        backButton.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
