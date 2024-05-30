using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace bsj
{
    public class SoundManager : SceneSingleton<SoundManager>
    {
        [SerializeField] AudioMixer _audioMixer;
        private void Start()
        {
            bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
        }
        private void OnPlayerSpawn()
        {
            Reset();
            StartCoroutine(kjh.GameManager.Instance.DelayedCall(1f, StartFadeIn));
        }

        private void StartFadeIn()
        {
            StartCoroutine(FadinSfx());
        }
        private IEnumerator FadinSfx()
        {
            float vol = 0f;
            float fadeInTime = 3f;
            while (vol <= 1f)
            {
                yield return null;
                vol = vol + (Time.deltaTime / fadeInTime); 
                SetVolum(vol);
            }
            SetVolum(1f);
        }

        private void SetVolum(float vol)
        {
            _audioMixer.SetFloat("Volum", Mathf.Log10(vol) * 20f);
        }


        /// <summary>
        /// 사운드 소스가 있는 프리팹을 지정된 위치에 스폰하고 인스턴스를 반환한다
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject PlayInPosition(GameObject prefab, Vector3 position, bool loop = false)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(prefab);
            item.transform.position = position;
            AudioSource source = item.GetComponent<AudioSource>();
            source.loop = loop;
            source.Play();
            if (!loop)
                StartCoroutine(DelayEnqueue(item, source.clip.length + 4f));
            return item;
        }
        public GameObject SpawnInPosition(GameObject prefab, Vector3 position, bool loop = false)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(prefab);
            item.transform.position = position;
            AudioSource source = item.GetComponent<AudioSource>();
            source.loop = loop;
            if (!loop)
                StartCoroutine(DelayEnqueue(item, source.clip.length));
            return item;
        }
        /// <summary>
        /// 사운드 소스가 있는 프리팹을 받아 스폰하고 지정된 트랜스폼의 자식으로 지정한다
        /// 생성된 인스턴스를 반환한다
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public GameObject PlayAttached(GameObject prefab, Transform parent, bool loop = false)
        {
            GameObject item = PlayInPosition(prefab, Vector3.zero, loop);
            item.transform.SetParent(parent, false);
            return item;
        }

        public GameObject SpawnAttached(GameObject prefab, Transform parent, bool loop = false)
        {
            GameObject item = SpawnInPosition(prefab, Vector3.zero, loop);
            item.transform.SetParent(parent, false);
            return item;
        }

        public void PlayRandom(GameObject randSfx)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(randSfx);
            ObjectPoolManager.Instance.EnqueueObject(item, 10f);
        }

        private IEnumerator DelayEnqueue(GameObject item ,float time)
        {
            yield return new WaitForSeconds(time);
            ObjectPoolManager.Instance.EnqueueObject(item);
        }

        public void Reset()
        {
            SetVolum(0f);
        }
    }
}