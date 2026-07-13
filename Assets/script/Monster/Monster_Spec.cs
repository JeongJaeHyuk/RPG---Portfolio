using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Monster_Spec : MonoBehaviour
{
    // 몬스터 데이터 스크립트
    public event Action<float> MonsterHP;
    float currentMonsterHp; // 현재체력
    float maxMonsterHp;// 최대체력
    float monsterDamage; // 공격력
    float monsterRange; // 몬스터 공격사거리
    float monsterAttackTime; // 공격속도(n초 마다 공격)
    float monsExp; // 몬스터가 주는 경험치
    int gold;      // 드롭 골드
    int minGold;   // 드롭 골드 최솟값
    int maxGold;   // 드롭 골드 최댓값
    public float MAX_HP
    {
        get => maxMonsterHp;
        set
        {
            maxMonsterHp = value;
            CURRENT_HP = currentMonsterHp;
        }
    }
    public float CURRENT_HP
    {
        get => currentMonsterHp;
        set
        {
            currentMonsterHp = Mathf.Clamp(value, 0, maxMonsterHp);
            MonsterHP?.Invoke(currentMonsterHp / maxMonsterHp);
        }
    }

    public float MONSTER_DAMAGE     { get => monsterDamage;     private set => monsterDamage = value; }
    public float MONSTER_EXP        { get => monsExp;            private set => monsExp = value; }
    public float MONSTER_RANGE      { get => monsterRange;       private set => monsterRange = value; }
    public float MONSTER_ATTACKTIME { get => monsterAttackTime;  private set => monsterAttackTime = value; }
    public int   MONSTER_GOLD       { get => gold;               private set => gold = value; }

    void Awake()
    {
        LoadSpec();
    }

    void Start()
    {
        MONSTER_GOLD = UnityEngine.Random.Range(minGold, maxGold + 1);
    }

    void OnEnable()
    {
        CURRENT_HP   = maxMonsterHp;
        MONSTER_GOLD = UnityEngine.Random.Range(minGold, maxGold + 1); // 리스폰 시 골드 재설정
    }

    void LoadSpec()
    {
        string monsterName = gameObject.name.Split('_')[0];

        if (!System.Enum.TryParse(monsterName, out MonsterType type))
        {
            Debug.LogError($"[Monster_Spec] '{monsterName}'이 MonsterType enum에 없습니다.");
            return;
        }

        MonsterData data = Monster_Data_Manager.Instance.GetData(type);
        if (data == null) return;

        MAX_HP             = data.maxHp;
        MONSTER_DAMAGE     = data.damage;
        MONSTER_RANGE      = data.attackRange;
        MONSTER_ATTACKTIME = data.attackSpeed;
        MONSTER_EXP        = data.exp;
        minGold            = data.minGold;
        maxGold            = data.maxGold;

    }
}
