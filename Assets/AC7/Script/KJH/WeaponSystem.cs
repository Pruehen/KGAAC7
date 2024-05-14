using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponSystem : NetworkBehaviour
    {
        [SerializeField] GameObject weaponPrf;
        [SerializeField] Transform fireTrf;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 현재 적용중인 프리팹을 발사하는 메서드
        /// </summary>
        /// <param name="aircraftVelocity"></param>
        /// <param name="target"></param>
        public void Fire(Vector3 aircraftVelocity, Transform target)
        {
            if(!isLocalPlayer) return;
            if(isServer)
            {
                GameObject item = Instantiate(weaponPrf, fireTrf.position, fireTrf.rotation);
                NetworkServer.Spawn(item);
                item.GetComponent<Rigidbody>().velocity = aircraftVelocity;

                Guided guided;
                if (item.TryGetComponent(out guided))
                {
                    guided.SetTarget(target);
                    Debug.Log("타겟 지정");
                }
            }
            else
            {
                CmdFire(aircraftVelocity, target);
            }
        }

        /// <summary>
        /// 서버에서 현재 적용중인 프리팹을 발사하는 메서드
        /// </summary>
        /// <param name="aircraftVelocity"></param>
        /// <param name="target"></param>
        [Command]
        public void CmdFire(Vector3 aircraftVelocity, Transform target)
        {
            RemoteFire(aircraftVelocity, target);
        }
        public void RemoteFire(Vector3 aircraftVelocity, Transform target)
        {
            if (isServer)
            {
                GameObject item = Instantiate(weaponPrf, fireTrf.position, fireTrf.rotation);
                NetworkServer.Spawn(item);
                item.GetComponent<Rigidbody>().velocity = aircraftVelocity;

                Guided guided;
                if (item.TryGetComponent(out guided))
                {
                    guided.SetTarget(target);
                    Debug.Log("타겟 지정");
                }
            }
        }
    }
}
