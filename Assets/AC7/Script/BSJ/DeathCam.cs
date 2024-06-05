using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bsj
{
    public class DeathCam : NetworkBehaviour
    {
        Transform _playerTrf;
        Camera _cam;
        Vector3 _initialOffset;
        Quaternion _initialLocalRotation;
        Transform _initCamParent;

        private void Start()
        {
            bsj.GameManager.Instance.AfterPlayerSpawned += OnPlayerSpawn;
        }
        private void OnPlayerSpawn()
        {
            _cam = Camera.main;
            _initCamParent = Camera.main.transform.parent;
            _initialOffset = _cam.transform.localPosition;
            _initialLocalRotation = _cam.transform.localRotation;
            Vector3 offset = Vector3.up * 500f;
            if (isLocalPlayer)
            {
                GetComponent<VehicleCombat>().onDead.AddListener(Play);
            }
            _playerTrf = bsj.GameManager.Instance.player;
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

                //Camera.main.transform.position.DrawSphere(3f, Color.red);
                _cam.transform.LookAt(_playerTrf.position);
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
        public void ResetDead()
        {
            _cam.transform.SetParent(_initCamParent);
            _cam.transform.localPosition = _initialOffset;
            _cam.transform.localRotation = _initialLocalRotation;
        }
    }

}