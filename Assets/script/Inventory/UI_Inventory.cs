using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory inven = null;
    [SerializeField] GameObject content;
    [SerializeField] List<Item> equipList;          // player가 가지고있는 장비아이템
    [SerializeField] List<Item> consumeList;        // player가 가지고 있는 소비아이템     
    [SerializeField] List<Item> materialList;       // player가 가지고 있는 기타아이템

    GameObject[] itemSlot;
    int equipMaxSlot = 6;   // 무기 카테고리 최대 슬롯 개수
    int consumeMaxSlot = 12;  // 소비 카테고리 최대 슬롯 개수
    int materialMaxSlot = 18; // 기타 카테고리 최대 슬롯 개수

    [Header ("버튼관련")]
    [SerializeField] Image equipButton;         // 장비버튼 이미지
    [SerializeField] Image consumeButton;       // 소비버튼 이미지
    [SerializeField] Image materialButton;      // 기타버튼 이미지
    [SerializeField] Sprite buttonNormal;       // 버튼 기본 이미지
    [SerializeField] Sprite buttonSelected;      // 버튼 클릭된 이미지


    int maxIndex = 30;

#region 읽기전용 프로퍼티
    // 읽기전용 프로퍼티
    public List<Item> EQUIPLIST => equipList;
    public List<Item> CONSUMELIST => consumeList;
    public List<Item> MATERIALLIST => materialList;
    public int MAXINDEX => maxIndex;
    
