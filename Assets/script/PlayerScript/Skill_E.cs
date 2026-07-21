using UnityEngine;

public class Skill_E : Skill_Effect
{
    float speed = 7f;    // 투사체 속도
    GameObject oriPos;   // 비활성화될 때 되돌아갈 위치
    bool moveOn;         // 투사체 이동 여부

    [Header("=== 스킬 설정 ===")]
    [SerializeField] string skillName = "Blade storm"; // 스킬 이름 (Inspector에서 설정)

    [Header("=== 이 타격의 데미지 배율 ===")]
    [Tooltip("0.1 = 10%, 0.6 = 60% | 총합이 1.0(100%)이 되도록 설정")]
    // 배율 설정은 인스펙터창에서 각각 설정맞춰줘야함
    [SerializeField] float damageMultiplier = 0.2f; // 기본 20%

    void Start()
    {
        oriPos = transform.parent.gameObject;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        moveOn = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        moveOn = false;
        transform.position = oriPos.transform.position;
    }

    void Update()
    {
        if (moveOn)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // 데미지 계산 (기본 공식 × 이 타격의 배율)
    public override float SkillAttack()
    {
        return base.SkillAttack() * damageMultiplier;
    }
}
