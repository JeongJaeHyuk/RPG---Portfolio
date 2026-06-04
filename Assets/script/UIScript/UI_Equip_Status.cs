using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Equip_Status : MonoBehaviour
{
    // 여기가 장비 장착 관련 스크립트
    public static UI_Equip_Status instance = null;

    [SerializeField] PlayerSpecs plsp;          // 플레이어 스펙 스크립트

    [SerializeField] Equip_Slot weaponSlot;     // 무기 슬롯
    [SerializeField] Equip_Slot headSlot;       // 머리 방어구 슬롯
    [SerializeField] Equip_Slot chestSlot;      // 가슴 방어구 슬롯

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 무기를 장착하는 함수
    /// </summary>
    /// <param name="_item">장착할 무기 아이템</param>
    public void EquipWeapon(Item _item)
    {
        // 1. weaponSlot.EquipItem(_item) 호출 (슬롯이 알아서 기존 아이템 처리)
        weaponSlot.EquipItem(_item);
        // 2. PlayerSpecs에 무기 공격력 적용
        plsp.WAPONDAMAGE = 0f;
        plsp.WAPONDAMAGE = _item.attackPower;   // 이건 값타입이라 밑에 Remove해도 문제없음
        // 3. 플레이어 무기 오브젝트 교체
        PlayerWeaponManager.instance.ChangeWeapon(_item.itemIcon.name);
        // 4. 인벤토리에서 해당 아이템 제거 (UI_Inventory.inven.RemoveItem)
        UI_Inventory.inven.RemoveItem(_item);
    }

    /// <summary>
    /// 방어구를 장착하는 함수 (ArmorType에 따라 headSlot 또는 chestSlot에 장착)
    /// </summary>
    /// <param name="_item">장착할 방어구 아이템</param>
    public void EquipArmor(Item _item)
    {
        // 1. _item.armorType을 확인하여 Head인지 Chest인지 판단;
        switch(_item.armorType)
        {
            // 2. ArmorType.Head인 경우: headSlot.EquipItem(_item) 호출 (슬롯이 알아서 기존 아이템 처리)
            case ArmorType.Head:
            headSlot.EquipItem(_item);
            // 3. 방어구 적용
            plsp.HEADEFENSE = 0f;
            plsp.HEADEFENSE = _item.defense;
                break;
            // 4. ArmorType.Chest인 경우: chestSlot.EquipItem(_item) 호출 (슬롯이 알아서 기존 아이템 처리)
            case ArmorType.Chest:
            chestSlot.EquipItem(_item);
            // 5. 방어구 적용
            plsp.CHESTDEFENSE = 0f;
            plsp.CHESTDEFENSE = _item.defense;
                break;
        }
        // 6. 인벤토리에서 해당 아이템 제거 (UI_Inventory.inven.RemoveItem)
        UI_Inventory.inven.RemoveItem(_item);
    }

    /// <summary>
    /// 무기 장착을 해제하고 인벤토리로 되돌리는 함수
    /// </summary>
    public void UnEquipWeapon()
    {
        // 1. weaponSlot.GetEquippedItem()으로 현재 장착된 무기 가져오기
        // 2. 무기가 null이 아니면:
        //    - 인벤토리에 추가 (UI_Inventory.inven.AddItemInventory)
        //    - weaponSlot.UnEquipItem() 호출하여 슬롯 비우기
        if(weaponSlot.GetEquippedItem() != null)
        {
            // 1. 인벤토리로 되돌리기 (UI_Inventory.inven.AddItemInventory) 인벤에서 Add할떄 new처리로 복사본 넣어서 new 키워드 여기서 불필요
            UI_Inventory.inven.AddItemInventory(weaponSlot.GetEquippedItem());
            weaponSlot.UnEquipItem();
            //    - PlayerSpecs의 무기 공격력 초기화 (0으로)
            plsp.WAPONDAMAGE = 0f;
            //    - 플레이어 무기를 기본 무기로 변경
            PlayerWeaponManager.instance.ResetToDefaultWeapon();
        }
    }

    /// <summary>
    /// 방어구 장착을 해제하고 인벤토리로 되돌리는 함수
    /// </summary>
    /// <param name="_item">해제할 방어구 아이템</param>
    public void UnEquipArmor(Item _item)
    {
        // 무기와 다르게 방어구는 두곳이기때문에 매개변수를 받음
        if (_item != null)
        {
            // 1. _item.armorType을 확인하여 Head인지 Chest인지 판단
            switch(_item.armorType)
            {
                case ArmorType.Head:
                    if(headSlot.GetEquippedItem() != null)
                    {
                        // 인벤토리로 되돌리기
                        UI_Inventory.inven.AddItemInventory(headSlot.GetEquippedItem());
                        // 슬롯에서 장착 해제
                        headSlot.UnEquipItem();
                        // 방어력 초기화
                        plsp.HEADEFENSE = 0f;
                    }
                    break;
                case ArmorType.Chest:
                    if(chestSlot.GetEquippedItem() != null)
                    {
                        // 인벤토리로 되돌리기
                        UI_Inventory.inven.AddItemInventory(chestSlot.GetEquippedItem());
                        // 슬롯에서 장착 해제
                        chestSlot.UnEquipItem();
                        // 방어력 초기화
                        plsp.CHESTDEFENSE = 0f;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 현재 장착된 무기를 반환하는 함수
    /// </summary>
    /// <returns>장착된 무기 아이템, 없으면 null</returns>
    public Item GetEquippedWeapon()
    {
        // weaponSlot.GetEquippedItem() 반환
        return null;
    }

    /// <summary>
    /// 현재 장착된 방어구를 반환하는 함수
    /// </summary>
    /// <param name="_armorType">확인할 방어구 타입</param>
    /// <returns>장착된 방어구 아이템, 없으면 null</returns>
    public Item GetEquippedArmor(ArmorType _armorType)
    {
        // 1. _armorType에 따라 headSlot 또는 chestSlot 선택
        // 2. 선택된 슬롯의 GetEquippedItem() 반환
        return null;
    }
}
