using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 감지 센서 (SphereCollider용)
/// 사용법: Monster의 자식 오브젝트에 SphereCollider와 함께 부착
/// </summary>
public class Monster_Check : MonoBehaviour
{
    Monster_Move mobMove;
    Monster_Attack_Hit mobAttack;
    Monster_Move monsterMove;
    GameObject monster;

    void Awake()
    {
        // 부모 오브젝트 찾기
        monster = transform.parent.gameObject;
        monsterMove = transform.parent.GetComponent<Monster_Move>();

        // ✅ 최적화: Awake에서 한 번만 GetComponent (매 프레임마다 호출 방지)
        mobMove = monster.GetComponent<Monster_Move>();
        mobAttack = monster.GetComponent<Monster_Attack_Hit>();

        // 에러 체크
        if (monster == null)
            Debug.LogError($"{gameObject.name}: 부모 오브젝트를 찾을 수 없습니다!");
        if (mobMove == null)
            Debug.LogError($"{gameObject.name}: 부모에 Monster_Move를 찾을 수 없습니다!");
        if (mobAttack == null)
            Debug.LogError($"{gameObject.name}: 부모에 Monster_Attack_Hit를 찾을 수 없습니다!");
    }

    /// <summary>
    /// 플레이어가 감지 범위 안에 들어올 때
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        string name = LayerMask.LayerToName(other.gameObject.layer);
        if (name == "Player")
        {
            // ✅ 이미 Awake에서 GetComponent 했으므로 바로 사용
            if (mobMove != null && mobAttack != null)
            {
                mobMove.SetPlayer(other.gameObject);
                mobAttack.SetPlayer(other.gameObject);
            }
        }
    }

    /// <summary>
    /// 플레이어가 감지 범위를 벗어날 때
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        string name = LayerMask.LayerToName(other.gameObject.layer);
        if (name == "Player")
        {
            if (mobMove != null && mobAttack != null)
            {
                // 플레이어 정보 제거
                mobMove.SetPlayer(null);
                mobAttack.SetPlayer(null);
                // ✅ 캐싱된 참조는 유지 (다시 GetComponent 할 필요 없음)
                monsterMove.ISIDLE = true;  // 제자리로 돌아가기
            }
        }
    }
}
