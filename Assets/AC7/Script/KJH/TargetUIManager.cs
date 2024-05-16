using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUIManager : MonoBehaviour
{
    [SerializeField] GameObject targetUI;
    [SerializeField] AircraftMaster aircraftMaster;
    Radar radar;

    List<Transform> targetTrfList;
    List<TargetUI> targetUIList = new List<TargetUI>();

    // Start is called before the first frame update
    void Start()
    {
        radar = aircraftMaster.GetComponent<Radar>();
        targetTrfList = radar.targetList;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < targetTrfList.Count; i++)
        {
            if(targetUIList.Count == i)
            {
                targetUIList.Add(Instantiate(targetUI, this.transform).GetComponent<TargetUI>());
            }

            targetUIList[i].Target = targetTrfList[i].GetComponent<AircraftMaster>();
        }
    }    
}
