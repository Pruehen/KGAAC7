using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using kjh;

//� �װ��⸦ ����ϴ��� �˷��ִ� Ŭ����
public class AircraftSelecter : MonoBehaviour
{
    public GameObject controlAircraft;

    public AircraftData aircraftData { get; private set; }
    public kjh.WeaponSystem weaponSystem { get; private set; }
    public AircraftControl aircraftControl { get; private set; }

    private void Awake()
    {
        SetControlAircraft(controlAircraft);
    }

    /// <summary>
    /// aircraft�� ����� �װ��⸦ �����ִ� �޼���
    /// </summary>
    /// <param name="controlAircraft"></param>
    public void SetControlAircraft(GameObject controlAircraft)
    {
        if (this.aircraftControl != null)
        {
            this.controlAircraft.SetActive(false);
        }

        if(controlAircraft == null)
        {
            //���⼭ string �����͸� �޾Ƽ� find �޼��带 ���� ���� ���ӿ�����Ʈ�� ���� �� �Ҵ�
        }

        this.controlAircraft = controlAircraft;
        this.controlAircraft.SetActive(true);
        aircraftData = controlAircraft.GetComponent<AircraftData>();
        weaponSystem = controlAircraft.GetComponent<kjh.WeaponSystem>();
        weaponSystem.Init();
        aircraftControl = controlAircraft.GetComponent<AircraftControl>();
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
