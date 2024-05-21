using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.Events;

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
    public void ShowAirCombatSettings()
    {
        // 현재 활성화된 화면을 설정하는 메서드
        SetCurrentActiveScreen(airCombatSettings);

        // airCombatSettings의 부모 오브젝트를 찾음
        Transform parentTransform = airCombatSettings.transform.parent;

        // 부모 오브젝트 아래에서 background 오브젝트를 찾음
        Transform backgroundTransform = parentTransform.Find("Background");

        // background 오브젝트가 있는지 확인하고 비활성화
        if (backgroundTransform != null)
        {
            backgroundTransform.gameObject.SetActive(false);
        }
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
        onNavigateEvent.Invoke();
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

    public void StartMission()
    {
        playerInput.enabled = false;
        LoadingController.sceneName = "TestScene_KJH";
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
