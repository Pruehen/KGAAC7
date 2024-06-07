using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Org.BouncyCastle.Utilities.Encoders;

public class UIControl : MonoBehaviour
{
    [SerializeField] AircraftMaster aircraftMaster;
    AircraftControl aircraftControl;

    // Center
    [Header("Common Center UI")]
    [SerializeField]
    TextMeshProUGUI speedText;
    [SerializeField]
    TextMeshProUGUI altitudeText;
    [SerializeField]
    AlertUIController alertUIController;

    [Header("1st-3rd View Control")]
    [SerializeField]
    RectTransform commonCenterUI;
    [SerializeField]
    RectTransform firstCenterViewTransform;
    [SerializeField]
    RectTransform thirdCenterViewTransform;

    [SerializeField]
    Canvas firstViewCanvas;
    [SerializeField]
    Vector2 firstViewAdjustAngle;

    [Header("1st View Center UI")]
    [SerializeField]
    UVController speedUV;
    [SerializeField]
    UVController altitudeUV;

    [SerializeField]
    Image throttleGauge;
    [SerializeField]
    HeadingUIController headingUIController;

    // Upper Right
    [Header("Upper Left UI")]
    [SerializeField]
    GameObject checkpointReachedUI;

    // Upper Left
    [Header("Upper Left UI")]
    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI targetText;

    // Lower Right
    [Header("Lower Right UI : Armament")]
    [SerializeField]
    TextMeshProUGUI gunText;
    [SerializeField]
    TextMeshProUGUI mslText;
    [SerializeField]
    TextMeshProUGUI spwText;

    [SerializeField]
    TextMeshProUGUI dmgText;

    [SerializeField]
    GameObject mslIndicator;
    [SerializeField]
    GameObject spwIndicator;

    // Status
    [Header("Lower Right UI : Aircraft/Weapon Status")]
    [SerializeField]
    Image aircraftImage;
    [SerializeField]
    CooldownImage leftMslCooldownImage;
    [SerializeField]
    CooldownImage rightMslCooldownImage;

    [Header("Minimap Misc.")]
    [SerializeField]
    MinimapCompass minimapCompass;
    [SerializeField]
    MinimapController minimapController;

    [Header("Materials")]
    [SerializeField]
    Material spriteMaterial;
    [SerializeField]
    Material fontMaterial;

    [Header("Colors")]
    [SerializeField]
    Color cautionColor;

    List<MaskColorChange> maskColorChanges = new List<MaskColorChange>();

    [Header("Sounds")]
    [SerializeField]
    AudioClip spwChangeAudioClip;
    [SerializeField]
    AudioClip mslChangeAudioClip;
    [SerializeField]
    AudioClip timeLowAudioClip;

    AudioSource audioSource;

    float remainTime;
    int score = 0;
    float damage = 0;
    bool isTimeLow = false;
    bool isRedTimerActive = false;
    bool enableCount = true;

    float elapsedTime = 0;

    public MinimapController MinimapController
    {
        get { return minimapController; }
    }

    public float StopCountAndGetElapsedTime()
    {
        enableCount = false;
        return elapsedTime;
    }

    public void StartCountAndResetElapsedTime()
    {
        enableCount = true;
        elapsedTime = 0;
    }

    // Center
    void SetSpeed(int speed)
    {
        string text = string.Format("<mspace=18>{0}</mspace>", speed);
        speedText.text = text;

        speedUV.SetUV(speed);
    }

    void SetAltitude(int altitude)
    {
        string text = string.Format("<mspace=18>{0}</mspace>", altitude);
        altitudeText.text = text;

        altitudeUV.SetUV(altitude);
    }

    public void SetThrottle(float throttle)
    {
        throttleGauge.fillAmount = (1 + throttle) * 0.5f; ;
    }

    public void SetHeading(float heading)
    {
        headingUIController.SetHeading(heading);
        //minimapCompass.SetCompass(heading);
    }

    void SetTime()
    {
        remainTime -= Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if (isRedTimerActive == false && remainTime < 10 && isTimeLow == false)
        {
            InvokeRepeating("PlayTimeLowAudioClip", 0, 1);
            isTimeLow = true;
        }

        if (remainTime <= 0)
        {
            CancelInvoke();
            GameManager.Instance.GameOver(false);
            remainTime = 0;
        }

        int seconds = (int)remainTime;

        int min = seconds / 60;
        int sec = seconds % 60;
        int millisec = (int)((remainTime - seconds) * 100);
        string text = string.Format("TIME <mspace=18>{0:00}</mspace>:<mspace=18>{1:00}</mspace>:<mspace=18>{2:00}</mspace>", min, sec, millisec);
        timeText.text = text;
    }

