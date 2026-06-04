using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비 상점 - 20개 동적 생성 방식
/// EquipTab GameObject에 붙임
/// </summary>
public class Equip_Shop : BaseShop
{
    [Header("Dynamic Slot Generation")]
    [SerializeField] GameObject slotPrefab;     // 생성 슬롯 프리팹 
    [SerializeField] Transform slotParent;      // 슬롯이 생성될 부모 (Grid Layout Group)

    [Header("게임 시작 이후 사용할 변수들")]
    [SerializeField] List<ShopSlot> shopSlots;

    List<Item> shopEquipList => Item_Manager.itemManager.GetEquipList();

    /// <summary>
    /// 장비 아이템 데이터 로드 및 슬롯 동적 생성
    /// </summary>
    protected override void CreatedShopItems()
    {
        // 아이템매니저에서 아이템종류만큼 슬롯생성해주는 반복문
        for (int i = 0; i < shopEquipList.Count; i++)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slot.name = slotPrefab.name + "_" + (i+1);
            shopSlots.Add(slot.GetComponent<ShopSlot>());
        }
        LoadShopItems();
    }
    protected override void LoadShopItems()
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            Item item = shopEquipList[i];
            shopSlots[i].SetItem(this, item);
        }
        base.LoadShopItems(); // 부모의 플레이어 인벤토리 로드
    }


    protected override void ClearShopItems()
    {
        foreach (var slot in shopSlots)
        {
            slot.Clear();
        }
    }
}
