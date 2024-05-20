using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class DeathCam : MonoBehaviour
    {
        Transform _playerTrf;
        Camera _cam;

        void Start()
        {
            _cam = Camera.main;
            Vector3 offset = Vector3.up * 500f;
            GetComponent<VehicleCombat>().onDead.AddListener(Play);
            _playerTrf = kjh.GameManager.Instance.player.transform;
        }

        public void Play()
        {
            Camera.main.transform.SetParent(null, true);
            StartCoroutine(MoveCam( 60f));
        }

        private IEnumerator MoveCam(float rotateSpeed)
        {
            Vector3 initialOffset =  -_playerTrf.position + _cam.transform.position;
            while (true)
            {
                yield return null;

                Camera.main.transform.position.DrawSphere(3f, Color.red);
                _cam.transform.LookAt(kjh.GameManager.Instance.player.transform.position);
                if(_playerTrf == null)
                    yield break;
                float offset = 100f * Time.deltaTime;
                _cam.transform.position = _playerTrf.position + initialOffset;
                initialOffset += offset * Vector3.up;
            }
        }
    }

}