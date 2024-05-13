using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kjh
{
    public class WeaponSystem : MonoBehaviour
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
        /// ���� �������� �������� �߻��ϴ� �޼���
        /// </summary>
        /// <param name="aircraftVelocity"></param>
        /// <param name="target"></param>
        public void Fire(Vector3 aircraftVelocity, Transform target)
        {
            GameObject item = Instantiate(weaponPrf, fireTrf.position, fireTrf.rotation);
            item.GetComponent<Rigidbody>().velocity = aircraftVelocity;

            Guided guided;
            if(item.TryGetComponent(out guided))
            {
                guided.SetTarget(target);
                Debug.Log("Ÿ�� ����");
            }
        }
    }
}
