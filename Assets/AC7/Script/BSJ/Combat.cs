using Mirror;
using UnityEngine;

//combatComponent를 가지는 녀석들을 구분하기 위함
public interface IFightable
{
    /// <summary>
    /// 공격자가 있는경우 사용하는 메서드
    /// 알아서 타겟에게 데미지를 가해줌
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    void DealDamage(IFightable target, float damage);
    /// <summary>
    /// 공격자가 없는경우 데미지를 가하는 메서드
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(float damage);
}

[System.Serializable]
public class Combat
{
    public Transform _owner;
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;
    /// <summary>
    /// 피격시 적용되는 무적 시간
    /// </summary>
    [SerializeField] private float _invincibleTime = .0f;
    [SerializeField] private float _prevHitTime = 0f;

    /// Action과 Func
    /// 외부에서 On~ += 메서드이름;
    /// 으로 구독후 이벤트 Invoke시 구독된 메서드들이 호출된다

    /// <summary>
    /// 데미지 입었을때 실행되는 이벤트
    /// </summary>
    public System.Action OnDamaged { get; set; }
    /// <summary>
    /// 공격을 성공했을때 발생하는 이벤트 타겟을 인수로 받는다.
    /// </summary>
    public System.Action<Combat> OnAttackWithTarget { get; set; }
    /// <summary>
    /// 사망시 실행되는 이벤트
    /// </summary>
    public System.Action OnDead { get; set; }

    /// <summary>
    /// 데미지 입을시 추가조건으로 데미지를 무시하는 이벤트
    /// true 를 반환하면 데미지를 입을수 있고
    /// false 면 데미지를 무시한다
    /// </summary>
    public System.Func<bool> AdditionalDamageableCondition { get; set; }
    /// <summary>
    /// 힐했을때 실행되는 이벤트
    /// </summary>
    public System.Action OnHeal { get; set; }
    /// <summary>
    /// 초기화, 소유자 트랜스폼과 최대체력을 초기화한다.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="maxHp"></param>
    /// <param name="defaultEffectOnDamaged"></param>
    public void Init(Transform owner, float maxHp)
    {
        _owner = owner;
        _maxHp = maxHp;
        _hp = _maxHp;
        _dead = false;
    }
    public float GetHp() { return _hp; }
    public float GetMaxHp()
    {
        return _maxHp;
    }
    public bool DealDamage(Combat target, float damage)
    {
        bool isAttackSucceeded = target.TakeDamage(_owner.transform.position, damage);
        if (isAttackSucceeded)
        {
            OnAttackWithTarget?.Invoke(target);
            return false;
        }
        return true;
    }
    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTime)
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }
        bool result = true;
        if (AdditionalDamageableCondition != null)
        {
            result = result && AdditionalDamageableCondition.Invoke();
        }
        if (!result)
        {
            return false;
        }
        return true;
    }
    public bool TakeDamage(float damage)
    {
        return TakeDamage(_owner.position, damage);
    }
    public bool TakeDamage(Vector3 position, float damage, int effectType = -1)
    {
        if (!IsDamageable())
            return false;
        CalcTakeDamage(damage);;
        return true;
    }
    public void Heal(float v)
    {
        if (_hp < _maxHp)
        {
            _hp += v;
        }
        if (OnHeal != null)
        {
            OnHeal.Invoke();
        }
    }
    public bool IsDead()
    {
        return _dead;
    }
    public void Die()
    {
        TakeDamage(_owner.position, _hp);
    }
    public void Reset()
    {
        _hp = _maxHp;
        _dead = false;
    }

    //실제 데미지 계산
    private void CalcTakeDamage(float damage)
    {
        _prevHitTime = Time.time;
        _hp -= damage;
        OnDamaged?.Invoke();
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke();
        }
    }
}