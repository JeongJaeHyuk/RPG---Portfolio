   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOn : MonoBehaviour
{
    public static SkillOn skOn = null;
    [SerializeField] GameObject skillQ;
    [SerializeField] GameObject skillW;
    [SerializeField] GameObject skillE_1;
    [SerializeField] GameObject skillE_2;
    [SerializeField] GameObject skillR;
    private void Awake()
    {
        if (skOn == null)
        {
            skOn = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //리소스정보를 모아서 오브젝트를 소환하는 함수들
        rcCreateSkillQ();
        rcCreateSkillW();
        rcCreateSkillE();
        rcCreateSkillR();
        //

    }
    void Start()
    {
        
    }
    #region 여기서 사용하는함수들
    // 리소스 폴더에서 스킬 Q정보를 가져와서 생성하는 함수
    void rcCreateSkillQ()
    {
        GameObject obj = Resources.Load<GameObject>("Prefab/SkillEffect/Sword Slash_Q"); // 로드함수로 정보 불러오기
        skillQ = GameObject.Instantiate<GameObject>(obj); // 불러온 정보를 생성하고 변수에 대입
        skillQ.name = obj.name; // 정보의 이름을 생성한 오브젝트이름에 대입
        skillQ.transform.position = transform.position; // 생성한 오브젝트위치를 지금오브젝트위치로 옮김
        skillQ.transform.SetParent(transform); // 생성한 오브젝트의 부모 설정
        skillQ.SetActive(false); // 오브젝트 비활성화
    }
    // 리소스 폴더에서 스킬 W정보를 가져와서 생성하는 함수
    void rcCreateSkillW()
    {
        GameObject obj = Resources.Load<GameObject>("Prefab/SkillEffect/Spikes attack_W"); // 로드 함수로 정보 불러오기
        skillW = GameObject.Instantiate<GameObject>(obj); // 불러온 정보를 생성하고 변수에 대입
        skillW.name = obj.name; // 정보의 이름을 생성한 오브젝트이름에 대입
        skillW.transform.position = transform.position; // 생성한 오브젝트위치를 지금오브젝트위치로 옮김
        skillW.transform.SetParent(transform); // 생성한 오브젝트의 부모 설정
        skillW.SetActive(false); // 오브젝트 비활성화
    }
    
    // 리소스폴더에서 스킬 E정보를 가져와서 생성하는 함수
    void rcCreateSkillE()
    {
        GameObject obj1 = Resources.Load<GameObject>("Prefab/SkillEffect/Sword Slash_E-1"); // 로드함수로 정보 불러오기
        GameObject obj2 = Resources.Load<GameObject>("Prefab/SkillEffect/Sword Slash_E-2"); // 로드함수로 정보 불러오기
        skillE_1 = GameObject.Instantiate<GameObject>(obj1); // 불러온 정보를 생성하고 변수에 대입
        skillE_2 = GameObject.Instantiate<GameObject>(obj2); // 불러온 정보를 생성하고 변수에 대입
        skillE_1.name = obj1.name; // 정보의 이름을 생성한 오브젝트이름에 대입
        skillE_2.name = obj2.name; // 정보의 이름을 생성한 오브젝트이름에 대입
        skillE_1.transform.position = transform.position; // 생성한 오브젝트위치를 지금오브젝트위치로 옮김
        skillE_2.transform.position = transform.position; // 생성한 오브젝트위치를 지금오브젝트위치로 옮김
        skillE_1.transform.SetParent(transform); // 생성한 오브젝트의 부모 설정
        skillE_2.transform.SetParent(transform); // 생성한 오브젝트의 부모 설정
        skillE_1.SetActive(false); // 오브젝트 비활성화
        skillE_2.SetActive(false); // 오브젝트 비활성화
    }
    void rcCreateSkillR()
    {
        GameObject obj = Resources.Load<GameObject>("Prefab/SkillEffect/Sword Slash_R"); // 로드함수로 정보 불러오기
        skillR = GameObject.Instantiate<GameObject>(obj); // 불러온 정보를 생성하고 변수에 대입
        skillR.name = obj.name; // 정보의 이름을 생성한 오브젝트이름에 대입
        skillR.transform.position = transform.position; // 생성한 오브젝트위치를 지금오브젝트위치로 옮김
        skillR.transform.SetParent(transform); // 생성한 오브젝트의 부모 설정
        skillR.SetActive(false); // 오브젝트 비활성화
    }
    #endregion
    #region 외부에서 사용하는 함수들
    // Q 이펙트 사용
    public void SkillQ_ON(GameObject _skillPos, Skill _skillData)
    {
        skillQ.transform.position = _skillPos.transform.position; // 매개변수의 위치로 이동
        skillQ.transform.forward = _skillPos.transform.forward; // 스킬이 사용하는 플레이어 정면으로 할수있게 설정

        // 스킬 데이터 전달 (SKill_Q 컴포넌트에)
        Skill_Q skillQComponent = skillQ.GetComponent<Skill_Q>();
        if (skillQComponent != null)
        {
            skillQComponent.SetSkillData(_skillData);
        }

        skillQ.SetActive(true); // 스킬 오브젝트 활성화
        for (int i = 0; i < skillQ.transform.childCount; i++)
        {
            skillQ.transform.GetChild(i).gameObject.SetActive(true); // 스킬 오브젝트 활성화
        }
    }
    public void SkillQ_Pos(GameObject _skillPos)
    {
        skillQ.transform.SetParent(_skillPos.transform); // 부모를 매개변수로받은곳으로 설정
    }
    public void skillQ_PosRollBack()
    {
        skillQ.transform.SetParent(gameObject.transform); // 부모를 다시 이곳으로 설정
    }
    // W 이펙트 사용
    public void SkillW_ON(GameObject _skillPos, Skill _skillData)
    {
        skillW.transform.position = _skillPos.transform.position; // 매개변수의 위치로 이동
        skillW.transform.forward = _skillPos.transform.forward; // 스킬이 사용하는 플레이어 정면으로 할수있게 설정

        // 스킬 데이터 전달 (Skill_W 컴포넌트에)
        Skill_W skillWComponent = skillW.GetComponent<Skill_W>();
        if (skillWComponent != null)
        {
            skillWComponent.SetSkillData(_skillData);
        }

        skillW.SetActive(true);
        for (int i = 0; i < skillW.transform.childCount; i++)
        {
            skillW.transform.GetChild(i).gameObject.SetActive(true); // 스킬 오브젝트 활성화
        }
    }
    // E 이펙트 사용
    public void SkillE1_ON(GameObject _skillPos, Skill _skillData)
    {
        skillE_1.transform.position = _skillPos.transform.position;// 매개변수의 위치로 이동
        skillE_1.transform.forward = _skillPos.transform.forward; // 스킬이 사용하는 플레이어 정면으로 할수있게 설정

        // 스킬 데이터 전달 (Skill_E 컴포넌트에)
        Skill_E skillEComponent = skillE_1.GetComponent<Skill_E>();
        if (skillEComponent != null)
        {
            skillEComponent.SetSkillData(_skillData);
        }

        skillE_1.SetActive(true); // 스킬 오브젝트 활성화
    }
    public void SkillE2_ON(GameObject _skillPos, Skill _skillData)
    {
        skillE_2.transform.position = _skillPos.transform.position;// 매개변수의 위치로 이동
        skillE_2.transform.forward = _skillPos.transform.forward; // 스킬이 사용하는 플레이어 정면으로 할수있게 설정

        // 스킬 데이터 전달 (Skill_E 컴포넌트에)
        Skill_E skillEComponent = skillE_2.GetComponent<Skill_E>();
        if (skillEComponent != null)
        {
            skillEComponent.SetSkillData(_skillData);
        }

        skillE_2.SetActive(true); // 스킬 오브젝트 활성화
    }
    // R 이펙트 사용과 콜라이더를 사용하기 위해 각별로 이벤트키 이용
    public void SkillR1_ON(GameObject _skillPos, Skill _skillData)
    {
        // 스킬을 사용하게되면 처음에 좌표값으로 이동하기 위해 설정
        skillR.transform.position = _skillPos.transform.position;
        skillR.transform.forward = _skillPos.transform.forward;

        // 모든 자식 오브젝트의 Skill_R 컴포넌트에 스킬 데이터 전달
        for (int i = 0; i < skillR.transform.childCount; i++)
        {
            Skill_R skillRComponent = skillR.transform.GetChild(i).GetComponent<Skill_R>();
            if (skillRComponent != null)
            {
                skillRComponent.SetSkillData(_skillData);
            }
        }

        skillR.SetActive(true);
        skillR.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void SkillR2_ON()
    {
        skillR.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void SkillR3_ON()
    {
        skillR.transform.GetChild(2).gameObject.SetActive(true);
    }
    public void SkillR4_ON()
    {
        skillR.transform.GetChild(3).gameObject.SetActive(true);
        for (int k = 0; k < skillR.transform.GetChild(3).childCount; k++)
        {
            skillR.transform.GetChild(3).GetChild(k).gameObject.SetActive(true);
        }
    }
    #endregion
}
