using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillSlot : MonoBehaviour
{

    // 스킬정보를 보내줄 변수
    [SerializeField] UI_Skill uiSkill;
    [SerializeField] Skill skill;
    public void UI_Set(UI_Skill _skill)
    {
        uiSkill = _skill;
    }
    // 스킬정보
    [SerializeField] TMP_Text skillName; // 스킬 제목
    [SerializeField] Image skillIcon; // 스킬 아이콘
    [SerializeField] TMP_Text description; // 스킬 설명란
    [SerializeField] string now_Damage; // 나중에건들것
    [SerializeField] string next_Damage; // 나중에 건들것
    [SerializeField] TMP_Text skill_Level; // 현재레벨 / 최대레벨을 뿌려줄 텍스트 컴포넌트
    [SerializeField] float coolTime; // 쿨타임
    [SerializeField] TMP_Text skillType; // 스킬타입 ( 액티브 or 패시브 )
                                         
    void Start()
    {
        skill.LevelChage += SkillText;
        skill.LevelChage += SkillDescrion;
    }
    // Update is called once per frame
    void Update()
    {

    }
    // 처음 시작시 스킬의 정보를 받아오는 함수
    public void StartSetSkills(Skill _skill)
    {
        skillIcon.sprite = _skill.skillIcon;
        skillName.text = _skill.skillName;
        description.text = $"{_skill.CURRENT_DAMAGE * 100}% 데미지를 입힙니다.\n 쿨타임 : {_skill.CURRENT_COOLTIME}초"; 
        skill_Level.text = _skill.CURRENT_LEVEL.ToString() + " / " + _skill.MAX_LEVEL.ToString();
        skillType.text = _skill.skillType;
    }
    public void StartSettingSkillSlot(Skill_Database _skillDb)
    {
        skillIcon.sprite = _skillDb.skill_Icon;
        skillName.text = _skillDb.skill_Id;
        // 스킬레벨
        // 스킬타입
    }
    public void GetSkillData(Skill _skill)
    {
        skill = _skill;
        // skill.LevelChage += SkillLevelUp;
    }
    void SkillText()
    {
        skill_Level.text = skill.CURRENT_LEVEL.ToString() + " / " + skill.MAX_LEVEL.ToString();
    }
    void SkillDescrion()
    {
        description.text = $"{skill.CURRENT_DAMAGE * 100}% 데미지를 입힙니다.\n 쿨타임 : {skill.CURRENT_COOLTIME}초"; 
    }
    public void SkillLevelUp()
    {
        // 조건문으로 스킬포인트가 1이상있을때 
        // 스킬포인트 카운트 하나 빼기
        if (uiSkill.PlayerProg().SPLEVELUP > 0)
        {
            skill.CURRENT_LEVEL++;
            uiSkill.PlayerProg().UseSkillPoint();
            if (skill.CURRENT_LEVEL > 0 && skill.CURRENT_LEVEL < 2)
            {
                uiSkill.NameSkillLock(skill.skillName);
            }
        }
        else if (uiSkill.PlayerProg().SPLEVELUP < 0)
        {
            Debug.Log("스킬포인트가 부족합니다");
        }
        // 레벨업 버튼을 눌렀을 때 skillcsv파일을 열어서 저장하는 코드 실행
    }
}
