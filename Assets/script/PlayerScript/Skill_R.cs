using UnityEngine;

public class Skill_R : Skill_Effect
{
    [Header("=== 이 타격의 데미지 배율 ===")]
    [Tooltip("0.1 = 10%, 0.6 = 60% | 총합이 1.0(100%)이 되도록 설정")]
    [SerializeField] float damageMultiplier = 0.2f; // 기본 20%
    // 배율 설정은 인스펙터창에서 각각 설정맞춰줘야함
    // 데미지 계산 (기본 공식 × 이 타격의 배율)
    public override float SkillAttack()
    {
        return base.SkillAttack() * damageMultiplier;
    }
}
