using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모든 상점의 기본 클래스
/// GameObject에 직접 붙이지 않고 상속용으로만 사용
/// </summary>
public abstract class BaseShop : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] protected Text playerGoldText;          // 플레이어 골드 표시
    [SerializeField] protected GameObject[] invenSlot; // 인벤토리 슬롯 프리팹
    protected List<Item> equipList;          // player가 가지고있는 장비아이템
    protected List<Item> consumeList;        // player가 가지고 있는 소비아이템     
    protected List<Item> materialList;       // player가 가지고 있는 기타아이템
    int equipSlotIndex;     // 보유한 무기 카테고리의 갯수 
    int consumeSlotIndex;   // 보유한 소비 카테고리의 갯수 
    int materialSlotIndex;  // 보유한 기타 카테고리의 갯수 
    int slotMaxIndex;       // 최대 슬롯 갯수
    bool isEquipTab = false;
    bool isConsumeTab = false;
    bool isMaterialTab = false;
    [Header("버튼관련")]
    [SerializeField] Image equipButton;
    [SerializeField] Image consumeButton;
    [SerializeField] Image materialButton;
    [SerializeField] Sprite buttonNormal;
    [SerializeField] Sprite buttonSelected;
  

    void Awake()
    {
        CreatedShopItems();
    }
    void OnEnable()
    {
        UI.Instance.isShopOpen = true;
        UI_Inventory.inven.goldChaged += UpdatePlayerGoldText;
        LoadShopItems();
        EquipButtonClick(); // 상점탭이 열렸을경우 처음에 보이는것은 장비쪽이 먼저 보임
    }

    void OnDisable()
    {
        UI.Instance.isShopOpen = false;
        UI_Inventory.inven.goldChaged -= UpdatePlayerGoldText;
        ClearShopItems();
    }

    // 상점탭이 켜질때
    protected virtual void LoadShopItems()
    {
        if (UI_Inventory.inven == null)
        {
            Debug.LogWarning("UI_Inventory.inven이 아직 초기화되지 않았습니다.");
            return;
        }

        equipList = UI_Inventory.inven.EQUIPLIST;
        consumeList = UI_Inventory.inven.CONSUMELIST;
        materialList = UI_Inventory.inven.MATERIALLIST;
        slotMaxIndex = UI_Inventory.inven.MAXINDEX;
        equipSlotIndex = equipList.Count;
        consumeSlotIndex = consumeList.Count;
        materialSlotIndex = materialList.Count;
        
        playerGoldText.text = UI_Inventory.inven.GOLD.ToString();
    }
    protected virtual void ClearShopItems()
    {
        equipList = null;
        consumeList = null;
        materialList = null;
        equipSlotIndex = 0;
        consumeSlotIndex = 0;
        materialSlotIndex = 0;
    }
    public void BuyItem(Item _item)
    {
        // 슬롯 초과 체크
        if(!UI_Inventory.inven.CanAddItem(_item))
        {
            // 슬롯이 가득 찼습니다 알림
            Debug.Log("슬롯이 가득 찼습니다!");
            return;
        }

        if(UI_Inventory.inven.GOLD >= _item.price)
        {
            UI_Inventory.inven.GOLD -= _item.price;
            UI_Inventory.inven.AddItemInventory(_item);
            BuyRefresh();
        }
        else
        {
            // 골드가 부족합니다 알림
            Debug.Log("골드가 부족합니다!");
        }
    }
    // 구매를했을경우 내가 소비카테고리를 켜놓은상태에서 소비아이템을 살경우 Refresh 즉 
    // consumeList정보를 불러와 invenSlot을 켜주거나 아니면 존재할경우 카운트만 늘려주고 100개이상일시 새로운 슬롯 켜주는형식
    // 그리고 UI_Inven 거기에서 현재 슬롯갯수를 초과하면 구매 불가능하게 막기
    void BuyRefresh()
    {
        LoadShopItems();

        // 먼저 모든 슬롯 끄기
        for(int i = 0; i < slotMaxIndex; i++)
        {
            invenSlot[i].gameObject.SetActive(false);
        }

        // 현재 탭에 맞는 슬롯만 켜기
        if(isConsumeTab)
        {
            for(int i = 0; i < consumeList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, consumeList[i]);
            }
        }
        else if(isEquipTab)
        {
            for(int i = 0; i < equipList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, equipList[i]);
            }
        }
        else if(isMaterialTab)
        {
            for(int i = 0; i < materialList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, materialList[i]);
            }
        }
    }
    public void SellRefresh()
    {
        BuyRefresh();
    }

    // 처음 시작시 상점슬롯 생성하는 코드
    // 소비상점은 미리 씬에 생성시키고 하는거라 안쓰고
    // 장비상점은 미리 씬에 없어서 이코드로 생성시킴
    // 장비상점스크립트에는 Awake를 만들지 않았기때문에
    protected virtual void CreatedShopItems()
    {
        
    }

    // 카테고리에서 원하는거 눌럿을때 상점에서 player인벤쪽 카테고리 보여주는거
    public virtual void EquipButtonClick()
    {
        isEquipTab = true;
        isConsumeTab = false;
        isMaterialTab = false;
        ButtonSpriteCheck();
  

        // 먼저 모든 슬롯 끄기
        for(int i = 0; i < slotMaxIndex; i++)
        {
            invenSlot[i].gameObject.SetActive(false);
        }

        // 필요한 슬롯만 켜기
        if(equipList != null)
        {
            for(int i = 0; i < equipList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, equipList[i]);
            }
        }
    }
    public virtual void ConsumeButtonClick()
    {
        isEquipTab = false;
        isConsumeTab = true;
        isMaterialTab = false;
        ButtonSpriteCheck();
        // 먼저 모든 슬롯 끄기
        for(int i = 0; i < slotMaxIndex; i++)
        {
            invenSlot[i].gameObject.SetActive(false);
        }

        // 필요한 슬롯만 켜기
        if(consumeList != null)
        {
            for(int i = 0; i < consumeList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, consumeList[i]);
            }
        }
    }
    public virtual void MaterialButtonClick()
    {
        isEquipTab = false;
        isConsumeTab = false;
        isMaterialTab = true;
        ButtonSpriteCheck();
        // 먼저 모든 슬롯 끄기
        for(int i = 0; i < slotMaxIndex; i++)
        {
            invenSlot[i].gameObject.SetActive(false);
        }

        // 필요한 슬롯만 켜기
        if(materialList != null)
        {
            for(int i = 0; i < materialList.Count; i++)
            {
                invenSlot[i].gameObject.SetActive(true);
                Shop_Inven_Slot slot = invenSlot[i].GetComponent<Shop_Inven_Slot>();
                slot.SetItem(this, materialList[i]);
            }
        }
    }

    // 카테고리버튼클릭했을때 어떤것이 눌렷나 체크해서
    // 그 카테고리 이미지는 눌린거 아닌것은 기본버튼이미지로 변경하는함수
    void ButtonSpriteCheck()
    {
        equipButton.sprite = isEquipTab? buttonSelected : buttonNormal;
        consumeButton.sprite = isConsumeTab? buttonSelected : buttonNormal;
        materialButton. sprite= isMaterialTab? buttonSelected : buttonNormal;
    }
    void UpdatePlayerGoldText()
    {
        playerGoldText.text = UI_Inventory.inven.GOLD.ToString();
    }
}
