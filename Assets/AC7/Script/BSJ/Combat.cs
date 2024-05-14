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
    [SerializeField] private Transform _owner;
    [SerializeField] private float _maxHp = 100f;
    [SerializeField] private float _hp = 100f;
    [SerializeField] private bool _dead = false;
    [SerializeField] private float _invincibleTime = .0f;
    [SerializeField] private float _prevHitTime = 0f;
    [SerializeField] private Combat _prevAttacker = null;


    /// <summary>
    /// ���� �޾����� �߻��ϴ� �̺�Ʈ
    /// </summary>
    public System.Action OnDamaged { get; set; }
    /// <summary>
    /// ������ ���������� �߻��ϴ� �̺�Ʈ Ÿ���� �μ��� �޴´�.
    /// </summary>
    public System.Action<Combat> OnAttackSuccess { get; set; }
    /// <summary>
    /// �׾����� �߻��ϴ� �̺�Ʈ
    /// </summary>
    public System.Action OnDead { get; set; }
    /// <summary>
    /// �������� ������ �ִ��� ������ �߰��� �Ǵ��ϴ� �̺�Ʈ
    /// </summary>
    public System.Func<bool> AdditionalDamageableCondition { get; set; }
    /// <summary>
    /// ���� �Ǿ����� �߻��ϴ� �̺�Ʈ
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
    }
    /// <summary>
    /// ���� Hp�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public float GetHp() { return _hp; }
    /// <summary>
    /// MaxHp�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public float GetMaxHp()
    {
        return _maxHp;
    }
    /// <summary>
    /// ü���� �ʱ�ȭ�ϰ� dead �� �ʱ�ȭ��
    /// </summary>
    public void ResetHp()
    {
        _hp = _maxHp;
        _dead = false;
    }
    /// <summary>
    /// �����ڰ� �ִ°�� �������� �����ϴ� �޼���
    /// </summary>
    /// <param name="damage"> ������ ������ </param>
    /// <param name="target"> ���ݹ��� Ÿ�� </param>
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
    /// �������� �ִ� �޼���
    /// </summary>
    /// <param name="damage">������</param>
    /// <param name="attacker">������</param>
    /// <returns>������ ���뿡 ������ ��� <see langword="false"/>��ȯ (�̹��׾��ų� �߰����� ����)</returns>
    public bool TakeDamage(float damage, Combat attacker = null)
    {
        if (!IsDamageable())
            return false;

        _prevAttacker = attacker;

        CalcTakeDamage(damage);
        return true;
    }
    /// <summary>
    /// ü���� ȸ���ϴ� �޼���
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
    /// �׾����� Ȯ���ϴ� �޼���
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return _dead;
    }
    /// <summary>
    /// ������ �װ��ϴ� �޼���
    /// </summary>
    public void Die()
    {
        TakeDamage(_hp);
    }


    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTime) // ���� ��������
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }

        //�߰� ������ ������� �߰� ���� �Ǵ�
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