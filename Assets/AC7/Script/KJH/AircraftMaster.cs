using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//참조용 클래스. 하위 컴포넌트에 접근할 때 사용. 웬만하면 여기서 하위 컴포넌트를 수정하지 말 것
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
