using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M163 : MonoBehaviour
{
    [SerializeField] private Vector3 _desiredDirection;

    [SerializeField] private Transform _firePos;
    private Rigidbody _target;

    Transform _turret;
    Transform _gun;


    [SerializeField] public float _bulletSpeed;

    Vulcan _vulcan;
    private void Awake()
    {
        _turret = transform.GetChild(1);
        _gun = transform.GetChild(1).GetChild(0);
        _vulcan = _gun.GetComponent<Vulcan>();
        _bulletSpeed = _vulcan.bulletSpeed;
    }

    private void Start()
    {
        bsj.GameManager.Instance.OnPlayerSpawned += Init;
    }

    private void Init()
    {
        _target = bsj.GameManager.Instance.player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(_target.position, transform.position) < 3000f)
        {
            _desiredDirection = FireControlSystem.CalcFireDirection(_firePos.position, _target, _bulletSpeed);
            Vector3 targetRot = Quaternion.LookRotation(_desiredDirection).eulerAngles;
            _turret.rotation = Quaternion.Euler(new Vector3(0f, targetRot.y, 0f));
            _gun.localRotation = Quaternion.Euler(new Vector3(Mathf.Asin(_desiredDirection.y) * - Mathf.Rad2Deg, 0f, 0f));
            _vulcan.Fire();
        }
    }
}
