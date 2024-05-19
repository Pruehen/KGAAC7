using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class SoundManager : SceneSingleton<SoundManager>
    {
        /// <summary>
        /// ���� �ҽ��� �ִ� �������� ������ ��ġ�� �����ϰ� �ν��Ͻ��� ��ȯ�Ѵ�
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
                StartCoroutine(DelayEnqueue(item, source.clip.length));
            return item;
        }
        public GameObject SpawnInPosition(GameObject prefab, Vector3 position, bool loop = false)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(prefab);
            item.transform.position = position;
            AudioSource source = item.GetComponent<AudioSource>();
            source.loop = loop;
            source.Stop();
            if (!loop)
                StartCoroutine(DelayEnqueue(item, source.clip.length));
            return item;
        }
        /// <summary>
        /// ���� �ҽ��� �ִ� �������� �޾� �����ϰ� ������ Ʈ�������� �ڽ����� �����Ѵ�
        /// ������ �ν��Ͻ��� ��ȯ�Ѵ�
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
    }
}