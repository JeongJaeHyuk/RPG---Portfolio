using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_E : MonoBehaviour
{
    private Skill skillData; // 스킬 데이터 참조
    [SerializeField] BoxCollider damageCol; // 데미지콜라이더
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] PlayerProgression pps;

    float speed = 7f; // 투사체 속도
    GameObject oriPos; // 활성화끝날떄 다시갈 포지션
    [SerializeField] bool moveOn; // 투사체 움직이는거 설정할때

    [Header("=== 스킬 설정 ===")]
    [SerializeField] string skillName = "Blade storm"; // 스킬 이름 (Inspector에서 설정)

    [Header("=== 이 타격의 데미지 배율 ===")]
    [Tooltip("0.1 = 10%, 0.6 = 60% | 총합이 1.0(100%)이 되도록 설정")]
    [SerializeField] float damageMultiplier = 0.2f; // 기본 20%

    private void Awake()
    {
        damageCol = GetComponent<BoxCollider>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        plsp = playerObj.GetComponent<PlayerSpecs>();
        pps = playerObj.GetComponent<PlayerProgression>();
    }

    private void Start()
    {
        oriPos = gameObject.transform.parent.gameObject;
    }

    private void OnEnable()
    {
        moveOn = true;
        damageCol.enabled = true;
    }

    private void OnDisable()
    {
        moveOn = false;
        damageCol.enabled = false;
        transform.position = oriPos.transform.position;
    }

    private void Update()
    {
        if (moveOn)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
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
        float damage = plsp.TOTAL_DAMAGE * skillDamage * damageMultiplier;
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
        //         Debug.Log($"Skill_E Hit! Damage: {damage}");
        //     }
        // }
    }
}
