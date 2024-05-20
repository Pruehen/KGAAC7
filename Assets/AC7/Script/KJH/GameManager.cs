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
        /// 게임매니저에 타겟을 추가
        /// </summary>
        public void AddActiveTarget(VehicleCombat vehicleCombat)
        {
            activeTargetList.Add(vehicleCombat);
        }


        /// <summary>
        /// 게임매니저에서 타겟을 제거
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
