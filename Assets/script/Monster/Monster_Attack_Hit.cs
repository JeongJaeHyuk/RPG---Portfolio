using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터 전투 시스템
/// - 플레이어 감지, 공격 실행, 피격 처리, 사망 처리
/// </summary>
public class Monster_Attack_Hit : MonoBehaviour
{
    #region ========== 같은 GameObject 컴포넌트 (GetComponent로 자동 찾기) ==========
    Monster_Stats monsterStats;
    Monster_Move monsterMove;
    NavMeshAgent nav;
    Animator ani;
    CapsuleCollider damageSensor;  // 몸통 콜라이더 (피격 감지용)
    #endregion

    #region ========== 자식 오브젝트 센서 (Inspector에서 드래그 필요) ==========
    [Tooltip("공격 범위 센서 (BoxCollider) - Monster_Attack_Check.cs 참고")]
    [SerializeField] BoxCollider attackSensor;

    [Tooltip("플레이어 감지 센서 (SphereCollider) - Monster_Check.cs 참고")]
    [SerializeField] SphereCollider monsterSensor;
    #endregion

    #region ========== 외부 참조 ==========
    GameObject player;  // 현재 감지된 플레이어
    #endregion

    #region ========== 공격 상태 변수 ==========
    [SerializeField] bool IsAttack = false;     // 공격 범위에 플레이어가 있는지 여부
    [SerializeField] bool IsAttacking = false;  // 공격 중인지 여부
    public bool ISATTACK { get => IsAttack; set => IsAttack = value; }
    #endregion

    #region ========== 스킬 관련 변수 ==========
    bool IsSkill_1 = false; // 스킬1 사용 여부 (Monster_2 전용)
    bool IsSkill_2 = false; // 스킬2 사용 여부
    #endregion

    #region ========== 코인 드롭 설정 ==========
    [Header("==== 코인 드롭 ====")]
    [SerializeField] int coin = 0;
    [SerializeField] int minCoin = 10;
    [SerializeField] int maxCoin = 50;
    #endregion

    #region ========== 플레이어 설정 ==========
    /// <summary>
    /// 플레이어 정보 설정 (Monster_Check.cs에서 호출)
    /// </summary>
    public void SetPlayer(GameObject _obj)
    {
        player = _obj;
    }
    #endregion
    #region ========== Unity 생명주기 ==========
    void Awake()
    {
        // 같은 GameObject의 컴포넌트 자동 찾기
        monsterStats = GetComponent<Monster_Stats>();
        monsterMove = GetComponent<Monster_Move>();
        damageSensor = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();

        // 에러 체크
        if (monsterStats == null) Debug.LogError($"{gameObject.name}: Monster_Stats를 찾을 수 없습니다!");
        if (monsterMove == null) Debug.LogError($"{gameObject.name}: Monster_Move를 찾을 수 없습니다!");
    }

    void Start()
    {
        coin = Random.Range(minCoin, maxCoin);
    }

    void OnEnable()
    {
        SensorOn();
    }
    #endregion
    #region ========== 피격 처리 (OnTriggerEnter) ==========
    /// <summary>
    /// 플레이어 공격에 맞았을 때 데미지 처리
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        string name = LayerMask.LayerToName(other.gameObject.layer);

