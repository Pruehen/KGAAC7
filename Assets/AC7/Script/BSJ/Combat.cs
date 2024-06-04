using Mirror;
using UnityEngine;

//combatComponent�� ������ �༮���� �����ϱ� ����
public interface IFightable
{
    /// <summary>
    /// �����ڰ� �ִ°�� ����ϴ� �޼���
    /// �˾Ƽ� Ÿ�ٿ��� �������� ������
    /// </summary>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    void DealDamage(IFightable target, float damage);
    /// <summary>
    /// �����ڰ� ���°�� �������� ���ϴ� �޼���
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
    /// �ǰݽ� ����Ǵ� ���� �ð�
    /// </summary>
    [SerializeField] private float _invincibleTime = .0f;
    [SerializeField] private float _prevHitTime = 0f;

    /// Action�� Func
    /// �ܺο��� On~ += �޼����̸�;
    /// ���� ������ �̺�Ʈ Invoke�� ������ �޼������ ȣ��ȴ�

    /// <summary>
    /// ������ �Ծ����� ����Ǵ� �̺�Ʈ
    /// </summary>
    public System.Action OnDamaged { get; set; }
    /// <summary>
    /// ������ ���������� �߻��ϴ� �̺�Ʈ Ÿ���� �μ��� �޴´�.
    /// </summary>
    public System.Action<Combat> OnAttackWithTarget { get; set; }
    /// <summary>
    /// ����� ����Ǵ� �̺�Ʈ
    /// </summary>
    public System.Action OnDead { get; set; }

    /// <summary>
    /// ������ ������ �߰��������� �������� �����ϴ� �̺�Ʈ
    /// true �� ��ȯ�ϸ� �������� ������ �ְ�
    /// false �� �������� �����Ѵ�
    /// </summary>
    public System.Func<bool> AdditionalDamageableCondition { get; set; }
    /// <summary>
    /// �������� ����Ǵ� �̺�Ʈ
    /// </summary>
    public System.Action OnHeal { get; set; }
    /// <summary>
    /// �ʱ�ȭ, ������ Ʈ�������� �ִ�ü���� �ʱ�ȭ�Ѵ�.
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

    //���� ������ ���
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