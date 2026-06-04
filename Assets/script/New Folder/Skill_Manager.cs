using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Manager : MonoBehaviour
{
    public static Skill_Manager Instance { get; private set; }

    [SerializeField] private List<Skill> skills;
    [SerializeField] private Skill_Data_Manager skillDataManager;
    [SerializeField] private List<Skill_Database> skillData;

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    private void Start()
    {
        skills = skillDataManager.LoadSkillsFromCSV(); // 테이블에 저장된 플레이어 스킬 정보 불러오기
        skillData = skillDataManager.Skill_All_DataToCSV(); // 테이블에 저장된 스킬 정보 불러오기
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
