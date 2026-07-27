using System.Collections;
using UnityEngine;

public class Boss_Attack_Effect : MonoBehaviour
{
    [SerializeField] protected Collider hitCollider;
    [SerializeField] protected float damageRatio = 1f;  // 보스 공격력 대비 배율 (n%)
    [SerializeField] protected float hitDuration = 0.5f; // 콜라이더가 켜져있는 시간
    [SerializeField] protected float lifeTime = 1.5f;    // 풀로 복귀하기까지 총 시간

    protected string patternName;
    protected float damage;

    public void Activate(string _patternName, float _bossDamage)
    {
        patternName = _patternName;
        damage = _bossDamage * damageRatio;
        StartCoroutine(Play());
    }

    // 데미지 전달 패턴: 받는 쪽(추후 구현할 플레이어 피격 처리)이 이 함수를 호출해서 데미지를 가져간다
    public float GetDamage()
    {
        return damage;
    }

    protected virtual IEnumerator Play()
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(hitDuration);
        hitCollider.enabled = false;

        float remain = lifeTime - hitDuration;
        if (remain > 0)
        {
            yield return new WaitForSeconds(remain);
        }

        BossEffectManager.instance.ReturnEffect(patternName, this);
    }
}
