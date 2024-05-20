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
                for (int j = 0; j < targetTrf.GetChild(i).childCount; j++)
                {
                    AddActiveTarget(targetTrf.GetChild(i).GetChild(j).GetComponent<VehicleCombat>());
                }                
                //activeTargetList[i].onDeadWithSelf.AddListener(RemoveActiveTarget);
            }
        }

        /// <summary>
        /// �̼� ������ ȣ��Ǿ� ���â�� �����
        /// </summary>
        public void CompleteMission()
        {
            //�ϴ� ���� ����� ������ ��
            //Ȯ���� ������ 
            //���̵�ƿ��� ��
            //���� �̵���Ų��
            //���Ŵ���
            _gameResultUi.SetActive(true);
        }
        /// <summary>
        /// ���θ޴��� ���ư�
        /// �����Ϸ� ��ư ������ ȣ��
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}
