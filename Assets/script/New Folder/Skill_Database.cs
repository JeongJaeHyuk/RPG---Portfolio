using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Skill_Database
{
    public string skill_Id;       //스킬이름
    public int maxLevel;
    public Sprite skill_Icon;
    public float[] skill_CoolTime;    // 스킬 쿨타임    index 0 부터 순차적으로
    public float[] skill_Damage;    // 스킬 공격력    index 0 부터 순차적으로

    public float[] skill_MpCost; // 스킬 사용 마나

}