    public void SetScoreText(int score)
    {
        this.score += score;
        string text = string.Format("SCORE <mspace=18>{0}</mspace>", this.score);
        scoreText.text = text;
    }

    // Lower Right
    public void SetGunText(int bullets)
    {
        string text = string.Format("<align=left>GUN<line-height=0>\n<align=right><mspace=18>{0}</mspace><line-height=0>", bullets);
        gunText.text = text;
    }

    public void SetMissileText(int missiles)
    {
        string text = string.Format("<align=left>MSL<line-height=0>\n<align=right><mspace=18>{0}</mspace><line-height=0>", missiles);
        mslText.text = text;
    }

    public void SetSpecialWeaponText(string specialWeaponName, int specialWeapons)
    {
        string text = string.Format("<align=left>{0}<line-height=0>\n<align=right><mspace=18>{1}</mspace><line-height=0>", specialWeaponName, specialWeapons);
        spwText.text = text;
    }

    void SetAircraftDamageUI()
    {
        if (damage < 34)
        {
            //aircraftImage.color = GameManager.NormalColor;
        }
        else if (damage < 67)
        {
            //aircraftImage.color = cautionColor;
        }
        else
        {
            //aircraftImage.color = GameManager.WarningColor;
        }
        aircraftImage.color = Color.green;
    }

    public void SetDamage(int damage)
    {
        string text = string.Format("<align=left>DMG<line-height=0>\n<align=right>{0}%<line-height=0>", damage);
        dmgText.text = text;

        this.damage = damage;
        SetAircraftDamageUI();

        if (damage > 0)
        {
            StartCoroutine(alertUIController.ShowDamagedUI());
        }
    }

    public void SwitchWeapon(WeaponSlot[] weaponSlots, bool useSpecialWeapon, Missile missile, bool playAudio = true)
    {
        //mslIndicator.SetActive(!useSpecialWeapon);
        //spwIndicator.SetActive(useSpecialWeapon);

        // Justify that weaponSlots contains 2 slots
        //leftMslCooldownImage.SetWeaponData(weaponSlots[0], missile.missileFrameSprite, missile.missileFillSprite);
        //rightMslCooldownImage.SetWeaponData(weaponSlots[1], missile.missileFrameSprite, missile.missileFillSprite);

        if (playAudio == true)
        {
            AudioClip audioClip = (useSpecialWeapon == true) ? spwChangeAudioClip : mslChangeAudioClip;
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void SwitchUI(CameraController.CameraIndex index)
    {
        bool isFirstView = (index == CameraController.CameraIndex.FirstView ||
                            index == CameraController.CameraIndex.FirstViewWithCockpit);

        firstCenterViewTransform.gameObject.SetActive(isFirstView);

        RectTransform parentTransform = (isFirstView == true) ? firstCenterViewTransform : thirdCenterViewTransform;
        commonCenterUI.SetParent(parentTransform);

        commonCenterUI.anchoredPosition = Vector2.zero;
    }

    public void AddMaskColorChange(MaskColorChange mask)
    {
        maskColorChanges.Add(mask);
    }

    public void SetWarningUIColor(bool isWarning)
    {
        Color color;
        if (isWarning == true)
        {
            color = Color.red;
            aircraftImage.color = GameManager.WarningColor;
        }
        else
        {
            color = new Color32(0xAA, 0xFF, 0xAA, 0xFF);
            SetAircraftDamageUI();
        }

        spriteMaterial.color = color;
        fontMaterial.SetColor("_FaceColor", color);

        foreach (MaskColorChange maskColorChange in maskColorChanges)
        {
            maskColorChange.ChangeComponentColor(color);
        }
    }

    void Start()
    {
        firstViewAdjustAngle = new Vector2(1 / firstViewAdjustAngle.x, 1 / firstViewAdjustAngle.y);

        //mslIndicator.SetActive(true);
        //spwIndicator.SetActive(false);

        SetScoreText(0);
        SetWarningUIColor(false);

        aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableCount == true && remainTime > 0 && GameManager.Instance.IsGameOver == false) SetTime();

        UpdateUI();
    }

    void UpdateUI()
    {
        if(aircraftControl == null)
        {
            aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
            return;
        }
        SetSpeed((int)aircraftMaster.GetSpeed());
        SetAltitude((int)aircraftMaster.transform.position.y);
        SetThrottle(aircraftControl.throttle);
        SetHeading(aircraftMaster.transform.eulerAngles.y);
    }
}
