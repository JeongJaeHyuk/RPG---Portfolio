using UnityEngine;

public class Monster_State : MonoBehaviour
{
    public Monster_Range monsterRange;
    public Monster_Spec monsterSpec;
    public Animator ani;
    [SerializeField] Collider attackCollider; // 공격 판정 콜라이더

    GameObject player;   // 감지된 플레이어 참조

    protected bool isIdle;       // 대기 상태
    protected bool isAttacking;  // 공격 중
    protected bool isDead;       // 사망

    protected virtual void Awake()
    {
        monsterRange = GetComponentInChildren<Monster_Range>();
        monsterSpec  = GetComponent<Monster_Spec>();
        ani          = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        isIdle      = false;
        isAttacking = false;
        isDead      = false;
        player      = null;
        ani.SetInteger("Move", 0);
        ani.SetInteger("Attack", 0);
    }

    // Monster_Range에서 플레이어 감지/이탈 시 호출
    public void SetPlayer(GameObject _player)
    {
        player = _player;
        if (player != null)
        {
            isIdle = false; // 플레이어 감지되면 대기 해제
        }
    }

    public GameObject GetPlayer()      { return player; }
    public bool GetIsIdle()            { return isIdle; }
    public bool GetIsAttacking()       { return isAttacking; }
    public bool GetIsDead()            { return isDead; }

    // 이동 애니메이션 재생
    public virtual void EnterMove()
    {
        ani.SetInteger("Move", 1);
    }

    // 이동 애니메이션만 중단 (isIdle 변경 없음 - 추적 로직은 유지)
    public virtual void StopMoveAnim()
    {
        ani.SetInteger("Move", 0);
    }

    // 제자리 복귀 완료 시 호출 - 이동 완전 중단
    public virtual void EnterIdle()
    {
        isIdle = true;
        ani.SetInteger("Move", 0);
    }

    // 공격 시작 - 1,2 랜덤으로 공격 모션 선택
    public virtual void EnterAttack()
    {
        isAttacking = true;
        ani.SetInteger("Move", 0);
        ani.SetInteger("Attack", Random.Range(1, 3));
    }

    // 공격 히트 프레임 시작 - 이벤트키로 호출
    public virtual void AttackColliderOn()
    {
        attackCollider.enabled = true;
    }

    // 공격 히트 프레임 종료 - 이벤트키로 호출
    public virtual void AttackColliderOff()
    {
        attackCollider.enabled = false;
    }

    // 공격 애니메이션 끝날 때 이벤트키로 호출
    public virtual void ExitAttack()
    {
        isAttacking = false;
        ani.SetInteger("Attack", 0); // Attack 파라미터 초기화
    }

    // 사망 시 호출 - 사망 애니메이션 재생
    public virtual void EnterDead()
    {
        isDead = true;
        ani.SetTrigger("Die");
    }
}
