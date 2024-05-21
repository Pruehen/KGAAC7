using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace kjh
{
    public class GameManager : SceneSingleton<GameManager>
    {        
        [SerializeField] Transform targetTrf;
        [SerializeField] GameResultUi _gameResultUi;
        public List<VehicleCombat> activeTargetList = new List<VehicleCombat>();
        public AircraftMaster player;

        public System.Action<Transform> OnTargetAdded;
        public System.Action<int> targetCountChanged;


        public System.Action<Transform> OnMissileAdded;

        /// <summary>
        /// ���ӸŴ����� Ÿ���� �߰�
        /// </summary>
        public void AddActiveTarget(VehicleCombat vehicleCombat)
        {
            activeTargetList.Add(vehicleCombat);
            targetCountChanged?.Invoke(activeTargetList.Count);
            OnTargetAdded?.Invoke(vehicleCombat.transform);
        }


        /// <summary>
        /// ���ӸŴ������� Ÿ���� ����
        /// </summary>
        public void RemoveActiveTarget(VehicleCombat vehicleCombat)
        {
            activeTargetList.Remove(vehicleCombat);
            targetCountChanged?.Invoke(activeTargetList.Count);
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
        public void GameEnd(bool _win, float fadeInTime, float delay = 0f)
        {
            //�ϴ� ���� ����� ������ ��
            //Ȯ���� ������ 
            //���̵�ƿ��� ��
            //���� �̵���Ų��
            //���Ŵ���
            Debug.Assert(_gameResultUi != null);
            StartCoroutine(DelayedCall(delay, _gameResultUi.FadeIn));
        }

        private IEnumerator DelayedCall(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        /// <summary>
        /// ���θ޴��� ���ư�
        /// �����Ϸ� ��ư ������ ȣ��
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            Debug.Log("ReturnToMainMenu");
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            Debug.Log("ReloadScene");
        }

        public void FadeIn(Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeEnd = null)
        {
            StartCoroutine(FadeInCoroutine(image, time, onlyActiveSelf, fadeEnd));
        }

        private IEnumerator FadeInCoroutine(Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeEnd = null)
        {
            float alpha = 0f;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            image.gameObject.SetActive(true);

            if (onlyActiveSelf)
            {
                for (int i = 0; i < image.transform.childCount; i++)
                {
                    image.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            while (image.color.a < 1f)
            {
                alpha += Time.deltaTime / time;
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            fadeEnd?.Invoke();
            yield break;
        }

        public void AddMissile(Transform target)
        {
            OnMissileAdded?.Invoke(target);
        }
    }
}
