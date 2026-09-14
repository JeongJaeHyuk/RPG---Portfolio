using System.Collections;
using UnityEngine;

// 보스 패턴 하나(Breath/Slash Attack 등)의 데미지 판정 + 대기 상태 복귀 관리 - BossEffectManager가 오브젝트 하나를 계속 재사용한다.
// 호출 순서: 애니메이션 이벤트가 Monster_State_Boss의 XXXOn() 호출
//         -> BossEffectManager.PlayEffect()가 위치/방향 잡고 SetActive(true) + Activate() 호출
//         -> 여기 Activate()가 Play() 코루틴 시작 -> 콜라이더 켰다 끄고 -> 끝나면 대기 상태로 복귀
// 즉 Inspector 세팅만 맞춰두면 이 클래스 쪽에서 따로 호출할 건 없다 - 전부 위 체인 안에서 자동으로 진행됨.
public class Boss_Attack_Effect : MonoBehaviour
{
    [SerializeField] protected Collider hitCollider;      // 데미지 판정용 콜라이더 (플레이어 쪽 OnTriggerEnter가 감지)
    [SerializeField] protected float damageRatio = 1f;    // 보스 공격력(Monster_Spec.MONSTER_DAMAGE) 대비 배율(n%) - 예: 0.2면 공격력의 20%가 이번 타격 데미지
    [SerializeField] protected float hitDuration = 0.5f;  // 콜라이더가 켜져있는 시간
    [SerializeField] protected float lifeTime = 1.5f;     // 대기 상태로 복귀하기까지 총 시간(파티클 잔여 재생 포함)

    protected string patternName;
    protected float damage;

    // BossEffectManager.PlayEffect()가 위치/방향 다 세팅한 직후 호출
    public void Activate(string _patternName, float _bossDamage)
    {
        patternName = _patternName;
        damage = _bossDamage * damageRatio; // 실제 데미지 = 보스 공격력 * damageRatio, 여기서 한 번만 계산해서 고정
        StartCoroutine(Play());
    }

    // 데미지 전달 패턴: 공격 쪽(여기)은 데미지 값만 갖고 있고, 받는 쪽(플레이어)이 OnTriggerEnter에서
    // 이 함수를 호출해 가져가서 자기 체력을 깎는다 - Monster_HitDamage가 몬스터 공격을 받는 방식과 동일.
    // 주의: 지금 Player.cs의 MonsterAttack 레이어 처리가 아직 빈 스텁(케이스만 있고 실제 처리 없음)이라
    // 이 함수를 호출하는 쪽이 아직 없다 - 플레이어 피격 처리를 따로 구현해야 실제로 체력이 깎인다.
    public float GetDamage()
    {
        return damage;
    }

    protected virtual IEnumerator Play()
    {
        hitCollider.enabled = true;  // 이 구간에서만 데미지 판정이 유효함
        yield return new WaitForSeconds(hitDuration);
        hitCollider.enabled = false;

        float remain = lifeTime - hitDuration;
        if (remain > 0)
        {
            yield return new WaitForSeconds(remain); // 파티클이 마저 재생될 시간만큼 기다렸다가 반환
        }

        BossEffectManager.instance.ReturnEffect(patternName, this); // 대기 상태로 복귀 (SetActive(false)까지 처리됨)
    }
}
