using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_W : MonoBehaviour
{
    private Skill skillData; // 스킬 데이터 참조
    [SerializeField] SphereCollider damageSensor;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] PlayerProgression pps;

    private void Awake()
    {
        damageSensor = GetComponent<SphereCollider>();
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

    // 데미지 계산
    public float SkillAttack()
    {
        float skillDamage = skillData != null ? skillData.CURRENT_DAMAGE : 0;
        float damage = plsp.TOTAL_DAMAGE * skillDamage;
        return damage;
    }

    // 몬스터와 충돌 시 데미지 전달
    private void OnTriggerEnter(Collider other)
    {
        // if (LayerMask.LayerToName(other.gameObject.layer).Equals("Monster"))
        // {
        //     Monster_Attack_Hit monster = other.GetComponent<Monster_Attack_Hit>();
        //     if (monster != null)
        //     {
        //         float damage = SkillAttack();
        //         monster.TakeDamage(damage, plsp, pps);
        //         Debug.Log($"Skill_W Hit! Damage: {damage}");
        //     }
        // }
    }
}
