using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetUIManager : SceneSingleton<TargetUIManager>
{
    [SerializeField] GameObject targetUIPrf;
    public Radar Radar_Ref;
    VehicleCombat VehicleCombat_LocalPlayerVC;
    //[SerializeField] AircraftMaster aircraftMaster;

    List<VehicleCombat> targetList;
    List<TargetUI> useTargetUIList = new List<TargetUI>();
    public void RemoveListUI(TargetUI targetUI)
    {
        useTargetUIList.Remove(targetUI);
    }

    // Start is called before the first frame update
    void Start()
    {
        targetList = kjh.GameManager.Instance.activeTargetList;
        VehicleCombat_LocalPlayerVC = Radar_Ref.gameObject.GetComponent<VehicleCombat>();
        StartCoroutine(TargetListUpdate());
    }
    
    // Update is called once per frame
    /*void Update()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (useTargetUIList.Count == i)
            {
                GameObject item = ObjectPoolManager.Instance.DequeueObject(targetUI);
                item.transform.SetParent(this.transform);
                useTargetUIList.Add(item.GetComponent<TargetUI>());
            }

            useTargetUIList[i].Target = targetList[i];
        }
    } */   
    
    IEnumerator TargetListUpdate()
    {        
        while (true)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                if (useTargetUIList.Count == i)
                {
                    GameObject item = ObjectPoolManager.Instance.DequeueObject(targetUIPrf);
                    item.transform.SetParent(this.transform);
                    useTargetUIList.Add(item.GetComponent<TargetUI>());
                }

                if (VehicleCombat_LocalPlayerVC != targetList[i])
                {
                    useTargetUIList[i].Target = targetList[i];
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
