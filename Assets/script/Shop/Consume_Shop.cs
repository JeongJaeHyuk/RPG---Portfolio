using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소비 상점 - 4개 고정 슬롯 방식
/// ConsumeTab GameObject에 붙임
/// </summary>
public class Consume_Shop : BaseShop
{
    [Header("Shop Slots (4개 미리 배치)")]
    [SerializeField] ShopSlot[] shopSlots;  // Inspector에서 4개 연결

    List<Item> shopConsumeList => Item_Manager.itemManager.GetConsumeList();    // 상점 정보 불러올 변수

    protected override void LoadShopItems()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            Item item = shopConsumeList[i];
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
        base.ClearShopItems();
    }
}
