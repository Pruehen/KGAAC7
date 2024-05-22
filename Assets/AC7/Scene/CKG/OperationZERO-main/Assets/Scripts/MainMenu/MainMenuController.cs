using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Unity.VisualScripting;

public class MainMenuController : MonoBehaviour
{
    private static MainMenuController instance = null;
    
    [SerializeField]
    PlayerInput playerInput;

    [SerializeField]
    FadeController fadeController;

    [SerializeField]
    GameObject mainMenuScreen;
    [SerializeField]
    GameObject selectPlayScreen;
    [SerializeField]
    GameObject missionSettings;
    [SerializeField]
    GameObject optionScreen;
    [SerializeField]
    GameObject resultScreen;
    [SerializeField]
    GameObject airCombatSettings;
    [SerializeField]
    GameObject LoadingScreen1;
    [SerializeField]
    GameObject LoadingScreen2; 
    [SerializeField]
    GameObject LoadingScreen3;
    [SerializeField]
    TextMeshProUGUI descriptionText;
    
    [SerializeField]
    float initDelay;

    [Header("Audios")]
    
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip scrollAudioClip;
    [SerializeField]
    AudioClip confirmAudioClip;
    [SerializeField]
    AudioClip backAudioClip;

    GameObject currentActiveScreen = null;
    MenuController currentMenuController = null;

    public UnityEvent onNavigateEvent;

    public void SetDescriptionText(string text)
    {
        descriptionText.text = text;
    }

    public static MainMenuController Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    
    public static PlayerInput PlayerInput
    {
        get { return Instance?.playerInput; }
    }

