using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ Ŭ����. ���� ������Ʈ�� ������ �� ���. �����ϸ� ���⼭ ���� ������Ʈ�� �������� �� ��
public class AircraftMaster : NetworkBehaviour
{
    [SerializeField] bool aiControl;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (aiControl)
        {
            GetComponent<FlightController>().enabled = false;
        }
        else
        {
            GetComponent<FlightController_AI>().enabled = false;
            bsj.GameManager.Instance.TriggerPlayerSpawn(transform);
            Camera.main.transform.parent.SetParent(transform);
        }

    }
}
