using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MissileIndicator : MonoBehaviour
{
    [SerializeField]
    float blinkTimer = 0.1f;

    [SerializeField]
    float shrinkScaleValue = 0.7f;
    [SerializeField]
    float shrinkLerpAmount = 1;

    Vector3 shrinkScale;

    Guided missile;
    Transform missileTransform;
    Transform aircraftTransform;
    RectTransform rectTransform;

    bool isEmergency = false;
    RawImage rawImage;

    public Guided Missile
    {
        set
        {
            missile = value;
            missileTransform = value.transform;            
        }
    }

    void Blink()
    {
        rawImage.enabled = !rawImage.enabled;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        aircraftTransform = kjh.GameManager.Instance.player.transform;
        shrinkScale = Vector3.one * shrinkScaleValue;
    }

    void OnEnable()
    {
        rectTransform.localScale = Vector3.one;        

        rawImage.enabled = false;
        isEmergency = false;
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(aircraftTransform == null) return;

        if(!missile.Target() || missileTransform == null)
        {
            ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
            return;
        }

        Vector3 relativePos = missileTransform.position - aircraftTransform.position;
        // Emergency check
        float distance = relativePos.magnitude;
        if(isEmergency == false && distance < 5000)
        {
            isEmergency = true;
            InvokeRepeating("Blink", blinkTimer, blinkTimer);
        }

        if(isEmergency == true)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, shrinkScale, shrinkLerpAmount * Time.deltaTime);
        }

        // Set Angle
        float angle = Mathf.Atan2(-relativePos.x, relativePos.z) * Mathf.Rad2Deg;
        angle += Camera.main.transform.eulerAngles.y;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        if(rawImage.enabled == false && isEmergency == false)
        {
            rawImage.enabled = true;
        }
    }
}
