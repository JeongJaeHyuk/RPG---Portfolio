using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_State_Boss : Monster_State
{
    // 보스 몬스터 - 일반공격 없이 전부 패턴(스킬)으로 처리
    // OrcKing_Animation_Controller 파라미터 기준:
    //  Move(int 0/1) - 그대로 상속 사용
    //  Skill_Attack(int) : Idle/Walk에서 1=Sword And Shield Slash, 2=TurnSlash 바로 발동(무쿨타임) - meleeSkillIds
    //  Attack_Ready(trigger) : Shout 0(예비동작)으로 진입 후, 그 안에서 Skill_Attack 3=Slash Attack(원거리), 4=Breath(근거리)로 분기
    //  (샤우트 쿨타임 게이트) - closeSkillIds/farSkillIds
    //
    // MONSTER_RANGE(CSV)는 근접 사거리 전용으로 원본 그대로 사용 - 근접 패턴은 지금까지처럼
    // "다가가서 멈추면 EnterAttack()" 흐름을 그대로 탄다. 이동/회전 로직은 Monster_Move가 아니라 보스 전용
    // Boss_Move(sibling)를 따로 써서 일반 몹에 영향 안 가게 분리했다. 샤우트(원거리 포함) 패턴은 Boss_Move를
    // 건드리지 않기 위해 이 클래스 자체 Update()에서 별도 사거리(shoutRange)로 독립적으로 체크한다.

    [Header("패턴 발동 위치")]
    [SerializeField] Transform meleePoint; // Sword And Shield Slash / TurnSlash
    [SerializeField] Transform breathPoint;
    [SerializeField] Transform farPoint;   // Slash Attack

    [Header("근접 패턴 목록")]
    [SerializeField] int[] meleeSkillIds = { 1, 2 }; // 1: Sword And Shield Slash, 2: TurnSlash - 나중에 늘어나면 배열에 추가만 하면 됨(개수 제한 없음)

    [Header("샤우트 패턴")]
    [SerializeField] float shoutCooldown = 10f;
    [SerializeField] float shoutRange = 8f;         // 이 거리 안이면 쿨타임 됐을 때 샤우트 발동 후보
    [SerializeField] float closeRangeThreshold = 6f; // 샤우트 발동 시 이 거리 이하면 closeSkillIds, 넘으면 farSkillIds 중 랜덤
    [SerializeField] int[] closeSkillIds = { 4 };    // 4: Breath - 나중에 늘어나면 배열에 추가만 하면 됨
    [SerializeField] int[] farSkillIds = { 3 };      // 3: Slash Attack - 나중에 늘어나면 배열에 추가만 하면 됨

    float lastShoutTime = -999f;
    int selectedFarSkill; // farSkillIds 중 이번에 고른 id - FarSkillOn()에서 이펙트 이름 매핑용
    Boss_Attack_Effect slashContainer; // FarSkillOn()이 위치 잡아둔 검풍 컨테이너 - 블레이드 개별 발사(SlashBladeXOn)에서 사용

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

        lastShoutTime = Time.time;
        isAttacking = true;
        monsterMove.nav.updateRotation = false; // 공격 중 NavMeshAgent가 회전을 되돌리지 못하게 차단
        monsterMove.nav.updatePosition = false; // 원거리 패턴의 root motion 이동도 NavMeshAgent가 취소하지 못하게 차단
        ani.SetInteger("Move", 0);
        monsterMove.SnapToPlayer(); // Boss_Move의 ChaseMove를 거치지 않는 경로라 회전을 직접 호출 - 한 번만 불리므로 순간 스냅 버전 사용
        ani.SetTrigger("Attack_Ready");

        // TEMP: Slash Attack만 먼저 검증 - 확인되면 아래 거리 분기 원복
        selectedFarSkill = farSkillIds[Random.Range(0, farSkillIds.Length)];
        ani.SetInteger("Skill_Attack", selectedFarSkill); // Slash Attack
        /*
        if (dist <= closeRangeThreshold)
        {
            ani.SetInteger("Skill_Attack", closeSkillIds[Random.Range(0, closeSkillIds.Length)]); // Breath
        }
        else
        {
            selectedFarSkill = farSkillIds[Random.Range(0, farSkillIds.Length)]; // Slash Attack
            ani.SetInteger("Skill_Attack", selectedFarSkill);
        }
        */
    }

    // 공격 애니메이션이 끝날 때 애니메이션 이벤트로 호출 - Skill_Attack을 0으로 되돌려야 Idle에서 즉시 재발동하지 않는다
    public override void ExitAttack()
    {
        isAttacking = false;
        ani.SetInteger("Skill_Attack", 0);
    }

    // 아래 함수들은 각 패턴 애니메이션의 타격 프레임에 애니메이션 이벤트로 연결해서 사용
    public void BreathOn()
    {
        BossEffectManager.instance.PlayEffect("Breath", breathPoint, monsterSpec.MONSTER_DAMAGE);
    }

    public void FarSkillOn()
    {
        // 지금은 원거리(far) 패턴이 Slash Attack 하나뿐이라 이름 고정 - farSkillIds에 새 패턴 추가하면 여기도 id -> 이름 매핑 case 추가할 것
        // PlayEffect()는 검풍 컨테이너(자식으로 블레이드 3개를 가진 오브젝트)만 farPoint에 위치시킨다 -
        // 블레이드 개별 발사는 아래 SlashBladeXOn()이 애니메이션 이벤트 타이밍에 맞춰 따로 호출한다.
        string patternName = "SlashAttack";
        slashContainer = BossEffectManager.instance.PlayEffect(patternName, farPoint, monsterSpec.MONSTER_DAMAGE);
    }

    // 검풍 블레이드 1~3 - 컨테이너 배치 후 애니메이션 이벤트 타이밍에 맞춰 각각 호출해서 순서대로 발사
    public void SlashBlade1On() { FireSlashBlade(0); }
    public void SlashBlade2On() { FireSlashBlade(1); }
    public void SlashBlade3On() { FireSlashBlade(2); }

    void FireSlashBlade(int _index)
    {
        if (slashContainer == null || _index >= slashContainer.transform.childCount)
        {
            return;
        }

        Transform blade = slashContainer.transform.GetChild(_index);
        Boss_Attack_Effect_Projectile projectile = blade.GetComponent<Boss_Attack_Effect_Projectile>();
        blade.gameObject.SetActive(true);
        projectile.Activate("SlashAttack", monsterSpec.MONSTER_DAMAGE);
    }
}
