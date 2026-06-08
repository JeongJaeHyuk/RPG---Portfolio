using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    public static PlayerWeaponManager Instance { get; private set; } = null;

    [Header("플레이어 무기 오브젝트들")]
    [SerializeField] GameObject defaultWeapon;          // 기본 무기 (장착 해제 시)
    [SerializeField] GameObject[] weaponObjects;        // 모든 무기 오브젝트 배열 (플레이어 하위)

    GameObject currentWeapon;                           // 현재 장착된 무기

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 기본 무기 활성화는 PlayerRuntimeSetup.SetupWeaponManager()에서 처리
    }

    // ------------------------------------------------------------------
    // 플레이어 생성 후 PlayerRuntimeSetup에서 호출
    // 플레이어 하위의 무기 오브젝트 배열과 기본 무기를 외부에서 전달받아 설정
    // ------------------------------------------------------------------
    public void Setup(GameObject _defaultWeapon, GameObject[] _weaponObjects)
    {
        defaultWeapon = _defaultWeapon;
        weaponObjects = _weaponObjects;
        ResetToDefaultWeapon();
    }

    /// <summary>
    /// 무기 오브젝트를 바꾸는 함수 (아이템 이름으로 검색)
    /// </summary>
    /// <param name="weaponName">장착할 무기 이름</param>
    public void ChangeWeapon(string weaponName)
    {
        // 1. 모든 무기 비활성화
        if (defaultWeapon != null)
            defaultWeapon.SetActive(false);

        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
                weapon.SetActive(false);
        }

        // 2. weaponName이 null이거나 비어있으면 기본 무기
        if (string.IsNullOrEmpty(weaponName))
        {
            ResetToDefaultWeapon();
            return;
        }

        // 3. 이름이 일치하는 무기 찾아서 활성화
        bool found = false;
        foreach (var weapon in weaponObjects)
        {
            if (weapon != null && weapon.name == weaponName)
            {
                weapon.SetActive(true);
                currentWeapon = weapon;
                found = true;
                Debug.Log($"무기 교체: {weaponName}");
                return;
            }
        }

        // 4. 무기를 못 찾으면 기본 무기로
        if (!found)
        {
            ResetToDefaultWeapon();
        }
    }

    /// <summary>
    /// 기본 무기로 되돌리기
    /// </summary>
    public void ResetToDefaultWeapon()
    {
        // 모든 무기 비활성화
        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }            
        }

        // 기본 무기 활성화
        if (defaultWeapon != null)
        {
            defaultWeapon.SetActive(true);
            currentWeapon = defaultWeapon;
            Debug.Log("기본 무기로 변경");
        }
        else
        {
        }
    }

    /// <summary>
    /// 현재 장착된 무기 반환
    /// </summary>
    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