        // 데미지 연산: 기본 공격, 스킬 공격
        switch (name)
        {
            case "BasicAttack":  // 기본 공격
                {
                    if (!monsterMove.ISDIE)
                    {
                        PlayerSpecs plSp = other.transform.root.gameObject.GetComponent<PlayerSpecs>();
                        PlayerProgression pps = other.transform.root.gameObject.GetComponent<PlayerProgression>();
                        if (plSp != null)
                        {
                            float damage = plSp.BasicTotalDamage();
                            TakeDamage(damage, plSp, pps);
                        }
                    }
                    break;
                }
            case "Spear":  // 스킬 공격 (창 공격)
                {
                    if (!monsterMove.ISDIE)
                    {
                        // 감지한 오브젝트에서 최상위 오브젝트로 접근
                        PlayerSpecs plSp = other.transform.root.gameObject.GetComponent<PlayerSpecs>();
                        PlayerProgression pps = other.transform.root.gameObject.GetComponent<PlayerProgression>();
                        if (plSp != null)
                        {
                            float damage = plSp.SpearTotalDamage();
                            TakeDamage(damage, plSp, pps);
                        }
                    }
                    break;
                }
        }
    }
    #endregion
    #region ========== 공격 시스템 ==========
    /// <summary>
    /// 공격 코루틴
    /// </summary>
    public IEnumerator Attack()
    {
        while (player != null)  // 플레이어가 있을 경우 계속 반복
        {
            RandomAttack();  // 공격 함수
            yield return new WaitForSeconds(monsterStats.MONSTER_ATTACKTIME);  // n초 대기 후 다시 검사
            break;  // TODO: break로 인해 한 번만 공격함 (버그 가능성)
        }
        monsterMove.ISMOVE = false;    // 공격 끝나면 움직일 수 있도록 설정
        monsterMove.ISPLAYER = false;  // 공격 종료
    }

    /// <summary>
    /// 랜덤 공격 패턴 선택
    /// </summary>
    public void RandomAttack()
    {
        if (monsterMove.ISDIE)  // 죽은 경우 움직임을 막기 위해
            return;

        // 몬스터 이름에서 타입 추출 (예: "Monster_1#1" → "Monster_1")
        string name = gameObject.name;
        name = name.Substring(0, name.IndexOf("_"));

        int attackType = 0;

        switch (name)
        {
            case "RockGolem":
            {
                attackType = Random.Range(1, 3);  // 공격 타입 1~2 랜덤
                break;
            }
            case "GreenOrc":
            case "BlueOrc":
            case "RedOrc":
            {
                // 체력 1/3 이하일 때 스킬 사용
                if (monsterStats.MAX_HP / 3 > monsterStats.CURRENT_HP && !IsSkill_1)
                {
                    IsSkill_1 = true;
                    attackType = 2;
                    break;
                }
                else
                {
                    attackType = 1;
                    break;
                }
            }
        }

        // 공격 실행
        nav.isStopped = true;        // 이동 멈춤
        nav.velocity = Vector3.zero; // 속도 0으로 설정
        ani.SetInteger("Move", 0);   // 애니메이션 Idle로 전환
        ani.SetInteger("Attack", attackType);  // 공격 애니메이션 실행
        monsterMove.ISMOVE = true;   // 캐릭터 움직임 막기
        monsterMove.ISPLAYER = true;
    }

    /// <summary>
    /// 애니메이션 이벤트: 공격 애니메이션 끝날 때 호출
    /// Attack 파라미터 값을 0으로 리셋 (안 하면 계속 공격 모션 반복됨)
    /// </summary>
    public void AniAttackReset()
    {
        ani.SetInteger("Attack", 0);
    }
    #endregion

    #region ========== 데미지 및 사망 처리 ==========
    /// <summary>
    /// 데미지를 받았을 때 처리
    /// </summary>
    public void TakeDamage(float _damage, PlayerSpecs _plsp, PlayerProgression _pps)
    {
        monsterStats.CURRENT_HP -= _damage;  // 플레이어 공격력을 받아 몬스터 체력 감소

        if (monsterStats.CURRENT_HP <= 0f)  // 몬스터 체력이 0 이하일 경우
        {
            monsterStats.CURRENT_HP = 0f;
            ani.SetTrigger("Die");
            monsterMove.ISDIE = true;
            StartCoroutine("Die");
            SensorOff();
            _pps.CURRENT_EXP += monsterStats.MONSTER_EXP;  // 경험치 추가
            CoinManager.Instance.DropCoin(gameObject, coin);  // 코인 드롭
            string monsName = gameObject.name.Split('_')[0]; // _ 뒤에 다 잘라버리기
            DropData drop = DropTable_Manager.instance.GetDropItem(monsName);   // 이름에 맞는 DropData가 존재하는지 확인후 리턴
            // 드랍이 확률로 드랍하는거라 없을경우도 있기떄문에 drop이 null아닐떄 아이템전리품 사용
            if(drop != null)
            {

                LootBag loot = LootBagPool.instance.GetLootBag();   // lootbag 오브젝트 활성화 및 활성화된 데이터 받아오기
                loot.SetDropData(drop); // 받아온 데이터로 아이템 정보 설정
                loot.gameObject.transform.position = gameObject.transform.position; // 설정이 된 오브젝트 위치를 몬스터 위치로 이동
            } 

            // 수락 퀘스트중에 맞는 이름의 몬스터가 있나 확인을 위한 함수
            UI_Quest.instance.KillMonsterQuest(gameObject.name);
        }
    }

    /// <summary>
    /// 사망 처리 코루틴
    /// </summary>
    public IEnumerator Die()
    {
        yield return new WaitForSeconds(5f);
        // 5초 후 오브젝트 비활성화
        gameObject.SetActive(false);
    }
    #endregion
    #region ========== 센서 관리 ==========
    /// <summary>
    /// 센서 비활성화 (사망 시)
    /// </summary>
    public void SensorOff()
    {
        nav.enabled = false;           // 움직임 담당하는 컴포넌트 Off
        monsterSensor.enabled = false; // 플레이어 감지 센서 Off
        damageSensor.enabled = false;  // 데미지 센서 Off
        attackSensor.enabled = false;  // 공격 범위 센서 Off
    }

    /// <summary>
    /// 센서 활성화 (재생성 시)
    /// </summary>
    public void SensorOn()
    {
        // 애니메이션 초기화 (꼬이는 것 방지)
        ani.SetInteger("Move", 0);     // Idle 상태로 전환
        ani.SetInteger("Attack", 0);   // 공격 애니메이션 리셋
        ani.ResetTrigger("Die");       // Die 트리거 리셋
        ani.ResetTrigger("Hit");       // Hit 트리거 리셋

        // 상태 초기화
        monsterMove.ISDIE = false;     // 죽음 상태 해제
        monsterMove.ISIDLE = false;    // Idle 상태 해제
        monsterMove.ISMOVE = false;    // 이동 상태 해제
        monsterMove.ISPLAYER = false;  // 플레이어 추격 상태 해제

        // 센서 활성화
        nav.enabled = true;            // NavMeshAgent On
        monsterSensor.enabled = true;  // 플레이어 감지 센서 On
        damageSensor.enabled = true;   // 데미지 센서 On
        attackSensor.enabled = true;   // 공격 범위 센서 On

        // 스탯 초기화
        monsterStats.CURRENT_HP = monsterStats.MAX_HP;  // 체력 최대치로 복구
        IsSkill_1 = false;  // 스킬 사용 여부 리셋
        IsSkill_2 = false;  // 스킬 사용 여부 리셋

        // 플레이어 정보 초기화
        monsterMove.SetPlayer(null);
        SetPlayer(null);

        // 코인 재설정
        coin = Random.Range(minCoin, maxCoin);
    }

    /// <summary>
    /// 재생성 코루틴
    /// </summary>
    public IEnumerator ReSpawn()
    {
        yield return new WaitForSeconds(20f);
        Debug.Log("몬스터 재생성");
        gameObject.SetActive(true);
    }
    #endregion
}
