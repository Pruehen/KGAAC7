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
        [SerializeField] PlayerInputCustom _playerInputCustom;
        public List<VehicleCombat> activeTargetList = new List<VehicleCombat>();
        public AircraftMaster player;

        public System.Action<Transform> OnTargetAdded;
        public System.Action<int> targetCountChanged;

        public System.Action<Transform> OnMissileAdded;

        public CameraShake cameraShake;

        /// <summary>
        /// ���ӸŴ����� Ÿ���� �߰�
        /// </summary>
        public void AddActiveTarget(VehicleCombat vehicleCombat)
        {
            if (!activeTargetList.Contains(vehicleCombat))
            {
                activeTargetList.Add(vehicleCombat);
                targetCountChanged?.Invoke(activeTargetList.Count);
                OnTargetAdded?.Invoke(vehicleCombat.transform);
            }
        }


        /// <summary>
        /// ���ӸŴ������� Ÿ���� ����
        /// </summary>
        public void RemoveActiveTarget(VehicleCombat vehicleCombat)
        {
            if (activeTargetList.Contains(vehicleCombat))
            {
                Debug.Log("RemoveActiveTarget");
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

                    //���� ��� ������ ������ �����Ƿ� �ڽ��� ã�Ƽ� �߰�
                    VehicleCombat[] childCombat = combat.GetComponentsInChildren<VehicleCombat>();
                    foreach (VehicleCombat childCombatItem in childCombat)
                    { AddActiveTarget(childCombatItem); }

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

            UnityEngine.Cursor.lockState = CursorLockMode.None;

            StartCoroutine(DelayedCall(delay, _gameResultUi.FadeIn));
            //�÷��̾� ����
            StartCoroutine(DelayedCall(delay + 1f,  () => player.gameObject.SetActive(false))) ;
        }
        public void GameReset(float delay = 0f)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;

            StartCoroutine(DelayedCall(delay, _gameResultUi.FadeOutResultUi));
            //�÷��̾� ����
            player.gameObject.SetActive(true);

        }

        public IEnumerator DelayedCall(float time, System.Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action?.Invoke();
        }

        /// <summary>
        /// ���θ޴��� ���ư�
        /// �����Ϸ� ��ư ������ ȣ��
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
        public void FadeOut(Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeStart = null, System.Action fadeEnd = null)
        {
            StartCoroutine(FadeOutCoroutine(image, time, onlyActiveSelf, fadeStart, fadeEnd));
        }

        private IEnumerator FadeOutCoroutine(Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeStart = null, System.Action fadeEnd = null)
        {
            fadeStart?.Invoke();
            float alpha = 1f;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            image.gameObject.SetActive(true);

            if (onlyActiveSelf)
            {
                for (int i = 0; i < image.transform.childCount; i++)
                {
                    image.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            while (image.color.a > 0f)
            {
                alpha -= Time.fixedUnscaledDeltaTime / time;
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
            fadeEnd?.Invoke();
            yield break;
        }

        public void AddMissile(Transform target)
        {
            OnMissileAdded?.Invoke(target);
        }

        public bool IsPaused { get; set; } = false;
        public void PauseTrigget()
        {
            if(IsPaused)
            {
                IsPaused = false;
                ResumeGame(1f, 1f);
            }
            else
            {
                IsPaused = true;
                PauseGame(1f, 1f);
            }
        }

        private void PauseGame(float fadeInDelay, float pauseGameDelay)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            _playerInputCustom.isControlable = false;
            StartCoroutine(DelayedCall(fadeInDelay, _gameResultUi.FadeIn));
        }
        private void ResumeGame(float fadeInDelay, float pauseGameDelay)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            _playerInputCustom.isControlable = true;
            StartCoroutine(DelayedCall(fadeInDelay, _gameResultUi.FadeOutResultUi));
        }


    }
}
