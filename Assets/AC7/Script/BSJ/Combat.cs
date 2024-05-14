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
    [SerializeField] private Transform _owner;
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;
    [SerializeField] private float _invincibleTime = .0f;
    [SerializeField] private float _prevHitTime = 0f;
    [SerializeField] private Combat _prevAttacker = null;


    /// <summary>
    /// 공격 받았을때 발생하는 이벤트
    /// </summary>
    public System.Action OnDamaged { get; set; }
    /// <summary>
    /// 공격을 성공했을때 발생하는 이벤트 타겟을 인수로 받는다.
    /// </summary>
    public System.Action<Combat> OnAttackSuccess { get; set; }
    /// <summary>
    /// 죽었을때 발생하는 이벤트
    /// </summary>
    public System.Action OnDead { get; set; }
    /// <summary>
    /// 데미지를 받을수 있는지 없는지 추가로 판단하는 이벤트
    /// </summary>
    public System.Func<bool> AdditionalDamageableCondition { get; set; }
    /// <summary>
    /// 힐이 되었을때 발생하는 이벤트
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
    }
    /// <summary>
    /// 현재 Hp를 반환
    /// </summary>
    /// <returns></returns>
    public float GetHp() { return _hp; }
    /// <summary>
    /// MaxHp를 반환
    /// </summary>
    /// <returns></returns>
    public float GetMaxHp()
    {
        return _maxHp;
    }
    /// <summary>
    /// 체력을 초기화하고 dead 도 초기화함
    /// </summary>
    public void ResetHp()
    {
        _hp = _maxHp;
        _dead = false;
    }
    /// <summary>
    /// 공격자가 있는경우 데미지를 적용하는 메서드
    /// </summary>
    /// <param name="damage"> 적용할 데미지 </param>
    /// <param name="target"> 공격받을 타겟 </param>
    /// <returns></returns>
    public bool DealDamage(float damage, Combat target)
    {
        bool isAttackSucceeded = target.TakeDamage(damage, this);
        if (isAttackSucceeded)
        {
            OnAttackSuccess?.Invoke(target);
            return false;
        }
        return true;
    }


    /// <summary>
    /// 데미지를 주는 메서드
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="attacker">공격자</param>
    /// <returns>데미지 적용에 실패할 경우 <see langword="false"/>반환 (이미죽었거나 추가조건 실패)</returns>
    public bool TakeDamage(float damage, Combat attacker = null)
    {
        if (!IsDamageable())
            return false;

        _prevAttacker = attacker;

        CalcTakeDamage(damage);
        return true;
    }
    /// <summary>
    /// 체력을 회복하는 메서드
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        if (_hp < _maxHp)
        {
            _hp += amount;
        }
        if (OnHeal != null)
        {
            OnHeal.Invoke();
        }
    }
    /// <summary>
    /// 죽었는지 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return _dead;
    }
    /// <summary>
    /// 강제로 죽게하는 메서드
    /// </summary>
    public void Die()
    {
        TakeDamage(_hp);
    }


    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTime) // 아직 무적인지
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }

        //추가 조건이 있을경우 추가 조건 판단
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