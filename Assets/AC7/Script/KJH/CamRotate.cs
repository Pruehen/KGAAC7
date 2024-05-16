using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] AircraftMaster aircraftMaster;
    [SerializeField] float camRotateMaxAngle;
    AircraftControl aircraftControl;

    Vector3 initLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
        initLocalPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {        
        float throttle = aircraftControl.throttle;                

        Vector3 camTargetPos = initLocalPos + new Vector3(0, 0, -throttle);
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, camTargetPos, Time.fixedDeltaTime);
    }
}
