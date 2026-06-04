using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    //플레이어 EXP(경험치). LEVEL(레벨), SP(스킬포인트)
    public delegate void EXPSatasChage(float newHP);
    public event EXPSatasChage ExpChage;
    public event EXPSatasChage LevChage;
    public event EXPSatasChage SpLev;
    #region 스킬 레벨 관련
    float spLev;

    // 프로퍼티
    public float SPLEVELUP
    {
        get => spLev;
        set
        {
            spLev = value;
            SpLev?.Invoke(spLev);
            Debug.Log("스킬포인트 사용: "+ spLev);
        }
    }
    // 스킬 포인트 소비 함수
    public bool UseSkillPoint()
    {
        if (spLev > 0)
        {
            SPLEVELUP--; // 이벤트 호출 포함
            return true; // 사용 성공
        }
        else
        {
            Debug.Log("스킬포인트가 부족합니다.");
            return false; // 사용 실패
        }
    }
    public void AddSkillPoint()
    {
        SPLEVELUP++;
    }
    #endregion
    #region 경험치및 레벨업 프로퍼티 및 함수
    // 설명은 위와 동일 (경험치)
    float maxExp = 0f;
    float currentExp = 0f;
    float remainingExp;

    public float CURRENT_EXP
    {
        get => currentExp;
        set
        {
            currentExp = Mathf.Clamp(value, 0, maxExp);
            ExpChage?.Invoke(currentExp / maxExp);
            if (currentExp >= maxExp)
            {
                Debug.Log("currentExp경험치 초과로 if들어옴");
                LevelUp();
            }
            remainingExp = 0f;
        }
    }
    public float MAX_EXP
    {
        get => maxExp;
        set
        {
            maxExp = value;
            ExpChage?.Invoke(currentExp / maxExp); // 이벤트호출사용을 위해서 즉 무엇이 변할떄 업데이트가 되기위해선 Invoke함수가 실행되는것이 필수
        }
    }
    float maxLevel;
    float currentLevel = 1f;
    public float CURRENT_LEVEL
    {
        get => currentLevel;
        set
        {
            currentLevel = Mathf.Clamp(value, 1, maxLevel);
            LevChage?.Invoke(currentLevel);
        }
    }
    public float MAX_LEVEL
    {
        get => maxLevel;
        set
        {
            maxLevel = value;
        }
    }
    // 이것도 수정이 필요함
    public void LevelUp()
    {
        CURRENT_EXP = 0;
        Debug.Log("레벨업전 :" + CURRENT_LEVEL);
        CURRENT_LEVEL++;
        Debug.Log("레벨업후 :" + CURRENT_LEVEL);
        AddSkillPoint(); // 스킬포인트 1흭득
        //최대 경험치 증가
        MAX_EXP += maxExp * 0.7f;
        Debug.Log(maxExp);
    }
    #endregion
    //PlayerDataManager에서 불러오고 저장하는 코드 사용할꺼라 싱글톤으로 사용하지말고 각자 Player오브젝트에 가지고있기
    private void Awake()
    {

    }

    // 장비 장착했을때 변수 변동되는 함수 사용하기.
    void Start()
    {
        // 임시로 선언해둔것 무조건 max가 먼저 선언되어야함 인보크함수떄매 어쩔수없음
        MAX_EXP = 30f;
        MAX_LEVEL = 50f;
        CURRENT_EXP = 10f;
        CURRENT_LEVEL = 1f;
    }

}
