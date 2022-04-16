using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject startMenuUI;
    [SerializeField] GameObject controlMenuUI;
    [SerializeField] GameObject soundMenuUI;
    [SerializeField] GameObject backButton;

    [SerializeField] GameObject score;
    

    // First function called
    // Called before Start
    private void Awake()
    {
        PauseMenu.GameCanPause = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        score.SetActive(false);
        BackToStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        Debug.Log("To First Scene");
        PauseMenu.GameCanPause = true;
        StartCoroutine(VariableManager.Instance.transition.ExitScene());
    }

    public void BackToStart()
    {
        Debug.Log("Loading Start Menu...");
        controlMenuUI.SetActive(false);
                
        soundMenuUI.SetActive(false);
        
        backButton.SetActive(false);

        startMenuUI.SetActive(true);
                
    }

    public void LoadControlsMenu()
    {
        Debug.Log("Loading Controls Menu...");
        startMenuUI.SetActive(false);
        
        controlMenuUI.SetActive(true);
        
        backButton.SetActive(true);
    }

    public void LoadSoundMenu()
    {
        Debug.Log("Loading Sound Menu...");
        startMenuUI.SetActive(false);
        
        soundMenuUI.SetActive(true);
        
        backButton.SetActive(true);
    }
}
