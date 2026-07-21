using UnityEngine;

// Skill_Q/W/E/R 공통 부모 — 스킬 이펙트(파티클) 오브젝트가 들고 있는 데미지 판정 스크립트
public class Skill_Effect : MonoBehaviour
{
    Skill skillData; // 스킬 데이터 참조
    [SerializeField] Collider damageCollider;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] PlayerProgression pps;

    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        plsp = playerObj.GetComponent<PlayerSpecs>();
        pps  = playerObj.GetComponent<PlayerProgression>();
    }

    protected virtual void OnEnable()
    {
        damageCollider.enabled = true;
    }

    protected virtual void OnDisable()
    {
        damageCollider.enabled = false;
    }

    // 스킬 데이터를 받아서 설정하는 함수 (SkillOn에서 호출)
    public void SetSkillData(Skill _skill)
    {
        skillData = _skill;
    }

    // 데미지 계산 (기본 공식). 타수별 배율이 있는 스킬(E/R)은 override해서 곱연산 추가
    public virtual float SkillAttack()
    {
        float skillDamage = skillData != null ? skillData.CURRENT_DAMAGE : 0;
        return plsp.TOTAL_DAMAGE * skillDamage;
    }

    // Monster_HitDamage에서 PlayerSkillAttack 레이어 감지 시 경험치 지급 대상 조회용
    public GameObject GetPlayer()
    {
        return plsp.gameObject;
    }
}
