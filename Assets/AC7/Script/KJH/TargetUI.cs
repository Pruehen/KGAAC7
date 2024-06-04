using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class TargetUI : MonoBehaviour
{
    VehicleCombat targetObject;

    [Header("UI / Texts")]
    [SerializeField]
    RawImage frameImage;
    [SerializeField] RawImage outerLock;
    [SerializeField] RawImage innerLock;

    [SerializeField]
    TextMeshProUGUI distanceText;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI nicknameText;
    [SerializeField]
    TextMeshProUGUI targetText;

    [Header("Properties")]
    [SerializeField]
    bool isMainTarget;
    
    [SerializeField]
    float hideDistance;
    
    [SerializeField]
    GameObject uiObject;
    [SerializeField]
    GameObject blinkUIObject;
    [SerializeField]
    GameObject nextTargetText;

    [SerializeField]
    Color normalColor;
    [SerializeField]
    Color warningColor;

    [SerializeField]
    float blinkRepeatTime;

    bool isTargeted;
    bool isNextTarget;
    bool isBlinking;

    bool isInvisible;

    public bool IsInvisible
    {
        set { isInvisible = value; }
    }

    //ObjectInfo objectInfo;
    RectTransform rectTransform;

    public VehicleCombat Target
    {
        get
        {
            return targetObject;
        }

        set
        {
            targetObject = value;
            //objectInfo = targetObject.Info;

            if (targetObject != null)
            {
                nameText.text = targetObject.name;
                nicknameText.text = targetObject.nickname;
                targetText.gameObject.SetActive(targetObject.mainTarget);
                isTargeted = (TargetUIManager.Instance.Radar_Ref.GetTarget() == targetObject);
                SetTargetted();
            }
        }
    }

    public GameObject UIObject
    {
        get { return uiObject; }
    }

    Vector2 screenSize;
    float screenAdjustFactor;
    Camera activeCamera;
    
    // Recursive search
    Canvas GetCanvas(Transform parentTransform)
    {
        if(parentTransform.GetComponent<Canvas>() != null)
        {
            return parentTransform.GetComponent<Canvas>();
        }
        else
        {
            return GetCanvas(parentTransform.parent);
        }
    }

    /*public void SetTargetted(bool isTargetted)
    {
        //this.isTargetted = isTargetted;
        SetBlink(isTargetted);
        frameImage.color = GameManager.NormalColor;
    }*/

    void SetTargetted()
    {
        if(isTargeted)
        {
            outerLock.gameObject.SetActive(true);
            innerLock.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            nicknameText.gameObject.SetActive(true);     
            distanceText.gameObject.SetActive(true);
        }
        else
        {
            outerLock.gameObject.SetActive(false);
            innerLock.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            nicknameText.gameObject.SetActive(false);
            distanceText.gameObject.SetActive(false);
        }
    }
    void TargetIsInRange()
    {
        Radar radar = TargetUIManager.Instance.Radar_Ref;
        if (targetObject == radar.GetTarget())
        {
            if (radar.isRadarLock)
            {
                innerLock.color = warningColor;
            }
            else
            {
                innerLock.color = normalColor;
            }

            if (radar.isMissileLock)
            {
                outerLock.color = warningColor;
            }
            else
            {
                outerLock.color = normalColor;
            }
        }
    }


    /*void SetBlink(bool blink)
    {
        if(isBlinking == blink) return;

        if(blink == true)
        {
            isBlinking = true;
            InvokeRepeating("Blink", 0, blinkRepeatTime);
        }
        else
        {
            isBlinking = false;
            CancelInvoke();
            blinkUIObject.SetActive(true);
        }
    }*/

    /*void Blink()
    {
        blinkUIObject.SetActive(!blinkUIObject.activeInHierarchy);
    }*/

    /*public void SetLock(bool isLocked)
    {
        if(isLocked == true)
        { 
            SetBlink(false);
            frameImage.color = GameManager.WarningColor;
        }
        else
        {
            SetTargetted(targetObject != null);
            frameImage.color = GameManager.NormalColor;
        }
    }*/

    // Call before destroy
    void OnDestroy()
    {
        targetObject = null;
        CancelInvoke();
    }


    void Start()
    {
        isInvisible = true;
        rectTransform = GetComponent<RectTransform>();

        screenSize = new Vector2(Screen.width, Screen.height);
        screenAdjustFactor = Mathf.Max((1920.0f / Screen.width), (1080.0f / Screen.height));
        Target = targetObject;  // execute Setter code
    }

    // Update is called once per frame
    void LateUpdate()
    {
        activeCamera = Camera.main;

        if(targetObject == null || targetObject.IsDead())
        {
            TargetUIManager.Instance.RemoveListUI(this);
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
            return;
        }
        Vector3 screenPosition = activeCamera.WorldToScreenPoint(targetObject.transform.position);
        bool isOutsideOfCamera = (screenPosition.z < 0 ||
                    screenPosition.x < 0 || screenPosition.x > screenSize.x ||
                    screenPosition.y < 0 || screenPosition.y > screenSize.y);

        if (isOutsideOfCamera)
        {
            TargetUIManager.Instance.RemoveListUI(this);
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
            return;
        }
        
        float distance = Vector3.Distance(targetObject.transform.position, kjh.GameManager.Instance.player.transform.position);
        nextTargetText.SetActive(false);

        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // Text
            if(distance < 1000)
            {
                distanceText.text = string.Format("{0:0}m", distance);
            }
            else
            {
                distanceText.text = string.Format("{0:0.##}km", distance * 0.001f);
            }    

            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(activeCamera, targetObject.transform.position);
            Vector2 position = screenPoint - screenSize * 0.5f;
            position *= screenAdjustFactor;
            rectTransform.anchoredPosition = position;
        }

        // the transform is outside of camera view (not behind, we need to consider Field of View)

        //uiObject.SetActive(isOutsideOfCamera == false && isInvisible == true && distance < hideDistance);

        TargetIsInRange();

        //GameManager.TargetController.ShowTargetArrow(isOutsideOfCamera && distance < hideDistance);
    }
}
