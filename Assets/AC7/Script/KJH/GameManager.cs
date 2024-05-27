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

        public CameraShake cameraShake;

        /// <summary>
        /// 게임매니저에 타겟을 추가
        /// </summary>
        public void AddActiveTarget(VehicleCombat vehicleCombat)
        {
            if (vehicleCombat.gameObject.activeInHierarchy && !activeTargetList.Contains(vehicleCombat))
            {
                activeTargetList.Add(vehicleCombat);
                targetCountChanged?.Invoke(activeTargetList.Count);
                OnTargetAdded?.Invoke(vehicleCombat.transform);
            }
        }


        /// <summary>
        /// 게임매니저에서 타겟을 제거
        /// </summary>
        public void RemoveActiveTarget(VehicleCombat vehicleCombat)
        {
            if (vehicleCombat.gameObject.activeInHierarchy && activeTargetList.Contains(vehicleCombat))
            {
                activeTargetList.Remove(vehicleCombat);
                targetCountChanged?.Invoke(activeTargetList.Count);
            }
        }

        private void Awake()
        {
            if(targetTrf == null)
            {
                targetTrf = GameObject.Find("Enemy_Transform").transform;
            }

            for (int i = 0; i < targetTrf.childCount; i++)
            {
                for (int j = 0; j < targetTrf.GetChild(i).childCount; j++)
                {
                    VehicleCombat combat = targetTrf.GetChild(i).GetChild(j).GetComponent<VehicleCombat>();                    
                    AddActiveTarget(combat);

                    //배의 경우 파츠를 가지고 있으므로 자식을 찾아서 추가
                    VehicleCombat[] childCombat = combat.GetComponentsInChildren<VehicleCombat>();
                    foreach (VehicleCombat childCombatItem in childCombat)
                    { AddActiveTarget(childCombatItem); }

                }
                //activeTargetList[i].onDeadWithSelf.AddListener(RemoveActiveTarget);
            }
        }

        /// <summary>
        /// 미션 성공시 호출되어 경과창을 띄워줌
        /// </summary>
        public void GameEnd(bool _win, float fadeInTime, float delay = 0f)
        {
            //일단 전투 결과를 보여준 후
            //확인을 누르면 
            //페이드아웃한 후
            //씬을 이동시킨다
            //씬매니저
            Debug.Assert(_gameResultUi != null);

            UnityEngine.Cursor.lockState = CursorLockMode.None;

            StartCoroutine(DelayedCall(delay, _gameResultUi.FadeIn));
            //플레이어 정지
            StartCoroutine(DelayedCall(delay + 1f,  () => player.gameObject.SetActive(false))) ;
        }

        public IEnumerator DelayedCall(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        /// <summary>
        /// 메인메뉴로 돌아감
        /// 전투완료 버튼 누를시 호출
        /// </summary>
        public void ReturnToMainMenu()
        {
            
            Destroy(GameObject.Find("_"+ kjh.GameManager.Instance.player.AircraftSelecter().name));
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        public void ReloadCurrentScene()
        {
            bsj.SoundManager.Instance.Reset();
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
                alpha += Time.fixedUnscaledDeltaTime / time;
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
