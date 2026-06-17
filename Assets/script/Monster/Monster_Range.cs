using UnityEngine;

public class Monster_Range : MonoBehaviour
{
    // 몬스터가 플레이어 감지하는 스크립트
    public Monster_State monsterState;
    int playerLayer;

    void Awake()
    {
        monsterState = GetComponentInParent<Monster_State>();
        playerLayer  = LayerMask.NameToLayer("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            monsterState.SetPlayer(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            monsterState.SetPlayer(null);
        }
    }
}
