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
                float offset = 20 * Time.deltaTime;
                //if(_playerTrf.position.y > 0f)
                //{
                //}
                //else
                //{
                //    _cam.transform.position = new Vector3(_playerTrf.position.x, _cam.transform.position.y, _playerTrf.position.z) + initialOffset;
                //}
                _cam.transform.position += Vector3.up * offset;

            }
        }
    }

}