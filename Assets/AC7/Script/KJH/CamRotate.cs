using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] AircraftMaster aircraftMaster;    
    AircraftControl aircraftControl;
    Transform camAxisTrf;//���� ȸ����ų ��
    [SerializeField]Transform virtualAxis;//���� ��
    Transform viewTargetTrf;//ī�޶� ������ Ʈ������

    bool isTargetTraking = false;
    float lerpTime = 0;
    float postDeadTargetTime = 0;
    public void SetTargetTrf()
    {
        isTargetTraking = true;
        lerpTime = 0;
    }
    public void RemoveTargetTrf()
    {
        isTargetTraking = false;
        lerpTime = 0;
        viewTargetTrf = null;
    }

    Vector3 initLocalPos;

    // Start is called before the first frame update
    void Start()
    {
        aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
        initLocalPos = this.transform.localPosition;
        camAxisTrf = this.transform.parent;
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        float throttle = aircraftControl.throttle;                

        Vector3 camTargetPos = initLocalPos + new Vector3(0, 0, -throttle);
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, camTargetPos, Time.fixedDeltaTime);


        if(isTargetTraking)//ī�޶� ���� ����� ���
        {
            Transform viewTargetTrfTemp = viewTargetTrf;

            VehicleCombat vehicleCombat = aircraftMaster.GetComponent<Radar>().GetTarget();
            if(vehicleCombat != null)
            {
                viewTargetTrf = vehicleCombat.transform;//���̴� Ÿ������ ī�޶��� Ÿ���� ����            
            }
            

            if(viewTargetTrfTemp != viewTargetTrf)//Ÿ���� ����Ǿ��� ���
            {
                lerpTime = 0;
            }
        }

        if(viewTargetTrf != null)//ī�޶��� Ÿ���� ������ ���
        {
            virtualAxis.LookAt(viewTargetTrf.position);//�ش� Ÿ���� �ٶ󺸵��� ���� ���� ����
        }
        else
        {
            virtualAxis.localRotation = Quaternion.identity;            
        }

        lerpTime += Time.deltaTime;
        if (lerpTime < 1)
        {
            camAxisTrf.rotation = Quaternion.Lerp(camAxisTrf.rotation, virtualAxis.rotation, lerpTime);
        }
        else
        {
            camAxisTrf.rotation = virtualAxis.rotation;
        }
    }
}
