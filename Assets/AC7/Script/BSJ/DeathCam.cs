using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class DeathCam : MonoBehaviour
    {
        Vector3 _deathcamTarget;
        Camera _cam;

        void Start()
        {
            _cam = Camera.main;
            Vector3 offset = Vector3.up * 500f;
            _deathcamTarget = kjh.GameManager.Instance.player.transform.position + offset;
            GetComponent<VehicleCombat>().onDead.AddListener(Play);
        }

        public void Play()
        {
            StartCoroutine(MoveCam(_deathcamTarget, 60f));
        }

        private IEnumerator MoveCam(Vector3 toPos, float rotateSpeed)
        {
            while (true)
            {
                yield return null;
                _cam.transform.LookAt(kjh.GameManager.Instance.player.transform.position);
                Vector3 vel = Vector3.zero;
                _cam.transform.position += Vector3.up * Time.deltaTime * 100f;
            }
        }
    }

}