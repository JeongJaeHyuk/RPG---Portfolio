using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_State_Boss : Monster_State
{
    // 보스 몬스터 - 일반공격 없이 전부 패턴(스킬)으로 처리
    // OrcKing_Animation_Controller 파라미터 기준:
    //  Move(int 0/1) - 그대로 상속 사용
    //  Skill_Attack(int) : Idle/Walk에서 1=Overhead Slash, 2=ComboAttack 바로 발동(무쿨타임)
    //  Attack_Ready(trigger) : Shout 0(예비동작)으로 진입 후, 그 안에서 Skill_Attack 3=Slash Attack, 4=Leaping Strike, 5=Breath로 분기(샤우트 쿨타임 게이트)
    //
    // Monster_Move/MONSTER_RANGE(CSV)는 근접 사거리 전용으로 원본 그대로 사용 - 근접 패턴은 지금까지처럼
    // "다가가서 멈추면 EnterAttack()" 흐름을 그대로 탄다. 샤우트(원거리 포함) 패턴은 Monster_Move를 건드리지 않기 위해
    // 이 클래스 자체 Update()에서 별도 사거리(shoutRange)로 독립적으로 체크한다.

    [Header("패턴 발동 위치")]
    [SerializeField] Transform meleePoint; // ComboAttack / OverheadSlash
    [SerializeField] Transform breathPoint;
    [SerializeField] Transform farPoint;   // Slash Attack / Leaping Strike

    [Header("샤우트 패턴")]
    [SerializeField] float shoutCooldown = 10f;
    [SerializeField] float shoutRange = 8f;         // 이 거리 안이면 쿨타임 됐을 때 샤우트 발동 후보
    [SerializeField] float closeRangeThreshold = 3f; // 샤우트 발동 시 이 거리 이하면 Breath, 넘으면 Slash Attack/Leaping Strike 중 랜덤

    float lastShoutTime = -999f;
    int selectedFarSkill; // 3: Slash Attack, 4: Leaping Strike - 이번에 고른 것
    bool combatStarted;   // 등장 연출(Jumping Down -> Shout -> Idle)이 끝나기 전엔 false

    // 근접 사거리(Monster_Move가 감지) 안까지 다가왔을 때 호출 - 근접 패턴만 담당
    public override void EnterAttack()
    {
        isAttacking = true;
        ani.SetInteger("Move", 0);
        ani.SetInteger("Skill_Attack", Random.Range(1, 3)); // 1: Overhead Slash, 2: ComboAttack
    }

    // 등장 연출이 끝나는 지점(Shout 인트로 클립 끝)에 애니메이션 이벤트로 호출
    // 그 순간부터 쿨타임이 돌기 시작하고, 그때까지 꺼둔 PlayerSensor 콜라이더도 켜서 감지를 시작한다
    public void StartCombat()
    {
        combatStarted = true;
        lastShoutTime = Time.time;

        Collider sensorCollider = monsterRange.GetComponent<Collider>();
        if (sensorCollider != null)
        {
            sensorCollider.enabled = true;
        }
    }

    // Monster_Move와 별개로 샤우트(원거리 포함) 패턴만 자체적으로 감시 - 추격 중이든 멈춰있든 조건 맞으면 바로 발동
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
        ani.SetInteger("Move", 0);
        ani.SetTrigger("Attack_Ready");

        if (dist <= closeRangeThreshold)
        {
            ani.SetInteger("Skill_Attack", 5); // Breath
        }
        else
        {
            selectedFarSkill = Random.Range(0, 2) == 0 ? 3 : 4; // Slash Attack : Leaping Strike
            ani.SetInteger("Skill_Attack", selectedFarSkill);
        }
    }

    // 공격 애니메이션이 끝날 때 애니메이션 이벤트로 호출 - Skill_Attack을 0으로 되돌려야 Idle에서 즉시 재발동하지 않는다
    public override void ExitAttack()
    {
        isAttacking = false;
        ani.SetInteger("Skill_Attack", 0);
    }

    // 아래 함수들은 각 패턴 애니메이션의 타격 프레임에 애니메이션 이벤트로 연결해서 사용
    public void OverheadSlashOn()
    {
        BossEffectManager.instance.PlayEffect("OverheadSlash", meleePoint, monsterSpec.MONSTER_DAMAGE);
    }

    public void ComboAttackOn()
    {
        BossEffectManager.instance.PlayEffect("ComboAttack", meleePoint, monsterSpec.MONSTER_DAMAGE);
    }

    public void BreathOn()
    {
        BossEffectManager.instance.PlayEffect("Breath", breathPoint, monsterSpec.MONSTER_DAMAGE);
    }

    public void FarSkillOn()
    {
        string patternName = selectedFarSkill == 3 ? "SlashAttack" : "LeapingStrike";
        BossEffectManager.instance.PlayEffect(patternName, farPoint, monsterSpec.MONSTER_DAMAGE);
    }
}
