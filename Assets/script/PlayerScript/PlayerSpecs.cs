using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecs : MonoBehaviour
{
    // 플레이어 공격력, 방어력, 체력, 마나 를 관련하는 스크립트
    // 공격력과 방어력이 레벨에 따라 올라간다면 레벨업을 관리하는 PlayerProgression스크립트에 있는것을 구독해서 함수로 만들어 놓는것이 제일 효율적
    [SerializeField] PlayerProgression pps;
    // 캐릭터 스펙 변수
    float basicDamage; // 기본공격력(레벨에 따른 공격력)
    float waponDamage; // 무기공격력
    float basicDefense; // 기본방어력(레벨에 따른 방어력)
    float headDefense; // 머리 방어구 방어력
    float chestDefense; // 가슴 방어구 방어력
    float totalDamage; // 총공격력 (UI에 뿌려진 공격력과 입힐 데미지)
    float totalDefense; // 총방어력 (UI에 뿌려진 방어력과 방어할 방어력)


    //Hp(체력), MP(마나), Exp(경험치)에 사용되는 것들
    public delegate void SatasChage(float newHP);
    public event SatasChage HpChage;
    public event SatasChage MpChage;
    public event SatasChage TotalDamage;
    public event SatasChage TotalDefense;
    public float BASICDAMAGE
    {
        get => basicDamage;
        set
        {
            basicDamage = value;
            UpdateToTalDamage();
            Debug.Log("기본 공격력이 갱신");           
        }
    }
    public float WAPONDAMAGE
    {
        get => waponDamage;
        set
        {
            waponDamage = value;
            UpdateToTalDamage();
        }
    }
    public float TOTAL_DAMAGE => totalDamage; // 읽기전용 값이 변하면 안되기 때문
    void UpdateToTalDamage()
    {
        totalDamage = basicDamage + waponDamage;
        TotalDamage?.Invoke(totalDamage);
        Debug.Log("총공격력 갱신 : " + totalDamage);
    }
    public float BASICDEFENSE
    {
        get => basicDefense;
        set
        {
            basicDefense = value;
            UpdateToTalDenfense();
            Debug.Log("기본 방어력이 갱신");
        }
    }
    public float HEADEFENSE
    {
        get => headDefense;
        set
        {
            headDefense = value;
            UpdateToTalDenfense();
        }
    }
    public float CHESTDEFENSE
    {
        get => chestDefense;
        set
        {
            chestDefense = value;
            UpdateToTalDenfense();
        }
    }
    public float TOTAL_DEFENSE => totalDefense; // 읽기전용 값이 변하면 안되기 때문
    void UpdateToTalDenfense()
    {
        totalDefense = basicDefense + headDefense + chestDefense;
        TotalDefense?.Invoke(totalDefense);
        Debug.Log("총방어력 갱신 : "+ totalDefense);
    }
    #region 체력 프로퍼티
    // 체력
    float maxHp = 0f;
    public float MAX_HP
    {
        get => maxHp;
        set
        {
            maxHp = value;
            HpChage?.Invoke(currentHp / maxHp);
        }
    }
    float currentHp = 0f;
    public float CURRENT_HP
    {
        get => currentHp;
        set
        {
            currentHp = Mathf.Clamp(value, 0, maxHp); // Hp는 0에서 max사이에서
            HpChage?.Invoke(currentHp / maxHp); // 이벤트를 통해 새로운 비율(정보) Hp전달
        }
    }
    //MAX 프로퍼티는 최대 체력이나 마나, 경험치를 올려주는 아이템 또는 레벨업을 했을경우 최대값을 올려줄떄 사용되기위한 프로퍼티
    #endregion
    #region 마나 프로퍼티
    // 설명은 위와 동일 (마나)
    float maxMp = 0f;
    float currentMp = 0f;
    public float CURRENT_MP
    {
        get => currentMp;
        set
        {
            currentMp = Mathf.Clamp(value, 0, maxMp);
            MpChage?.Invoke(currentMp / maxMp);
        }
    }
    public float MAX_MP
    {
        get => maxMp;
        set
        {
            maxMp = value;
            MpChage?.Invoke(currentMp / maxMp);
        }
    }
    #endregion
    //PlayerDataManager에서 불러오고 저장하는 코드 사용할꺼라 싱글톤으로 사용하지말고 각자 Player오브젝트에 가지고있기
    private void Awake()
    {
        pps = GetComponent<PlayerProgression>();
    }
    private void OnEnable()
    {
        if (pps != null)
        {
            pps.LevChage += UpdatePlayerSpec;
        }
    }
    private void OnDisable()
    {
            pps.LevChage -= UpdatePlayerSpec;
    }

    // 장비 장착했을때 변수 변동되는 함수 사용하기.
    void Start()
    {
        // LoadGame()에서 CSV 기본값을 읽어서 대입할 예정
    }

    //레벨업을 햇을경우 공격력, 방어력, 체력, 마나 변경 함수
    void UpdatePlayerSpec(float _levValue)
    {
        maxHp = maxHp + maxHp * 0.5f; // 레벨업시 최대 체력 증가량
        maxMp = maxMp + maxMp * 0.3f; // 레벨업시 최대 마나 증가량
        BASICDAMAGE = basicDamage + basicDamage * 0.7f; // 레벨업시 최대 공격력 증가량
        Debug.Log("공격력 증가 : " + basicDamage);
        BASICDEFENSE = basicDefense + basicDefense * 0.7f; // 레벨업시 최대 방어력 증가량
        Debug.Log("방어력증가 :" + basicDefense);
        CURRENT_HP = maxHp;
        CURRENT_MP = maxMp;
    }
    //플레이어의 공격력을 보내주는 함수
    public float BasicTotalDamage()
    {
        return totalDamage;
    }
    public float SpearTotalDamage()
    {
        return totalDamage * 1.2f;
    }

    // 콤보별 데미지 배율 (1~4번 콤보)
    readonly float[] comboDamageMultiplier = { 0.7f, 0.8f, 0.9f, 1.0f, 1.2f };
    public int ComboCount { get; private set; }

    // 애니메이션 이벤트에서 콤보 카운트 설정
    public void SetComboCount(int _count)
    {
        ComboCount = _count;
    }

    // 현재 콤보 카운트 기준 데미지 반환
    public float GetComboDamage()
    {
        return totalDamage * comboDamageMultiplier[ComboCount - 1];
    }
}
