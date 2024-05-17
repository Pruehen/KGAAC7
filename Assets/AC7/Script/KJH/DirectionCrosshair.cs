using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionCrosshair : MonoBehaviour
{
    [SerializeField] AircraftMaster aircraftMaster;
    Rigidbody rigidbody;
    RectTransform rectTransform;
    Vector2 screenSize;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = aircraftMaster.GetComponent<Rigidbody>();
        screenSize = new Vector2(Screen.width, Screen.height);
        rectTransform = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = aircraftMaster.transform.position + rigidbody.velocity;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
        //float distance = GameManager.Instance.GetDistanceFromPlayer(targetTransform);

        // if screenPosition.z < 0, the object is behind camera
        if (screenPosition.z > 0)
        {
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetPosition);
            Vector2 position = screenPoint - screenSize * 0.5f;
            //position *= screenAdjustFactor;
            rectTransform.anchoredPosition = position;
            /*if (preventBlinkError == true)
            {
                CheckError(position);
            }
            else
            {
                rectTransform.anchoredPosition = position;
            }*/
        }
    }
}
