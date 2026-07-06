using UnityEngine;
using UnityEngine.AI;

public class Monster_Move : MonoBehaviour
{
    public NavMeshAgent nav;
    public Monster_State monsterState;
    public Vector3 oriPos; // 몬스터 초기 생성 위치

    void Awake()
    {
        nav          = GetComponent<NavMeshAgent>();
        monsterState = GetComponent<Monster_State>();
    }

    void Start()
    {
        oriPos = transform.position; // 생성 시점 위치 저장
    }

    void Update()
    {
        if (monsterState.GetIsDead()) return;      // 사망 시 이동 차단
        if (monsterState.GetIsIdle()) return;      // 대기 상태면 이동 로직 전체 스킵
        if (monsterState.GetIsAttacking()) return; // 공격 중 이동 차단

        if (monsterState.GetPlayer() != null)
        {
            ChaseMove(); // 플레이어 감지 중 - 추격
        }
        else
        {
            ReturnToOrigin(); // 플레이어 없음 - 원래 위치로 복귀
        }
    }

    void ChaseMove()
    {
        float dis = Vector3.Distance(transform.position, monsterState.GetPlayer().transform.position);

        if (dis <= monsterState.monsterSpec.MONSTER_RANGE) // 공격 사거리 이내
        {
            nav.isStopped = true;
            if (!monsterState.GetIsAttacking()) // 공격 중이 아닐 때만 회전 및 공격 시작
            {
                LookAtPlayer();
                monsterState.EnterAttack();
            }
        }
        else // 공격 사거리 밖 - 추격 이동
        {
            if (!monsterState.GetIsAttacking()) // 공격 중엔 이동 차단
            {
                nav.isStopped = false;
                nav.SetDestination(monsterState.GetPlayer().transform.position);

                if (nav.velocity.sqrMagnitude > 0.01f) // 실제 이동 중일 때만 걷기 애니메이션
                {
                    monsterState.EnterMove();
                }
                else // 충돌 등으로 막혀있을 때 - 이동 애니메이션 중단
                {
                    monsterState.StopMoveAnim();
                }
            }
        }
    }

    void LookAtPlayer()
    {
        Vector3 dir = monsterState.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void ReturnToOrigin()
    {
        nav.isStopped = false;
        nav.SetDestination(oriPos); // 초기 위치로 이동

        Vector2 cur = new(transform.position.x, transform.position.z);
        Vector2 ori = new(oriPos.x, oriPos.z);
        if (Vector2.Distance(cur, ori) <= 0.5f) // XZ 기준으로 도착 판정
        {
            nav.isStopped = true;
            nav.velocity  = Vector3.zero;
            monsterState.EnterIdle(); // 제자리 복귀 완료
        }
    }
}
