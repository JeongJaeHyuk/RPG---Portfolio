using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monster_Stats : MonoBehaviour
{
    public event Action<float> MonsterHP;
    float currentMonsterHp; // 현재체력
    float maxMonsterHp;// 최대체력
    float monsterDamage; // 공격력
    float monsterRange; // 몬스터 공격사거리
    float monsterAttackTime; // 공격속도(n초 마다 공격)
    float monsExp; // 몬스터가 주는 경험치
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

    public float MONSTER_DAMAGE
    {
        get => monsterDamage;
        set
        {
            monsterDamage = value;
        }
    }
    public float MONSTER_EXP
    {
        get => monsExp;
        set
        {
            monsExp = value;
        }
    }
    public float MONSTER_RANGE
    {
        get => monsterRange;
        set
        {
            monsterRange = value;
        }
    }
    public float MONSTER_ATTACKTIME
    {
        get => monsterAttackTime;
        set
        {
            monsterAttackTime = value;
        }
    }
    void Start()
    {
        StartGetStats();
    }
    //스타트 함수에서 사용 그리고 테이블로 만들어서 테이블 정보를 받아오는 함수를 만든후 현재,최대 체력 받아오고 공격력과 주는 경험치량을 대입하는 것이 있는함수
    void StartGetStats()
    {
        //변수대신에 숫자로 대입되어있는것들은 테이블정보받아서 넣어줘야할 숫자들
        MAX_HP = 200f; //테이블로 받아야할 최대 체력
        CURRENT_HP = maxMonsterHp;
        MONSTER_DAMAGE = 25f; // 테이블로 받아야할 몬스터 공격력
        monsExp = 20f; //테이블로 받아야할 경험치량
        MONSTER_RANGE = 2f; // 몬스터 사거리
        monsterAttackTime = 4f; // 몬스터 공격속도
    }
    
}
