using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_R : MonoBehaviour
{
    private Skill skillData; // 스킬 데이터 참조
    [SerializeField] BoxCollider damageSensor;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] PlayerProgression pps;

    [Header("=== 이 타격의 데미지 배율 ===")]
    [Tooltip("0.1 = 10%, 0.6 = 60% | 총합이 1.0(100%)이 되도록 설정")]
    [SerializeField] float damageMultiplier = 0.2f; // 기본 20%

    private void Awake()
    {
        damageSensor = GetComponent<BoxCollider>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        plsp = playerObj.GetComponent<PlayerSpecs>();
        pps = playerObj.GetComponent<PlayerProgression>();
    }

    private void OnEnable()
    {
        damageSensor.enabled = true;
    }

    private void OnDisable()
    {
        damageSensor.enabled = false;
    }

    // 스킬 데이터를 받아서 설정하는 함수 (SkillOn에서 호출)
    public void SetSkillData(Skill _skill)
    {
        skillData = _skill;
    }

    // 데미지 계산 (스킬 데미지 × 배율)
    public float SkillAttack()
    {
        float skillDamage = skillData != null ? skillData.CURRENT_DAMAGE : 0;
        float damage = plsp.TOTAL_DAMAGE * skillDamage * damageMultiplier;
        return damage;
    }

    // 몬스터와 충돌 시 데미지 전달
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer).Equals("Monster"))
        {
            Monster_Attack_Hit monster = other.GetComponent<Monster_Attack_Hit>();
            if (monster != null)
            {
                float damage = SkillAttack();
                monster.TakeDamage(damage, plsp, pps);
                Debug.Log($"Skill_R Hit! Damage: {damage} (배율: {damageMultiplier * 100}%)");
            }
        }
    }
}
