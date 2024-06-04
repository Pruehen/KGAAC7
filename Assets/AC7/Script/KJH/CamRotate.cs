using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    //[SerializeField] AircraftMaster aircraftMaster;
    AircraftMaster aircraftMaster;
    AircraftControl aircraftControl;
    Transform camAxisTrf;//실제 회전시킬 축
    [SerializeField]Transform virtualAxis;//가상 축
    Transform viewTargetTrf;//카메라가 추적할 트랜스폼

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
    private void Start()
    {
        //bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
        OnPlayerSpawn();
    }
    private void OnPlayerSpawn()
    {
        aircraftMaster = kjh.GameManager.Instance.player.GetComponent<AircraftMaster>();
        GetComponent<AudioListener>().enabled = false;
        StartCoroutine(AudioListenerOn());
        aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
        initLocalPos = this.transform.localPosition;
        camAxisTrf = this.transform.parent;
    }

    IEnumerator AudioListenerOn()
    {
        yield return new WaitForSeconds(0.5f);
        BGM_Player.Instance.Play();
        GetComponent<AudioListener>().enabled = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {        
        if(aircraftControl == null)
        {
            //aircraftControl = aircraftMaster.AircraftSelecter().aircraftControl;
            return;
        }
        float throttle = aircraftControl.throttle;

        //Vector3 camTargetPos = initLocalPos + new Vector3(0, (isTargetTraking && viewTargetTrf != null) ? 5 : 0, -throttle);
        Vector3 camTargetPos = initLocalPos + new Vector3(0, 0, -throttle * 2);
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, camTargetPos, Time.fixedDeltaTime);


        if(isTargetTraking)//카메라가 추적 모드일 경우
        {
            Transform viewTargetTrfTemp = viewTargetTrf;

            VehicleCombat vehicleCombat = aircraftMaster.GetComponent<Radar>().GetTarget();
            if(vehicleCombat != null)
            {
                viewTargetTrf = vehicleCombat.transform;//레이더 타겟으로 카메라의 타겟을 지정            
            }            
            else
            {
                viewTargetTrf = null;
            }

            if(viewTargetTrfTemp != viewTargetTrf)//타겟이 변경되었을 경우
            {
                lerpTime = 0;
            }
        }

        if(viewTargetTrf != null)//카메라의 타겟이 존재할 경우
        {
            virtualAxis.LookAt(viewTargetTrf.position);//해당 타겟을 바라보도록 가상 축을 조정
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
