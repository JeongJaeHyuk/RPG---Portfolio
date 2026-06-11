using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Manager : MonoBehaviour
{
    public static Skill_Manager Instance { get; private set; } = null;

    [SerializeField] private List<Skill> skills;
    [SerializeField] private Skill_Data_Manager skillDataManager;
    [SerializeField] private List<Skill_Database> skillData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // Awake에서 로드해야 PlayerManager.Awake()의 ApplyData()에서 참조 가능
        skills    = skillDataManager.LoadSkillsFromCSV();
        skillData = skillDataManager.Skill_All_DataToCSV();
    }

    // 불러온 스킬csv정보를 UI_Skill 이라는곳에 주는 함수
    public List<Skill> GetAllSkills()
    {
        return skills;
    }
    public List<Skill_Database> GetSkillData()
    {
        return skillData;
    }
}