    public void Navigate(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            PlayScrollAudioClip();
            currentMenuController?.Navigate(context);
        }
        onNavigateEvent.Invoke();
    }

    public void Confirm(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            PlayConfirmAudioClip();
            currentMenuController?.Confirm(context);
        }
    }
    
    public void Back(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            PlayBackAudioClip();
            currentMenuController?.Back(context);
        }
    }

    public void PlayConfirmAudioClip()
    {
        audioSource.PlayOneShot(confirmAudioClip);
    }

    public void PlayScrollAudioClip()
    {
        audioSource.PlayOneShot(scrollAudioClip);
    }

    public void PlayBackAudioClip()
    {
        audioSource.PlayOneShot(backAudioClip);
    }

    void SetCurrentActiveScreen(GameObject screenObject)
    {
        currentActiveScreen?.SetActive(false);
        currentActiveScreen = screenObject;
        currentMenuController = currentActiveScreen.GetComponent<MenuController>();
        currentActiveScreen.SetActive(true);
    }

    public void SetLanguage(string language)
    {
        if(language.ToLower() == "en")
        {
            GameSettings.languageSetting = GameSettings.Language.EN;
        }
        
        if(language.ToLower() == "kr")
        {
            GameSettings.languageSetting = GameSettings.Language.KR;
        }
    }

    public void SetDifficulty(int difficulty)
    {
        GameSettings.difficultySetting = (GameSettings.Difficulty)difficulty;
    }
    
    public void ShowSelectPlayScreen()
    {
        SetCurrentActiveScreen(selectPlayScreen);
    }
    public void ShowMissonSettings()
    {
        SetCurrentActiveScreen(missionSettings);
    }
    public void ShowAirCombatSettings1()
    {
        SetCurrentActiveScreen(LoadingScreen1);
        StartCoroutine(OnAirCombatScreen(LoadingScreen1));
        LoadingController.sceneName = "Mission01";
    }
    public void ShowAirCombatSettings2()
    {
        SetCurrentActiveScreen(LoadingScreen2);
        StartCoroutine(OnAirCombatScreen(LoadingScreen2));
        LoadingController.sceneName = "Mission02";
    }
    public void ShowAirCombatSettings3()
    {
        SetCurrentActiveScreen(LoadingScreen3);
        StartCoroutine(OnAirCombatScreen(LoadingScreen3));
        LoadingController.sceneName = "Mission03";
    }
    public void ShowMainMenu()
    {
        SetCurrentActiveScreen(mainMenuScreen);
    }

    public void ShowSettingsMenu()
    {
        SetCurrentActiveScreen(optionScreen);
    }

    public void ShowResultMenu()
    {
        SetCurrentActiveScreen(resultScreen);
    }

    public void ShowLoadingMenu()
    {
        
    }
    public void StartMission()
    {
        playerInput.enabled = false;
        fadeController.OnFadeOutComplete.AddListener(ReserveLoadScene);
        fadeController.FadeOut();
        currentActiveScreen.GetComponent<MenuController>().enabled = false; // Prevent MissingReferenceException about InputSystem
    }

    public void StartFreeFlight()
    {
        playerInput.enabled = false;
        LoadingController.sceneName = "FreeFlight";

        fadeController.OnFadeOutComplete.AddListener(ReserveLoadScene);
        fadeController.FadeOut();
        currentActiveScreen.GetComponent<MenuController>().enabled = false; // Prevent MissingReferenceException about InputSystem
    }

    public void ReserveLoadScene()
    {
        SceneManager.LoadScene("Loading");
    }

    public void Quit()
    {
        #if UNITY_WEBGL
        
        #else
        playerInput.enabled = false;

        fadeController.OnFadeOutComplete.AddListener(QuitEvent);
        fadeController.FadeOut();
        #endif
        
    }
    
    void QuitEvent()
    {
        Application.Quit();
    }

    IEnumerator InitMainMenu()
    {
        yield return new WaitForSeconds(initDelay);
        SetCurrentActiveScreen(mainMenuScreen);
    }

    IEnumerator OnAirCombatScreen(GameObject loadingScreen)
    {
        airCombatSettings.SetActive(true);
        Transform airCombatSelect = airCombatSettings.transform.Find("AirCombatSelect");
        Transform airCombatEnvironment = airCombatSettings.transform.Find("AirCombatEnvironment");
        if (airCombatSelect != null)
        {
            airCombatSelect.gameObject.SetActive(true);
        }
        if (airCombatEnvironment != null)
        {
            airCombatEnvironment.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(5);
        Transform parentTransform = airCombatSettings.transform.parent;
        Transform backgroundTransform = parentTransform.Find("Background");
        if (backgroundTransform != null)
        {
            backgroundTransform.gameObject.SetActive(false);
        }
        loadingScreen.SetActive(false);
        Transform selectUI = airCombatSettings.transform.Find("SelectUI");
        if (selectUI != null)
        {
            selectUI.gameObject.SetActive(true);
        }
        currentMenuController = airCombatSettings.GetComponent<MenuController>();
        onNavigateEvent.Invoke();
    }



    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        
        mainMenuScreen.SetActive(false);
        resultScreen.SetActive(false);
        optionScreen.SetActive(false);

        playerInput.enabled = true;
        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap("UI");
    }

    void Start()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;

        if(ResultData.missionName != "")
        {
            SetCurrentActiveScreen(resultScreen);
        }
        else
        {
            StartCoroutine(InitMainMenu());
        }
        descriptionText.text = "";

        onNavigateEvent.AddListener(UpdateAirCombatSelection);
    }

    void UpdateAirCombatSelection()
    {
        // Find the AirCombatSelect object
        Transform airCombatSelect = airCombatSettings.transform.Find("AirCombatSelect");
        if (airCombatSelect != null)
        {
            // Deactivate all children of airCombatSelect
            foreach (Transform child in airCombatSelect)
            {
                child.gameObject.SetActive(false);
            }

            // Activate the child corresponding to the current index
            if (currentMenuController != null && currentMenuController.currentIndex < airCombatSelect.childCount)
            {
                airCombatSelect.GetChild(currentMenuController.currentIndex).gameObject.SetActive(true);
            }
        }
    }
}
