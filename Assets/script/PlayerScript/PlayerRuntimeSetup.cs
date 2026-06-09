using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =====================================================================
// PlayerRuntimeSetup : 플레이어 생성 시 외부 스크립트와의 연동을 담당
// =====================================================================
// [역할]
// - 플레이어 런타임 생성 후 외부 매니저들과 참조 연결
// - Player.cs, PlayerSpecs.cs 등 다른 스크립트를 건드리지 않고
//   연동 코드를 이 스크립트에 집중
//
// [호출 순서]
// PlayerManager.Awake() → 플레이어 생성 → PlayerRuntimeSetup.Awake() → 외부 연동
// =====================================================================
public class PlayerRuntimeSetup : MonoBehaviour
{
    void Start()
    {
        SetupWeaponManager();
        SetupUISkillCoolTime();
    }

    void Update()
    {

    }

    [Header("── 무기 설정 ──────────────────")]
    [SerializeField] GameObject defaultWeapon;      // 기본 무기 오브젝트 (플레이어 하위)
    [SerializeField] GameObject[] weaponObjects;    // 모든 무기 오브젝트 배열 (플레이어 하위)
    [SerializeField] Player player; //플레이어 스크립트

    // ------------------------------------------------------------------
    // PlayerWeaponManager에 이 플레이어의 무기 오브젝트 정보 전달
    // ------------------------------------------------------------------
    void SetupWeaponManager()
    {
        if (PlayerWeaponManager.Instance == null)
        {
            Debug.LogWarning("[PlayerRuntimeSetup] PlayerWeaponManager를 찾을 수 없습니다.");
            return;
        }
        PlayerWeaponManager.Instance.Setup(defaultWeapon, weaponObjects);
    }
    
    void SetupUISkillCoolTime()
    {
        Debug.Log("셋업쿨타임실행");
        if(UI.Instance == null)
        {
            return;
        }
        player.SkillCoolSetup(UI.Instance.GetSkillCoolTimes());
        
    }
}