#endregion
    public event Action goldChaged;
    [SerializeField] Text goldtext;
    int gold = 0;

    bool isEquip = false;
    bool isConsume = false;
    bool isMaterial = false;
    public int GOLD
    {
        get => gold;
        set
        {
            gold = value;
            goldChaged?.Invoke();
        }
    }
    void Awake()
    {
        if(inven == null)
        {
            inven = this;
        }
        else
        {
            Destroy(gameObject);
        }
        itemSlot = new GameObject[content.transform.childCount];
        for (int i = 0; i < content.transform.childCount; i++)
        {
            itemSlot[i] = content.transform.GetChild(i).gameObject;
            if(i >= 6)
            {
                itemSlot[i].gameObject.SetActive(false);
            }
        }
        goldChaged += GoldText;
        gameObject.SetActive(false); // 반드시 인벤오브젝트 켜놔야함
    }
    void Start()
    {
        
    }

    void OnEnable()
    {
        UI.instance.isInvenOpen = true;
        // 아무 카테고리도 선택 안됐으면 기본값 설정
        if(!isEquip && !isConsume && !isMaterial)
        {
            isEquip = true;  // 기본: 장비 카테고리
        }
        Refresh();

        // 인벤토리 열릴 때 DropBlocker 활성화
        if(DropBlocker.instance != null)
        {
            DropBlocker.instance.gameObject.SetActive(true);
        }
    }
    void OnDisable()
    {
        UI.instance.isInvenOpen = false;
        ToolTip.instance.Close();

        // 인벤토리 닫힐 때 DropBlocker 비활성화
        if(DropBlocker.instance != null)
        {
            DropBlocker.instance.gameObject.SetActive(false);
        }
    }

    // 인벤토리 골드 구독 함수
    void GoldText()
    {
        goldtext.text = GOLD.ToString();
    }
    #region 버튼클릭함수들

    //무기카테고리 클릭함수
    public void WeaponSlotClick()
    {
        isEquip = true;
        isConsume = false;
        isMaterial = false;
        Refresh();
    }
    // 소비카테고리 클릭함수
    public void ConsumeSlotClick()
    {
        isEquip = false;
        isConsume = true;
        isMaterial = false;
        Refresh();
    }
    // 기타카테고리 클릭함수
    public void MaterialSlotClick()
    {
        isEquip = false;
        isConsume = false;
        isMaterial = true;
        Refresh();
    }
    #endregion
    // 인벤토리에 저장하는 함수
    public void AddItemInventory(Item _item)
    {
        switch(_item.type)
        {
            case ItemType.Weapon:
            Item newWeapon = new Item(_item);
            equipList.Add(newWeapon);
            break;
            case ItemType.Armor:
            Item newArmor = new Item(_item);
            equipList.Add(newArmor);
            break;
            case ItemType.Consumable:
            AddConsume(_item);
            break;
            case ItemType.Material:
            AddMaterial(_item);
            break;
        }
        // 아이템 수집 퀘스트있을경우
        UI_Quest.instance.CollectItemQuest(_item.itemTypeName,_item.currentCount);

        // 인벤토리가 열려있으면 UI 업데이트
        if(gameObject.activeSelf)
        {
            Refresh();
        }
    }
    // UI정보 업데이트 뭐하나 바뀔마다 이 함수 불러야함
    public void Refresh()
    {
        // 모든 오브젝트 끄기
        for(int i = 0; i < MAXINDEX;i++)
        {
            itemSlot[i].gameObject.SetActive(false);
        }
        if(isEquip)
        {
            // 장비 슬롯 최대치 만큼 게임오브젝트 키기
            for(int i = 0; i < equipMaxSlot;i++)
            {
                itemSlot[i].gameObject.SetActive(true);
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.Clear();
            }
            // 내가 보유한 장비 리스트를 게임오브젝트에 대입시키기
            for(int i = 0; i < equipList.Count; i++)
            {
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.SetItem(equipList[i]);
            }
        }
        else if(isConsume)
        {
            // 장비 슬롯 최대치 만큼 게임오브젝트 키기
            for(int i = 0; i < consumeMaxSlot;i++)
            {
                itemSlot[i].gameObject.SetActive(true);
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.Clear();
            }
            // 내가 보유한 장비 리스트를 게임오브젝트에 대입시키기
            for(int i = 0; i < consumeList.Count; i++)
            {
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.SetItem(consumeList[i]);
            }
        }
        else if(isMaterial)
        {   
            // 장비 슬롯 최대치 만큼 게임오브젝트 키기
            for(int i = 0; i < materialMaxSlot;i++)
            {
                itemSlot[i].gameObject.SetActive(true);
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.Clear();
            }
            // 내가 보유한 장비 리스트를 게임오브젝트에 대입시키기
            for(int i = 0; i < materialList.Count; i++)
            {
                ItemSlot slot = itemSlot[i].GetComponent<ItemSlot>();
                slot.SetItem(materialList[i]);
            }
        }
        // 어떤 카테고리가 선택되었다는 것에 따라 버튼이미지 설정
        equipButton.sprite = isEquip ? buttonSelected :buttonNormal;
        consumeButton.sprite = isConsume ? buttonSelected :buttonNormal;
        materialButton.sprite = isMaterial ? buttonSelected :buttonNormal;
    }

    // 소비아이템 추가
    void AddConsume(Item _item)
    {
        Item existing = consumeList.Find(x => x.itemId == _item.itemId && x.currentCount < x.maxCount);
        if(existing != null)
        {
            existing.currentCount++;
        }
        else
        {
            Item newItem = new Item(_item);
            consumeList.Add(newItem);
        }
    }

    // 기타아이템 추가
    void AddMaterial(Item _item)
    {
        Item existing = materialList.Find(x => x.itemId == _item.itemId && x.currentCount < x.maxCount);
        if(existing != null)
        {
            existing.currentCount++;
        }
        else
        {
            Item newItem = new Item(_item);
            materialList.Add(newItem);
        }
    }

    // 아이템 추가 가능 여부 체크 (슬롯 초과 체크)
    public bool CanAddItem(Item _item)
    {
        switch(_item.type)
        {
            // 무기와 방어구는 같은 리스트를 사용해서 이렇게 구현함
            case ItemType.Weapon:
            case ItemType.Armor:
                return equipList.Count < equipMaxSlot;

            case ItemType.Consumable:
                // 공간이 있는 같은 아이템이 있으면 OK
                Item existing = consumeList.Find(x => x.itemId == _item.itemId && x.currentCount < x.maxCount);
                if(existing != null)
                    return true;
                // 새 슬롯 필요
                return consumeList.Count < consumeMaxSlot;

            case ItemType.Material:
                // 공간이 있는 같은 아이템이 있으면 OK
                Item existingMat = materialList.Find(x => x.itemId == _item.itemId && x.currentCount < x.maxCount);
                if(existingMat != null)
                    return true;
                // 새 슬롯 필요
                return materialList.Count < materialMaxSlot;
        }
        return false;
    }
    // 아이템 삭제 함수
    // 판매나 아이템 사용후 그슬롯의 아이템갯수가 0이 될경우 실행시킬 함수
    public void RemoveItem(Item _item)
    {
        switch(_item.type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
                equipList.Remove(_item);
                break;
            case ItemType.Consumable:
                consumeList.Remove(_item);
                break;
            case ItemType.Material:
                materialList.Remove(_item);
                break;
        }

        // 인벤토리가 열려있으면 UI 업데이트
        if(gameObject.activeSelf)
        {
            Refresh();
        }
    }

    // 아이템 ID로 마지막 슬롯부터 검색 (부분 스택 우선 소비용)
    // 퀵슬롯에서 아이템 사용시 호출할 함수
    public Item FindLastItemByID(int _itemID)
    {
        // 소비 아이템 리스트에서 뒤에서부터 역순으로 검색
        for(int i = consumeList.Count - 1; i >= 0; i--)
        {
            // consumeList를 뒤에서부터 순회하며 _itemID와 일치하는 첫번째 아이템을 찾아서 반환
            if(_itemID == consumeList[i].itemId)
            {
                return consumeList[i];
            }
        }
        // 못 찾으면 null 반환
        return null;
    }
    // 같은 아이템의 갯수를 전부 반환하는 함수
    public int FindItemIDAllCount(int _itemID)
    {
        int totalCount = 0;
        // 소비아이템의 모든아이템을 검색
        foreach(var item in consumeList)
        {
            // id가 같은 아이템을 찾아서 합산
            if(_itemID == item.itemId)
            {
                // 해당 슬롯의 currentCount를 모두 합산
                totalCount += item.currentCount;
            }
        }
        return totalCount;
    }

    // 아이템 타입 이름으로 인벤토리에서 아이템 개수 조회 (수집 퀘스트용)
    // _itemTypeName: 퀘스트에서 사용하는 영문 식별자 (예: "Rock_Fragment")
    public int GetItemCount(string _itemTypeName)
    {
        int totalCount = 0;

        // 장비 리스트 검색
        foreach(Item item in equipList)
        {
            if(item.itemTypeName == _itemTypeName)
            {
                totalCount += item.currentCount;
            }
        }

        // 소비 아이템 리스트 검색
        foreach(Item item in consumeList)
        {
            if(item.itemTypeName == _itemTypeName)
            {
                totalCount += item.currentCount;
            }
        }

        // 기타 아이템 리스트 검색
        foreach(Item item in materialList)
        {
            if(item.itemTypeName == _itemTypeName)
            {
                totalCount += item.currentCount;
            }
        }

        return totalCount;
    }

    // 아이템 타입 이름과 개수로 인벤토리에서 아이템 제거 (수집 퀘스트 완료용)
    // _itemTypeName: 제거할 아이템 타입 이름 (영문 식별자, 예: "Rock_Fragment")
    // _count: 제거할 개수
    public void RemoveItem(string _itemTypeName, int _count)
    {
        // 제거해야 할 남은 개수 추적
        int remainingCount = _count;

        // 장비 리스트에서 제거
        // 역순으로 순회 (Remove 시 인덱스 변화 방지)
        for(int i = equipList.Count - 1; i >= 0 && remainingCount > 0; i--)
        {
            // 아이템 타입 이름이 일치하는지 확인
            if(equipList[i].itemTypeName == _itemTypeName)
            {
                // 현재 슬롯의 개수가 남은 제거 개수보다 작거나 같으면
                if(equipList[i].currentCount <= remainingCount)
                {
                    // 이 슬롯의 모든 아이템 개수만큼 차감
                    remainingCount -= equipList[i].currentCount;
                    // 슬롯 전체를 리스트에서 제거 (기존 RemoveItem 활용)
                    RemoveItem(equipList[i]);
                }
                else
                {
                    // 현재 슬롯의 개수가 더 많으면 일부만 차감
                    equipList[i].currentCount -= remainingCount;
                    remainingCount = 0;
                }
            }
        }

        // 소비 아이템 리스트에서 제거
        for(int i = consumeList.Count - 1; i >= 0 && remainingCount > 0; i--)
        {
            if(consumeList[i].itemTypeName == _itemTypeName)
            {
                if(consumeList[i].currentCount <= remainingCount)
                {
                    remainingCount -= consumeList[i].currentCount;
                    RemoveItem(consumeList[i]);
                }
                else
                {
                    consumeList[i].currentCount -= remainingCount;
                    remainingCount = 0;
                }
            }
        }

        // 기타 아이템 리스트에서 제거
        for(int i = materialList.Count - 1; i >= 0 && remainingCount > 0; i--)
        {
            if(materialList[i].itemTypeName == _itemTypeName)
            {
                if(materialList[i].currentCount <= remainingCount)
                {
                    remainingCount -= materialList[i].currentCount;
                    RemoveItem(materialList[i]);
                }
                else
                {
                    materialList[i].currentCount -= remainingCount;
                    remainingCount = 0;
                }
            }
        }

        // 인벤토리 UI가 열려있으면 갱신
        if(gameObject.activeSelf)
        {
            Refresh();
        }

        // 제거 후에도 남은 개수가 있다면 경고 (아이템이 부족했음)
        if(remainingCount > 0)
        {
            Debug.LogWarning($"{_itemTypeName} 아이템이 {remainingCount}개 부족합니다. 완전히 제거되지 않았습니다.");
        }
    }
}
