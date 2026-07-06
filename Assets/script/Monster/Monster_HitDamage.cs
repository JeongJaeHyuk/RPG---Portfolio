using System.Collections;
using UnityEngine;

public class Monster_HitDamage : MonoBehaviour
{
    Monster_State monsterState;
    Monster_Spec monsterSpec;
    CapsuleCollider capsuleCollider;
    UnityEngine.AI.NavMeshAgent nav;
    int playerLayer;

    void Awake()
    {
        monsterState    = GetComponent<Monster_State>();
        monsterSpec     = GetComponent<Monster_Spec>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav             = GetComponent<UnityEngine.AI.NavMeshAgent>();
playerLayer     = LayerMask.NameToLayer("PlayerBasicAttack");
    }

    void OnEnable()
    {
        capsuleCollider.enabled = true; // 사망 시 꺼진 피격 콜라이더 재활성화
        nav.enabled             = true; // 사망 시 꺼진 NavMeshAgent 재활성화
    }

    // 플레이어 공격 히트박스 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Debug.Log("어택받음");
            PlayerSpecs playerSpecs = other.GetComponentInParent<PlayerSpecs>();
            if (playerSpecs == null) return;
            if (playerSpecs.ComboCount == 0) return; // 공격 이펙트가 아닌 몸통 충돌 무시
            TakeDamage(playerSpecs.GetComboDamage(), playerSpecs.gameObject);
        }
    }

    // 공격 애니메이션 타격 프레임 이벤트키로 호출 - 플레이어에게 데미지 전달
    public void DealDamage()
    {
        if (monsterState.GetIsDead()) return; // 사망 시 공격 차단
        GameObject player = monsterState.GetPlayer();
        if (player == null) return;

        // 추후 플레이어 피격 스크립트 참조 후 데미지 전달
        // player.GetComponent<Player_HitDamage>().TakeDamage(monsterSpec.MONSTER_DAMAGE);
    }

    // 플레이어 공격에 피격 시 호출 - 데미지 연산 및 사망 처리
    public void TakeDamage(float _damage, GameObject _player)
    {
        if (monsterState.GetIsDead()) return; // 사망 시 데미지 차단
        monsterSpec.CURRENT_HP -= _damage;
        Debug.Log($"[{gameObject.name}] 데미지 : {_damage} / 남은 HP : {monsterSpec.CURRENT_HP}");

        if (monsterSpec.CURRENT_HP <= 0)
        {
            capsuleCollider.enabled = false; // 사망 시 피격 콜라이더 비활성화
            nav.enabled             = false; // 사망 시 NavMeshAgent 비활성화
            monsterState.EnterDead();
            GiveExp(_player);
            // 이곳에 골드랑 루트백 소환해야함
            StartCoroutine(SinkDown());
        }
    }

    // 사망 시 플레이어에게 경험치 전달
    void GiveExp(GameObject _player)
    {
        _player.GetComponent<PlayerProgression>().CURRENT_EXP += monsterSpec.MONSTER_EXP;
    }

    // 사망 시 몬스터가 밑으로 가라앉다가 오브젝트 비활성화
    public IEnumerator SinkDown()
    {
        float timer = 0f;
        float duration = 3f; // 가라앉는 시간

        while (timer < duration)
        {
            transform.position += Vector3.down * 0.1f * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
