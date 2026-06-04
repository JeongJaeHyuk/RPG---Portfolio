                           using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    PlayerSpecs plSp;

    Animator ani;
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Vector3 end;
    [SerializeField] bool isRoll = false;// 스킬 사용시 움직이는 키 막기

    //이펙트 관련 부모 변수
    [SerializeField] GameObject effect;
    //기본공격 관련 변수들
    bool isAttack = false; // 공격도중에 다른 행동을 못하게 방지하는 데 사용
    [SerializeField] GameObject[] basicAttackEf;
    [SerializeField] GameObject[] finalEf;
    [SerializeField] GameObject spearSensor;
    // 캐릭터 스펙 변수
    private float moveSpeed = 5f; //캐릭터 이동속도
    private float turnSpeed = 10f; // 캐릭터의 회전 속도

    // 캐릭터 스킬 오브젝트 정보들  
    // 파티클 시스템 찾아서 임펙트 나오게 할것
    [SerializeField] GameObject skillPos; // Q 와 E 위치 찍는 오브젝트
    [SerializeField] GameObject skillWPos; // W 위치 찍는 오브젝트
    // 마지막 R은 버프스킬로 광폭화를 넣을까 고민중 그러면 파티클 버프처럼 생긴거 하나랑 R눌렀을때 폭발하는 이펙트도 찾아볼것

    //데미지 받는 변수
    [SerializeField] CapsuleCollider damageSensor;

    // 마우스포인트로 움직여서 특정 레이어만 받을수 있도록 제한걸어야 움직이는 코드에서 몬스터센서등등이런거에 안걸려 버그가 안생김
    // 즉 몬스터가 캐릭터를 감지하는 범위 등 이런 콜라이더에 레이어를 받아서 움직임이 제한되는 경우가 생겨서 밑에 변수들둬서 움직임은 Terrain만 감지할수 있도록 한 것
    public LayerMask layerlayer; // Terrain의 레이어 정보

    // 스킬 쿨타임 정보
    [SerializeField]Skill_CoolTime[] skillCool;
    private void Awake()
    {
        // 여기는 컴포넌트할당
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        damageSensor = GetComponent<CapsuleCollider>();
        plSp = GetComponent<PlayerSpecs>();
        // 여기는 배열 할당
        basicArray();
        spearSensor = GameObject.FindGameObjectWithTag("Spear");
        spearSensor.SetActive(false);
    }
    void Start()
    {
        nav.speed = moveSpeed;
        nav.acceleration = 100f; // 캐릭터 관성 최소화 하기
        end = Vector3.zero;
    }

    void Update()
    {
        if (UI.instance.IsAnyUIOpen)   // UI 가 켜질 경우 
            return;
        if (isRoll) // 캐릭터가 구르기를 시전할 경우
            return;
        MovePlayer();
        IsRoll();
        Skill();
        BasicAttack();
    }
    #region 이동관련
    //마우스 우측키로 이동하는 함수(애니메이션은 각각 애니메이션 스크립트에서 설정하고 여기서는 타켓과 위치 포인트 잡는 함수
    void MovePlayer()
    {
        if (isAttack)
            return;
        if (Shop_Manager.shopManager.isShopOpen)
            return;
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerlayer))
            {
                nav.SetDestination(hit.point);
                ani.SetInteger("AniIndex", 1);
                end = hit.point;
            }
        }
        RotateCharacter();
        // 거리 계산해서 타켓과 거리가 1f이하일때 멈춘다 NPC 기능에 추가할려고하는데 이거 말고 콜라이더 충돌 처리로 해도 될듯
        float dis = Vector3.Distance(gameObject.transform.localPosition, end);
        if (dis <= 1f)
        {
            ani.SetInteger("AniIndex", 0);
            Vector3 stop = transform.position;
            nav.SetDestination(stop);
        }
    }
    void RotateCharacter()
    {
        // 이동 방향으로 캐릭터를 회전
        if (nav.velocity.magnitude > 0.1f)
        {
            Vector3 direction = (nav.steeringTarget - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }
    }
    #endregion
    #region 기본공격
    public void BasicAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ani.SetTrigger("BasicAttack");
            nav.ResetPath(); // 움직이는 도중 공격할경우 멈추는 함수
            isAttack = true; // 공격애니메이션 작동할떄 움직임 막는 함수
        }
    }
    //기본공격 파티클 배열에 할당받는 함수
    void basicArray()
    {
        Transform trans; // BasicEffect 위치정보를 알기위한 변수
        for (int i = 0; i < effect.transform.childCount; i++)
        {
            if (effect.transform.GetChild(i).name.Equals("BasicEffect")) // effect오브젝트에서 이름이 BasicEffect인 오브젝트가 있을경우 실행
            {
                trans = effect.transform.GetChild(i);// 위치정보 대입
                basicAttackEf = new GameObject[trans.childCount]; // 배열할당
                for (int k = 0; k < trans.childCount; k++)
                {
                    basicAttackEf[k] = trans.GetChild(k).gameObject;// 배열 대입
                }
            }
        }

    }

    // 기본공격 이벤트키
    void Slash1()
    {
        basicAttackEf[0].SetActive(true);
    }
    void Slash2()
    {
        basicAttackEf[1].SetActive(true);
    }
    void Slash3()
    {
        basicAttackEf[2].SetActive(true);
    }
    void Slash4()
    {
        basicAttackEf[3].SetActive(true);
    }
    void Slash5()
    {
        for (int i = 0; i < finalEf.Length; i++)
        {
            finalEf[i].SetActive(true);
        }
    }

    //데미지 센서는 파티클과 다르게 실행후 안꺼져서 이벤키로 조절
    void SpearSensorOff()
    {
        spearSensor.SetActive(false);
    }
    void isAttackFalse() // 공격도중 움직이는 막는 함수
    {
        isAttack = false;
        ani.SetInteger("AniIndex", 0);
    }
    #endregion

    #region 스킬관련
    public bool isSkill_LShift = true;
    void IsRoll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)&& isSkill_LShift == false&& skillCool[0].isCoolDown == false)
        {
            skillCool[0].StartCoroutine(skillCool[0].SkillCoolStart());
            isRoll = true;
            ani.SetTrigger("Rolling");
            damageSensor.isTrigger = false;
        }
    }
    // 스킬 창 BottomUI에서 스킬 잠김 표시 여부체크용
    public bool isSkill_Q = true;
    public bool isSkill_W = true;
    public bool isSkill_E = true;
    public bool isSkill_R = true;
    // 여기서 SKill_CoolTime 스크립트 정보를 가진 오브젝트를 리스트로 보관하고 각각 Input 키코드에 맞게 코루틴함수 불러보기 
    void Skill()
    {
        //오브젝트 활성화 시키고
        // && 연산자 추가해서 Skill_CoolTime스크립트 정보를 가진 리스트를 불러서 isCool 불타입이 false경우도 추가
        if (Input.GetKeyDown(KeyCode.Q) && isSkill_Q == false&& skillCool[1].isCoolDown == false)
        {
            // 여기다가 쿨타임 코루틴함수 실행처리
            skillCool[1].StartCoroutine(skillCool[1].SkillCoolStart());
            Vector3 stop = transform.position;
            nav.SetDestination(stop);
            isRoll = true;
            ani.SetTrigger("Skill_Q");
            StartCoroutine(QSkillMove());

            // 스킬 데이터 전달 (skillCool[1]에 Q스킬 데이터가 있음)
            SkillOn.skOn.SkillQ_ON(skillPos, skillCool[1].skill);
            SkillOn.skOn.SkillQ_Pos(skillPos);

        }
        else if (Input.GetKeyDown(KeyCode.W) && isSkill_W == false&& skillCool[2].isCoolDown == false)
        {
            skillCool[2].StartCoroutine(skillCool[2].SkillCoolStart());
            Vector3 stop = transform.position;
            nav.SetDestination(stop);
            isRoll = true;
            ani.SetTrigger("Skill_W");
            StartCoroutine(WSkillMove());
            // W스킬은 애니메이션 이벤트키로 사용함
            //SkillOn.skOn.SkillW_ON(skillPos);
        }
        else if (Input.GetKeyDown(KeyCode.E) && isSkill_E == false&& skillCool[3].isCoolDown == false)
        {
            skillCool[3].StartCoroutine(skillCool[3].SkillCoolStart());
            Vector3 stop = transform.position;
            nav.SetDestination(stop);
            isRoll = true;
            ani.SetTrigger("Skill_E");
            // E스킬은 애니메이션 이벤트키로 사용함
        }
        else if (Input.GetKeyDown(KeyCode.R) && isSkill_R == false && skillCool[4].isCoolDown == false)
        {
            skillCool[4].StartCoroutine(skillCool[4].SkillCoolStart());
            Vector3 stop = transform.position;
            nav.SetDestination(stop);
            isRoll = true;
            ani.SetTrigger("Skill_R");
        }

    }
    // 스킬사용할떄 이동하는 코루틴함수
    IEnumerator QSkillMove()
    {
        float dis = 5f; // 이동할 거리

        // 이동할 위치
        Vector3 targetPos = transform.position + transform.forward * dis;
        // 시작할 위치
        Vector3 startPos = transform.position;
        float moveTime = 0.2f; // 애니메이션 시간
        float elapsedTime = 0f; // 현재 시간

        while (elapsedTime < moveTime)
        {
            float t = elapsedTime / moveTime;
            nav.SetDestination(Vector3.Lerp(startPos, targetPos, t));// 에이전트를 사용하니까 에이전트전용함수사용하고 보간함수를사용하여 시작점과 도착점을 서서히 이동할수 있도록 한다
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // 스킬이펙트의 부모를 다시 원래 이 부모로 다시 설정하는 함수 이건 이벤트 키로 들어감
    public void QSkillPosRollBack()
    {
        SkillOn.skOn.skillQ_PosRollBack();
    }
    // 스킬사용할떄 이동하는 코루틴함수
    IEnumerator WSkillMove()
    {
        float dis = 2f; // 이동할 거리

        // 이동할 위치
        Vector3 targetPos = transform.position + transform.forward * dis;
        // 시작할 위치
        Vector3 startPos = transform.position;
        float moveTime = 0.25f; // 애니메이션 시간
        float elapsedTime = 0f; // 현재 시간

        while (elapsedTime < moveTime)
        {
            float t = elapsedTime / moveTime;
            nav.SetDestination(Vector3.Lerp(startPos, targetPos, t));// 에이전트를 사용하니까 에이전트전용함수사용하고 보간함수를사용하여 시작점과 도착점을 서서히 이동할수 있도록 한다
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    // 스킬이펙트 사용 이것은 타이밍맞추기위해 이벤트키로 사용함
    public void SkillW()
    {
        SkillOn.skOn.SkillW_ON(skillWPos, skillCool[2].skill);
    }
    public void SkillE_1()
    {
        SkillOn.skOn.SkillE1_ON(skillPos, skillCool[3].skill);
    }
    public void SkillE_2()
    {
        SkillOn.skOn.SkillE2_ON(skillPos, skillCool[3].skill);
    }
    public void SkillR_1()
    {
        SkillOn.skOn.SkillR1_ON(skillPos, skillCool[4].skill);
    }
    public void SkillR_2()
    {
        SkillOn.skOn.SkillR2_ON();
    }
    public void SkillR_3()
    {
        SkillOn.skOn.SkillR3_ON();
    }
    public void SkillR_4()
    {
        SkillOn.skOn.SkillR4_ON();
    }
    #endregion
    #region 데미지 받는 관련
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer).Equals("MonsterAttack")) // 레이어 이름이 MonsterAttack 일경우
        {
            GameObject mob = other.gameObject;

            switch (mob.name) // 충돌한 오브젝트 이름을 받아와서 이름에 따라 관련 로직수행
            {
                case "Monster_1":
                    Monster_Stats monsterStats = other.gameObject.GetComponent<Monster_Stats>();
                    if (monsterStats != null)
                    {
                        HitDamage(monsterStats.MONSTER_DAMAGE);
                        break;
                    }
                    break;
                case "b":

                    break;
                case "c":

                    break;
            }
        }       
    }
    public void HitDamage(float _value)
    {
        plSp.CURRENT_HP -= _value;
        if (plSp.CURRENT_HP <= 0)
        {
            plSp.CURRENT_HP = 0;
            //죽는 애니메이션 발동
            // 모든 기능을 끄는 함수 발동
        }
    }
    #endregion
    // 이것도 아마 이벤트키로 들어간듯?
    public void IsNav(bool _is)
    {
        nav.enabled = _is;
    }

    //애니메이션 이벤트키
    void IsRollFalse() // 스킬이나 다른 움직임 처리후 움직임 가능하게하는 함수
    {
        isRoll = false;
        ani.SetInteger("AniIndex", 0);
    }
    void damafeSensorON() //캐릭터 데미지 처리하는 콜라이더 키기 함수
    {
        damageSensor.isTrigger = true;
    }
    #region UI에 전달해주는 정보 함수

    #endregion
}
