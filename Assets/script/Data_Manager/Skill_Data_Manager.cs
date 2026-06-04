using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class Skill_Data_Manager : MonoBehaviour
{
    string playerSkillpath = string.Empty;
    string skillDataPath = string.Empty;
    private void Start()
    {
        playerSkillpath = Application.dataPath + "/" + "Resources" + "/" + "SkillDataPath" + "/" + "Player_Skill_Data.csv";
        skillDataPath = Application.dataPath + "/" + "Resources" + "/" + "SkillDataPath" + "/" + "SkillAllData.csv";
    }

    // csv파일을 불러와서 Skill_Data_Manager를 알고 있는곳에서 ex) List<Skill> listSkill = new List<Skill>(); 이후 listSkill = (Skill_Data_Manager)변수.LoadSkillsFromCSV() 사용하면 불러옴
    public List<Skill> LoadSkillsFromCSV() // 테이블에 추가될것들있으면 손써야함
    {
        List<Skill> skills = new List<Skill>();
        // 파일이 존재하는지 확인
        if (File.Exists(playerSkillpath))
        {
            string line = string.Empty;
            using (StreamReader sr = new StreamReader(playerSkillpath))
            {
                line = sr.ReadLine(); // 첫번쨰 줄 읽기 
                // 파일 끝까지 줄 단위로 읽기
                while ((line = sr.ReadLine()) != null)
                {
                    
                    // 줄을 쉼표로 구분하여 배열로 변환
                    string[] data = line.Split(',');
                    int culum = data.Length;
                    if (data.Length == culum) // 필요한 컬럼이 n개일 경우
                    {
                        // 각 데이터를 Skill 객체에 할당
                        string skillName = data[0];
                        string iconName = data[1];
                        string description = data[2];
                        int currentLevel = int.Parse(data[3]);
                        int maxLevel = int.Parse(data[4]);
                        float cooldown = float.Parse(data[5]);
                        string skilltype = data[6];
                        // Skill 객체 생성
                        Skill skill = new Skill
                        {
                            skillName = skillName,
                            description = description,
                            CURRENT_LEVEL = currentLevel,
                            MAX_LEVEL = maxLevel,
                            skillType = skilltype,
                        };

                        skills.Add(skill); // 리스트에 추가
                    }
                }
            }
        }
        else
        {
            Debug.LogError("SkillData.csv 파일이 존재하지 않습니다.");
        }

        return skills;
    }


    // Skills를 CSV로 저장하기 이건 아직 사용안함
    public void SaveSkillsToCSV(List<Skill> skills)
    {
        List<string> lines = new List<string>();
        lines.Add("skillName,icon,description,currentLevel,maxLevel,cooldown,skillType");

        foreach (Skill skill in skills)
        {
            string line = $"{skill.skillName},{skill.skillIcon.name},{skill.description},{skill.CURRENT_LEVEL},{skill.MAX_LEVEL},{skill.CURRENT_COOLTIME}.{skill.skillType}";
            lines.Add(line);
        }

        // 파일 경로에 데이터 쓰기
        File.WriteAllLines(playerSkillpath, lines.ToArray());
    }

    // 이걸 기준으로 skillSlot생성하는 코드 구현할것
    public List<Skill_Database> Skill_All_DataToCSV()
    {
        // 같은 이름의 스킬을 합치기 위한 Dictionary
        Dictionary<string, SkillDataBuilder> skillDict = new Dictionary<string, SkillDataBuilder>();

        if (File.Exists(skillDataPath))
        {
            string line = string.Empty;
            using (StreamReader sr = new StreamReader(skillDataPath))
            {
                line = sr.ReadLine(); // 첫번쨰 줄 읽기 (헤더)
                // 파일 끝까지 줄 단위로 읽기
                while ((line = sr.ReadLine()) != null)
                {
                    // 빈 줄 건너뛰기
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // 줄을 쉼표로 구분하여 배열로 변환
                    string[] data = line.Split(',');

                    if (data.Length >= 4) // 필요한 컬럼이 4개 이상일 경우
                    {
                        string skillId = data[0].Trim();
                        float coolTime = float.Parse(data[2].Trim());
                        float damage = float.Parse(data[3].Trim());
                        float mpCost = float.Parse(data[4].Trim());

                        // 같은 이름이 있으면 배열에 추가, 없으면 새로 생성
                        if (!skillDict.ContainsKey(skillId))
                        {
                            skillDict[skillId] = new SkillDataBuilder
                            {
                                skill_Id = skillId
                            };
                        }

                        skillDict[skillId].coolTimes.Add(coolTime);
                        skillDict[skillId].damages.Add(damage);
                        skillDict[skillId].mpCost.Add(mpCost);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("SkillAllData.csv 파일이 존재하지 않습니다: " + skillDataPath);
        }

        // Dictionary를 List로 변환
        List<Skill_Database> list = new List<Skill_Database>();
        foreach (var builder in skillDict.Values)
        {
            list.Add(new Skill_Database
            {
                skill_Id = builder.skill_Id,
                skill_Icon = ResourcesLoadManager.Instance.GetRcSkillIcon(builder.skill_Id),
                skill_CoolTime = builder.coolTimes.ToArray(),
                maxLevel = builder.coolTimes.Count,
                skill_Damage = builder.damages.ToArray(),
                skill_MpCost = builder.mpCost.ToArray()
            });
        }

        return list;
    }

    // 같은 이름의 스킬 데이터를 모으기 위한 헬퍼 클래스
    private class SkillDataBuilder
    {
        public string skill_Id;
        public List<float> coolTimes = new List<float>();
        public List<float> damages = new List<float>();
        public List<float> mpCost = new List<float>();
    }
}
