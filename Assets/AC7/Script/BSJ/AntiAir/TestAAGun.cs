//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TestAAGun : MonoBehaviour
//{
//    [SerializeField] private Transform _firePos;
//    [SerializeField] private float _fireInterval = 0.01f;
//    private float _GunInterverTimer = 0f;

//    [SerializeField] private GameObject _bulletProjectile;
//    [SerializeField] private Rigidbody _target;
//    public float _bulletSpeed = 1050;

//    public int accLoop = 10;
//    public int testMode;

//    private void Awake()
//    {
//        _firePos = transform;
//        _bulletSpeed = _bulletProjectile.GetComponent<bsj.Bullet>()._bulletSpeed;
//    }

//    Vector3 prevVel = Vector3.zero;
//    private void Update()
//    {
//        if(prevVel == Vector3.zero)
//            prevVel = _target.velocity;
//        _GunInterverTimer += Time.deltaTime;

//        Vector3 VelDelta = _target.velocity - prevVel;
//        switch (testMode)
//        {
//            case 0:
//                //transform.rotation = Quaternion.LookRotation(FireControlSystem.CalcFireDirection(transform.position, _target, _bulletSpeed, accLoop));
//                break;
//            case 1:
//                transform.rotation = Quaternion.LookRotation(FireControlSystem.CalcFireDirection(transform.position, _target, _bulletSpeed, accLoop, VelDelta));
//                break;
//            case 2:
//                transform.rotation = Quaternion.LookRotation(FireControlSystem.CalcFireDirection(transform.position, _target, _bulletSpeed, accLoop, VelDelta));
//                break;
//        }


//        if (_GunInterverTimer >= _fireInterval)
//        {
//            _GunInterverTimer = 0f;
//            Fire();
//        }
//        prevVel = _target.velocity;
//    }

//    public void Fire()
//    {
//        GameObject item = ObjectPoolManager.Instance.DequeueObject(_bulletProjectile);
//        bsj.Bullet bullet = item.GetComponent<bsj.Bullet>();
//        bullet.transform.position = _firePos.position;
//        bullet.transform.rotation = _firePos.rotation;
//        bullet.Init();
//        ObjectPoolManager.Instance.EnqueueObject(item, 30f);
//    }
//    public void FireTest(Vector3 pos, Quaternion rot)
//    {
//        GameObject item = ObjectPoolManager.Instance.DequeueObject(_bulletProjectile);
//        bsj.Bullet bullet = item.GetComponent<bsj.Bullet>();
//        bullet.transform.position = pos;
//        bullet.transform.rotation = rot;
//        bullet.Init();
//        ObjectPoolManager.Instance.EnqueueObject(item, 30f);
//    }

//}
