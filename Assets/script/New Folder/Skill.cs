using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]
public class Skill
{
    public string skillName; // 스킬이름
    public Sprite skillIcon; // 스킬아이콘 Resources.Load로 불러오기
    public string description; // 스킬 설명란    
    public int currentLevel; // 프로퍼티로 변경
    public int maxLevel; // 스킬 최대 레벨  프로퍼티로 변경
    public event Action LevelChage;
    public event Action CoolChage;
    public int CURRENT_LEVEL
    {
        get => currentLevel;
        set
        {
            currentLevel = value;
            if (currentLevel <= maxLevel)
            {
                LevelChage?.Invoke();
            }
            else
                currentLevel = maxLevel;
        }
    }

    public int MAX_LEVEL
    {
        get => maxLevel;
        set
        {
            maxLevel = value;
            LevelChage?.Invoke();
        }
    }
    public string skillType; // 스킬타입 액티브 or 패시브

    // 스킬 레벨에 따라 공격력 쿨타임 마나소모량
    public float[] damageLevel;
    public float CURRENT_DAMAGE
    {
        get
        {
            if (damageLevel == null || CURRENT_LEVEL <= 0 || CURRENT_LEVEL > damageLevel.Length)
            {
                return 0;
            }
            return damageLevel[CURRENT_LEVEL - 1];
        }
    }

    public float[] coolTimeLevel;
    public float CURRENT_COOLTIME
    {
        get
        {
            if (coolTimeLevel == null || CURRENT_LEVEL <= 0 || CURRENT_LEVEL > coolTimeLevel.Length)
            {
                return 0;
            }    
            return coolTimeLevel[CURRENT_LEVEL - 1];
        }
    }

    public float[] mpCost;
    public float MP_COST
    {
        get
        {
            if (mpCost == null || CURRENT_LEVEL <= 0 || CURRENT_LEVEL > mpCost.Length)
            {
                return 0;
            } 
            return mpCost[CURRENT_LEVEL - 1];
        }
    }


}
