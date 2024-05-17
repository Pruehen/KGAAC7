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
        public GameObject PlayInPosition(GameObject prefab, bool loop = false)
        {
            GameObject item = ObjectPoolManager.Instance.DequeueObject(prefab);
            AudioSource source = item.GetComponent<AudioSource>();
            source.loop = loop;
            source.Play();
            if(!loop)
                StartCoroutine(DelayDequeue(source.clip.length));
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
            GameObject item = PlayInPosition(prefab, loop);
            item.transform.SetParent(parent, false);
            return item;
        }

        private IEnumerator DelayDequeue(float time)
        {
            yield return new WaitForSeconds(time);
        }
    }
}