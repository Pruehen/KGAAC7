using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftMaster : MonoBehaviour
{
    [SerializeField] bool aiControl;


    private void Awake()
    {
        if(aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
