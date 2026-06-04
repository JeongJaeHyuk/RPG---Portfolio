using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Weapon,         // 무기
    Armor,          // 방어구
    Consumable,     // 소비
    Material        // 기타
}

public enum ArmorType
{
    None,           // 방어구가 아닐 경우
    Head,           // 머리 방어구
    Chest           // 가슴 방어구
}

[System.Serializable]
public class Item
{
    public int itemId;           // 아이템 id 비교하고나 찾을떄 id값으로 빠르게 찾기 위해 사용
    public string itemName;      // 아이템 이름
    public string itemTypeName;  // 아이템 영문명
    public Sprite itemIcon;      // 아이템 아이콘
    public string description;   // 아이템 설명
    public ItemType type;        // 아이템 타입 장착 소비 재료
    public ArmorType armorType;  // 방어구 타입 (방어구일 경우 부위 구분)
    public float attackPower;    // 공격력(무기일경우 필요)
    public float defense;        // 방어력(방어구일경우 필요)
    public float hpRecover;      // 체력회복량(포션일 경우)
    public float mpRecover;      // 마나회복량(포션일 경우)
    public int price;            // 아이템 가격 (상점에서 판매/구매 가격)
    public int currentCount;     // 현재 보유 개수
    public int maxCount;         // 최대 스택 가능 개수

    // 기본 생성자
    public Item() { }

    // 복사 생성자 (구매 시 원본 복사용)
    public Item(Item other)
    {
        this.itemId = other.itemId;
        this.itemName = other.itemName;
        this.itemTypeName = other.itemTypeName;
        this.itemIcon = other.itemIcon;
        this.description = other.description;
        this.type = other.type;
        this.armorType = other.armorType;
        this.attackPower = other.attackPower;
        this.defense = other.defense;
        this.hpRecover = other.hpRecover;
        this.mpRecover = other.mpRecover;
        this.price = other.price;
        this.maxCount = other.maxCount;
        this.currentCount = 1;  // 새로 구매하면 1개
    }
}
   
