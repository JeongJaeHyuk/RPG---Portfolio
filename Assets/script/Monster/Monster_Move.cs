using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Move : MonoBehaviour
{
    NavMeshAgent nav;
    Animator ani;
    [SerializeField] GameObject player = null;
    Vector3 oriPos;
    Monster_Stats monsterStats;
    Monster_Attack_Hit monsterAttack;
    

    bool IsIdle = false; // 맨 처음위치로 돌아갈지 정하는 불타입 true일 경우 처음위치로 가겠다
    bool IsMove = false; // 움직일지 정하는 false경우 움직일수있다
    bool IsDie = false; // 죽었는지 여부 체크
    [SerializeField] bool IsPlayer = false; // 플레이어 여부 체크하고 true경우 공격
    public bool ISIDLE { get => IsIdle; set => IsIdle = value; }
    public bool ISMOVE { get => IsMove; set => IsMove = value; }
    public bool ISDIE { get => IsDie; set => IsDie = value; }
    public bool ISPLAYER { get => IsPlayer; set => IsPlayer = value; }

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        monsterStats = GetComponent<Monster_Stats>();
        monsterAttack = GetComponent<Monster_Attack_Hit>();
    }
    private void Start()
    {
        oriPos = transform.position;
    }
    private void Update()
    {
        if (player != null)
        {
            MonsterRealMove();
        }
        else
        {
            ReturnPos(); //player가 null일 경우 맨처음 생성위치로 되돌아가는 함수
        }
        Die_Check();
    }
    // 몬스동 이동함수
    public void MonsterMove(GameObject other)
    {
        if (nav != null)
        {
            if (ISMOVE.Equals(false))
            {
                nav.isStopped = false;// 이동재개
                nav.SetDestination(other.transform.position); // 목표로 이동
                ani.SetInteger("Move", 1); // 걷는 애니메이션 실행
            }

        }
    }
    public void MonsterRotate(GameObject _target)
    {
        Vector3 direction = _target.transform.position - transform.position; // 플레이어를 바라보는 방향 계산
        direction.y = 0; // Y축 회전만 적용

        // Quaternion을 이용한 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
    }

    
    public void MonsterRealMove()
    {
        if (player != null && !ISDIE)
        {
            float dis = Vector3.Distance(gameObject.transform.position, player.gameObject.transform.position);
            if (dis <= monsterStats.MONSTER_RANGE && !monsterAttack.ISATTACK) // 플레이어와 몬스터 사이거리가 특정사거리 이하이고 공격중이 아닐떄
            {
                if (!ISPLAYER)// 플레이어가 앞에 있으면 실행
                {
                    StartCoroutine(monsterAttack.Attack());                  
                }
                else // 플레이어가 앞에 없으면 실행
                {
                    MonsterRotate(player); // 플레이어를 바라보는 함수
                }
            }
            else if (dis > monsterStats.MONSTER_RANGE && !monsterAttack.ISATTACK)
            {
                MonsterMove(player); // 플레이어한테 움직이는 함수
            }
        }
    }

    public void ReturnPos()
    {
        if (!IsDie)
        {
            if (nav != null) // 몬스터가 제자리로 돌아갔을떄 Idle상태로 되돌리는 함수
            {
                if (ISIDLE)
                {
                    nav.isStopped = false;// 이동재개
                    nav.SetDestination(oriPos); // 맨처음 위치로 이동
                    ani.SetInteger("Move", 1); // 걷는 애니메이션 실행
                    float dis = Vector3.Distance(oriPos, transform.position); // 처음위치와 현재 위치 거리계산
                    if (dis <= 2f)// 처음위치가 현재위치가 2f보다 작을경우 로직실행
                    {
                        nav.isStopped = true; // 이동멈춤
                        nav.velocity = Vector3.zero;// 속도 0으로 설정
                        ani.SetInteger("Move", 0); // Idle상태로 전환
                        ISIDLE = false; // 제자리로 도착했으니 false로 전환
                    }
                }
            }

        }
    }
    // Monster_Check 스크립트에서 player정보를 받아올 때 사용함
    public void SetPlayer(GameObject _obj)
    {
        player = _obj;
    }

    // 죽었을경우 Moster가 밑으로 내려가 사라지게 보이는 함수
    public void Die_Check()
    {
        if (IsDie)
        {
            // 몬스터의 현재 위치를 가져옴
            Vector3 currentPosition = transform.position;

            // y값을 감소시키고 새 위치를 설정
            currentPosition.y -= 0.3f * Time.deltaTime; // y값을 감소

            // 새 위치 설정
            transform.position = currentPosition;
        }
    }

    // 리스폰 함수
    public IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(20f);
        Debug.Log("몬스터 재생성");
        gameObject.SetActive(true);
    }
}
