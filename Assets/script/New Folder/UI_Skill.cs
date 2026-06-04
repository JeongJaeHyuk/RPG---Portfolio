using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skill : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Skill_Manager skillManager; // 인스펙터창에서 드래그 엔 드롭으로 설정 해야함
    [SerializeField] List<Skill> allSkills = new List<Skill>(); // 스킬 정보들
    [SerializeField] List<Skill_Database> skillDatabases = new List<Skill_Database>();
    [SerializeField] Transform content; // 생성한 스킬슬롯을 가지고있을 위치
    [SerializeField] Transform bottomSkillIconTrans;
    [SerializeField] List<GameObject> listBottomSkillIcon;
    [SerializeField] PlayerProgression pps;
    [SerializeField] UI ui;
    [SerializeField] Text skillText;

    private void Awake()
    {
        allSkills = skillManager.GetAllSkills(); // csv데이터를 받아 가지고있는 스킬정보를 전부다 allSkills에 대입
        skillDatabases = Skill_Manager.Instance.GetSkillData();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Transform parentUI = gameObject.transform.root;
        ui = parentUI.GetComponent<UI>();
        pps = ui.GetPlayerProg();
        pps.SpLev += skillPointText;
    }


    void OnEnable()
    {
        UI.instance.isSkillOpen = true;;
    }
    void OnDisable()
    {
        UI.instance.isSkillOpen = false;
    }
    void Start()
    {
        GetSkillIcon();
        IniSkillSlot();
    }
    // 이것도 수정이 좀 필요함 12.17일 여기서부터시작
    void IniSkillSlot()
    {
        int i = 1;
        foreach(Skill _skill in allSkills)
        {
            foreach(Skill_Database _skillDb in skillDatabases)
            {
                // 스킬객체가 스킬데이터베이스에게 필요한정보를 받아가는 코드
                if (_skillDb.skill_Id == _skill.skillName)
                {
                    _skill.coolTimeLevel = _skillDb.skill_CoolTime;
                    _skill.damageLevel = _skillDb.skill_Damage;
                    _skill.maxLevel = _skillDb.maxLevel;
                    _skill.skillIcon = _skillDb.skill_Icon;
                    _skill.mpCost = _skillDb.skill_MpCost;
                }
            }
            GameObject objPrefab = Resources.Load<GameObject>("Prefab/UI/Skill_Slot_"); // 게임오브젝트 Skill_Slot_라는 이름의 오브젝트정보 가져오기
            GameObject obj = GameObject.Instantiate<GameObject>(objPrefab, content); // 생성과 동시에 부모 지정
            string name = "Skill_Slot_" + i; // 스킬슬롯 오브젝트 이름
            obj.name = name; // 이름 대입
            SkillSlot skillSlot = obj.GetComponent<SkillSlot>(); // 생성한 오브젝트에서 스킬슬롯 컴포넌트 찾기
            skillSlot.StartSetSkills(_skill); // 찾으면 스킬슬롯정보에 csv에서 찾은 스킬정보들을 각각 대입 (참조 타입 가져오는거니까 애도 참조타입으로해서 그냥 가져가는 형식으로 하는게 좋아보이려나
            skillSlot.GetSkillData(_skill);
            // csv로 받아온 스킬레벨이 0보다 클경우 즉 1이상일경우
            if(_skill.CURRENT_LEVEL > 0)
            {
                // 해당 스킬의 이름정보를 가져와 함수실행
                NameSkillLock(_skill.skillName);
            }
            Skill_CoolTime_Set(_skill.skillName, _skill); ;
            i++; //이름 숫자 증가;
            skillSlot.UI_Set(transform.GetComponent<UI_Skill>());
        
        }
        
    }
    // 스킬 레벨이 변경되었을 때 변경정보 받기 (csv파일에 저장할 떄 필요하기 떄문)
    public void PlayerGetAllSkills(string _skillName, int _currentLevel)
    {
        for (int i = 0; i < allSkills.Count; i++)
        {
            if (allSkills[i].skillName == _skillName)
            {
                allSkills[i].CURRENT_LEVEL = _currentLevel;
                break;
            }
        }
    }
    // 스킬포인트 사용위한 정보를 보내주는 함수
    public PlayerProgression PlayerProg()
    {
        return pps;
    }
    // 스킬 포인트 정보 텍스트에 뿌리기
    void skillPointText(float _value)
    {
       skillText.text = pps.SKILL_POINT.ToString();
    }
    // 스킬 아이콘 오브젝트 알려주는 코드
    void GetSkillIcon()
    {
        for (int i = 0; i < bottomSkillIconTrans.childCount; i++)
        {
            GameObject obj = bottomSkillIconTrans.GetChild(i).gameObject;
            listBottomSkillIcon.Add(obj);
        }

    }
    // 스킬 잠김 해제 (스킬 레벨 0이었던게 1로 상승하여 습득)
    public void NameSkillLock(string _skillName)
    {
        switch (_skillName)
        {
            case "rolling":
                {
                    listBottomSkillIcon[0].transform.GetChild(1).gameObject.SetActive(false);
                    player.isSkill_LShift = false;
                    break;
                }
            case "bottom cutting":
                {
                    listBottomSkillIcon[1].transform.GetChild(1).gameObject.SetActive(false);
                    player.isSkill_Q = false;
                    break;
                }
            case "Take it down":
                {
                    listBottomSkillIcon[2].transform.GetChild(1).gameObject.SetActive(false);
                    player.isSkill_W = false;
                    break;
                }
            case "Blade storm":
                {
                    listBottomSkillIcon[3].transform.GetChild(1).gameObject.SetActive(false);
                    player.isSkill_E = false;
                    break;
                }
            case "Finish Attack":
                {
                    listBottomSkillIcon[4].transform.GetChild(1).gameObject.SetActive(false);
                    player.isSkill_R = false;
                    break;
                }
        }
    }
    public void Skill_CoolTime_Set(string _skillName, Skill _skill)
    {
        switch (_skillName)
        {
            case "rolling":
                {
                    listBottomSkillIcon[0].GetComponent<Skill_CoolTime>().skillSet(_skill);
                    break;
                }
            case "bottom cutting":
                {
                    listBottomSkillIcon[1].GetComponent<Skill_CoolTime>().skillSet(_skill);
                    break;
                }
            case "Take it down":
                {
                    listBottomSkillIcon[2].GetComponent<Skill_CoolTime>().skillSet(_skill);
                    break;
                }
            case "Blade storm":
                {
                    listBottomSkillIcon[3].GetComponent<Skill_CoolTime>().skillSet(_skill);
                    break;
                }
            case "Finish Attack":
                {
                    listBottomSkillIcon[4].GetComponent<Skill_CoolTime>().skillSet(_skill);
                    break;
                }
        }
    }
}
