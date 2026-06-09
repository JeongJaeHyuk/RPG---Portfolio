using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour
{
    public static Item_Manager Instance { get; private set; } = null;
    [SerializeField] Item_Data_Manager itemDB;
    [SerializeField] List<Item> itemList;


    List<Item> weaponList = new List<Item>();       // 무기
    List<Item> armorList = new List<Item>();        // 방어구
    List<Item> consumeList = new List<Item>();      // 소비
    public List<Item> materialList = new List<Item>();     // 기타


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject); // 중복생성방지
        }
        else
        {
            Instance = this;
        }
        LoadAndCategoryItems();
    }

    // Item_Data_Base스크립트에서 아이템 모든 정보를 불러오고
    // 거기에 ItemType에 따라 맞는 리스트에 add해주는함수

    void LoadAndCategoryItems()
    {
        itemList = itemDB.Item_Load_CSV();
        foreach(var item in itemList)
        {
            switch(item.type)
            {
                case ItemType.Weapon:
                Debug.Log("무기 들감");
                    weaponList.Add(item);
                    break;
                case ItemType.Armor:
                Debug.Log("방어구 들감");
                    armorList.Add(item);
                    break;
                case ItemType.Consumable:
                    consumeList.Add(item);
                    break;
                case ItemType.Material:
                    materialList.Add(item);
                    break;
            }
        }
    }

    // 상점에 보내줄 리스트 함수
    public List<Item> GetConsumeList()
    {
        return consumeList;
    }
    public List<Item> GetEquipList()
    {
        List<Item> equipList = new List<Item>();
        equipList.AddRange(weaponList);
        equipList.AddRange(armorList);
        return equipList;
    }
    public Item GetMaterialItem(int _itemId)
    {
        foreach(Item itemData in materialList)
        {
            if(itemData.itemId == _itemId)
            {
                return itemData;
            }
        }
        return null;
    }

    // 모든 타입에서 itemId로 원본 아이템 정보 반환 (세이브 로드 시 사용)
    public Item GetItemById(int _itemId)
    {
        return itemList.Find(x => x.itemId == _itemId);
    }
}
