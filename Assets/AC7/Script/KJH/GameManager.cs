using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace kjh
{
    public class GameManager : SceneSingleton<GameManager>
    {        
        [SerializeField] Transform targetTrf;
        [SerializeField] GameObject _gameResultUi;
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
                for (int j = 0; j < targetTrf.GetChild(i).childCount; j++)
                {
                    AddActiveTarget(targetTrf.GetChild(i).GetChild(j).GetComponent<VehicleCombat>());
                }                
                //activeTargetList[i].onDeadWithSelf.AddListener(RemoveActiveTarget);
            }
        }

        /// <summary>
        /// 미션 성공시 호출되어 경과창을 띄워줌
        /// </summary>
        public void GameEnd(bool _win)
        {
            //일단 전투 결과를 보여준 후
            //확인을 누르면 
            //페이드아웃한 후
            //씬을 이동시킨다
            //씬매니저
            _gameResultUi.SetActive(true);
        }
        /// <summary>
        /// 메인메뉴로 돌아감
        /// 전투완료 버튼 누를시 호출
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
