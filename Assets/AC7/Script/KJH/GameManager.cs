using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class GameManager : SceneSingleton<GameManager>
    {        
        [SerializeField] Transform targetTrf;
        public List<VehicleCombat> activeTargetList = new List<VehicleCombat>();
        public AircraftMaster player;

        /// <summary>
        /// ���ӸŴ����� Ÿ���� �߰�
        /// </summary>
        public void AddActiveTarget(VehicleCombat vehicleCombat)
        {
            activeTargetList.Add(vehicleCombat);
        }


        /// <summary>
        /// ���ӸŴ������� Ÿ���� ����
        /// </summary>
        public void RemoveActiveTarget(VehicleCombat vehicleCombat)
        {
            activeTargetList.Remove(vehicleCombat);
        }

        private void Awake()
        {
            for (int i = 0; i < targetTrf.childCount; i++)
            {
                AddActiveTarget(targetTrf.GetChild(i).GetComponent<VehicleCombat>());
                //activeTargetList[i].onDeadWithSelf.AddListener(RemoveActiveTarget);
            }
        }
    }
}
