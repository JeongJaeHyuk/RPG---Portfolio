using UnityEngine;
using UnityEngine.AI;

// Monster_Move의 보스 전용 버전 - 일반 몹(Monster_Move)에 영향 주지 않기 위해 별도 sibling 컴포넌트로 분리
// (CLAUDE.md 보스 추가 규칙: Monster_Move는 virtual이 없어 오버라이드 불가 -> 새 sibling으로 확장)
// 기본 추격/복귀 로직은 Monster_Move와 동일하고, 공격 진입 시 회전을 다 맞출 때까지 대기하는 게이트와
// 공격 애니메이션(특히 root motion으로 실제 전진하는 콤보/도약 패턴)이 NavMeshAgent한테 방해받지 않도록
// nav.updateRotation/updatePosition을 잠깐 꺼주는 처리가 추가되어 있다.
public class Boss_Move : MonoBehaviour
{
    public NavMeshAgent nav;
    public Monster_State monsterState;
    public Vector3 oriPos; // 몬스터 초기 생성 위치

    [SerializeField] float rotateSpeed = 720f; // LookAtPlayer 순간 스냅 대신 부드럽게 회전(텔포 느낌 방지)용 각속도(도/초)

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
        nav.updateRotation = true; // 공격 중이 아닐 때만 이 함수가 도니, 여기서 항상 NavMeshAgent한테 회전 제어권을 되돌려줌
        if (!nav.updatePosition) // 공격 중 root motion 이동 위해 꺼뒀던 걸 다시 켬
        {
            nav.updatePosition = true;
            nav.Warp(transform.position); // root motion으로 실제 옮겨진 위치로 에이전트 내부 위치도 동기화 - 안 하면 다음 이동 때 순간이동처럼 튐
        }

        float dis = Vector3.Distance(transform.position, monsterState.GetPlayer().transform.position);

        if (dis <= monsterState.monsterSpec.MONSTER_RANGE) // 공격 사거리 이내
        {
            nav.isStopped = true;
            if (!monsterState.GetIsAttacking()) // 공격 중이 아닐 때만 회전 및 공격 시작
            {
                monsterState.StopMoveAnim(); // 걷기 애니메이션 끄고 Idle로 전환 - 걷다가 바로 공격 자세로 튀는 것보다 자연스러움
                LookAtPlayer(); // 매 프레임 호출 - 부드럽게 다 돌기 전엔 아래 IsFacingPlayer가 false라 공격 진입 안 함
                if (IsFacingPlayer())
                {
                    // 공격 중엔 NavMeshAgent가 회전/위치를 되돌리지 못하게 차단 - 애니메이션 root motion이 정한 대로 유지
                    nav.updateRotation = false;
                    nav.updatePosition = false;
                    monsterState.EnterAttack();
                }
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

    public void LookAtPlayer()
    {
        Vector3 dir = monsterState.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
    }

    // 애니메이션 이벤트(콤보 중간 재조준 등)처럼 한 프레임만 호출되는 자리 전용 - LookAtPlayer는 매 프레임 호출을 전제로 한 보간이라 한 번만 부르면 반쯤 돌다 만 것처럼 튀어 보임
    public void SnapToPlayer()
    {
        Vector3 dir = monsterState.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    // LookAtPlayer가 목표 방향까지 다 돌았는지 체크 - 공격 진입 타이밍 게이트용(다 돌기 전에 공격 시작하면 텔포처럼 보임)
    bool IsFacingPlayer()
    {
        Vector3 dir = monsterState.GetPlayer().transform.position - transform.position;
        dir.y = 0;
        return Vector3.Angle(transform.forward, dir) < 5f;
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
