using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_State_Boss : Monster_State
{
    // 보스 몬스터 - 일반공격 없이 전부 패턴(스킬)으로 처리
    // OrcKing_Animation_Controller 파라미터 기준:
    //  Move(int 0/1) - 그대로 상속 사용
    //  Skill_Attack(int) : Idle/Walk에서 1=Sword And Shield Slash, 2=TurnSlash 바로 발동(무쿨타임) - meleeSkillIds
    //  Attack_Ready(trigger) : Shout 0(예비동작)으로 진입 후, 그 안에서 Skill_Attack 3=Jumping, 4=Breath 중 가중치 랜덤(farSkillWeight)으로 분기
    //  (샤우트 쿨타임 게이트, 쿨타임은 스킬이 끝난 시점부터 다시 시작) - closeSkillIds/farSkillIds
    //
    // MONSTER_RANGE(CSV)는 근접 사거리 전용으로 원본 그대로 사용 - 근접 패턴은 지금까지처럼
    // "다가가서 멈추면 EnterAttack()" 흐름을 그대로 탄다. 이동/회전 로직은 Monster_Move가 아니라 보스 전용
    // Boss_Move(sibling)를 따로 써서 일반 몹에 영향 안 가게 분리했다. 샤우트(원거리 포함) 패턴은 Boss_Move를
    // 건드리지 않기 위해 이 클래스 자체 Update()에서 별도 사거리(shoutRange)로 독립적으로 체크한다.

    [Header("패턴 발동 위치")]
    [SerializeField] Transform breathPoint;
    [SerializeField] Transform jumpPoint;  // 점프 착지 이펙트 발동 위치 - 발밑(Y=0 근처)에 둘 것

    [Header("근접 패턴 목록")]
    [SerializeField] int[] meleeSkillIds = { 1, 2 }; // 1: Sword And Shield Slash, 2: TurnSlash - 나중에 늘어나면 배열에 추가만 하면 됨(개수 제한 없음)

    [Header("샤우트 패턴")]
    [SerializeField] float shoutCooldown = 10f;
    [SerializeField] float shoutRange = 8f;         // 이 거리 안이면 쿨타임 됐을 때 샤우트 발동 후보
    [SerializeField] float farSkillWeight = 0.6f;   // 샤우트 발동 시 farSkillIds(Jumping)가 나올 확률 - 나머지는 closeSkillIds(Breath)
    [SerializeField] int[] closeSkillIds = { 4 };    // 4: Breath - 나중에 늘어나면 배열에 추가만 하면 됨
    [SerializeField] int[] farSkillIds = { 3 };      // 3: Jumping - 나중에 늘어나면 배열에 추가만 하면 됨

    float lastShoutTime = -999f;
    int selectedFarSkill; // farSkillIds 중 이번에 고른 id - 이펙트 이름 매핑용(새 원거리 패턴 배선 시 사용)
    Boss_Attack_Effect jumpContainer; // JumpingOn()이 위치 잡아둔 컨테이너 - 자식(Jumping_Effect/Spike_Effect) 순차 재생에 사용
    bool isShoutPattern; // 이번 공격이 샤우트(Breath/Jumping) 패턴인지 - ExitAttack에서 쿨타임 리셋 여부를 구분하기 위함(근접 패턴 종료 시엔 리셋하면 안 됨)

    // 등장 연출 중 오작동 방지 - PlayerSensor 콜라이더를 안 꺼도 코드로 한 번 더 막아주는 안전장치
    bool combatStarted;

    Boss_Move monsterMove;

    protected override void Awake()
    {
        base.Awake();
        monsterMove = GetComponent<Boss_Move>();
    }

    // 근접 사거리(Boss_Move가 감지) 안까지 다가왔을 때 호출 - 근접 패턴만 담당
    public override void EnterAttack()
    {
        isAttacking = true;
        isShoutPattern = false;
        ani.SetInteger("Move", 0);
        ani.SetInteger("Skill_Attack", meleeSkillIds[Random.Range(0, meleeSkillIds.Length)]); // Sword And Shield Slash / TurnSlash 중 랜덤
    }

    // "Shout"(등장) State 전용 - 클립을 공유하는 "Shout 0"엔 안 걸려서 전투 중엔 호출 안 됨
    public void StartCombat()
    {
        combatStarted = true;

        // 리셋 안 하면 전투 시작하자마자 샤우트 패턴이 바로 나감 - 첫 쿨타임만큼 근접만 나오게 하려는 의도적 설정
        lastShoutTime = Time.time;

        Collider sensorCollider = monsterRange.GetComponent<Collider>();
        if (sensorCollider != null)
        {
            sensorCollider.enabled = true;
        }
    }

    // Boss_Move와 별개로 샤우트(원거리 포함) 패턴만 자체적으로 감시 - 추격 중이든 멈춰있든 조건 맞으면 바로 발동
    void Update()
    {
        if (!combatStarted || GetIsDead() || GetIsAttacking() || GetIsIdle())
        {
            return;
        }

        GameObject player = GetPlayer();
        if (player == null)
        {
            return;
        }

        if (Time.time - lastShoutTime < shoutCooldown)
        {
            return;
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist > shoutRange)
        {
            return;
        }

        isAttacking = true;
        isShoutPattern = true;
        monsterMove.nav.updateRotation = false; // 공격 중 NavMeshAgent가 회전을 되돌리지 못하게 차단
        monsterMove.nav.updatePosition = false; // 원거리 패턴의 root motion 이동도 NavMeshAgent가 취소하지 못하게 차단
        ani.SetInteger("Move", 0);
        monsterMove.SnapToPlayer(); // Boss_Move의 ChaseMove를 거치지 않는 경로라 회전을 직접 호출 - 한 번만 불리므로 순간 스냅 버전 사용
        ani.SetTrigger("Attack_Ready");

        if (Random.value < farSkillWeight)
        {
            selectedFarSkill = farSkillIds[Random.Range(0, farSkillIds.Length)]; // Jumping
            ani.SetInteger("Skill_Attack", selectedFarSkill);
        }
        else
        {
            ani.SetInteger("Skill_Attack", closeSkillIds[Random.Range(0, closeSkillIds.Length)]); // Breath
        }
    }

    // 공격 애니메이션이 끝날 때 애니메이션 이벤트로 호출 - Skill_Attack을 0으로 되돌려야 Idle에서 즉시 재발동하지 않는다
    public override void ExitAttack()
    {
        isAttacking = false;
        ani.SetInteger("Skill_Attack", 0);

        if (isShoutPattern)
        {
            lastShoutTime = Time.time; // 쿨타임을 스킬이 끝난 시점부터 다시 계산 (트리거 시점이 아니라)
        }
    }

    // 아래 함수들은 각 패턴 애니메이션의 타격 프레임에 애니메이션 이벤트로 연결해서 사용
    public void BreathOn()
    {
        BossEffectManager.instance.PlayEffect("Breath", breathPoint, monsterSpec.MONSTER_DAMAGE);
    }

    // 점프 착지 순간 애니메이션 이벤트로 연결 - 점프 이펙트(연출) 먼저 재생하고 0.3초 뒤 스파이크(데미지) 재생
    public void JumpingOn()
    {
        jumpContainer = BossEffectManager.instance.PlayEffect("Jumping", jumpPoint, monsterSpec.MONSTER_DAMAGE);
        jumpContainer.transform.SetParent(null, true); // 위치만 한 번 받고 jumpPoint에서 분리 - 이후 보스가 움직여도 스파이크는 착지 지점에 고정
        StartCoroutine(JumpImpactSequence());
    }

    IEnumerator JumpImpactSequence()
    {
        Transform jumpEffect = jumpContainer.transform.Find("Jumping_Effect");
        jumpEffect.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        Transform spikeEffect = jumpContainer.transform.Find("Spike_Effect");
        spikeEffect.gameObject.SetActive(true);
        spikeEffect.GetComponent<Boss_Attack_Effect>().Activate("Jumping", monsterSpec.MONSTER_DAMAGE);
    }
}
