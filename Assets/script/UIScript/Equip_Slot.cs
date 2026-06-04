using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Equip_Slot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image itemIcon = null;
    Item currentEquipItem;

    /// <summary>
    /// 슬롯을 클릭했을 때 호출되는 함수
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
       if(currentEquipItem == null)
        {
            return;
        }

        // 우클릭
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ToolTip.instance.Hide();
            // 장착된 아이템이므로 isEquipped = true 전달
            ToolTip_Fun.instance.Show(currentEquipItem, itemIcon.transform.parent.GetComponent<RectTransform>(), true);
        }
    }

    /// <summary>
    /// 아이템을 슬롯에 장착하는 함수
    /// </summary>
    /// <param name="_item">장착할 아이템</param>
    public void EquipItem(Item _item)
    {
        // 1. 이미 장착된 아이템이 있는지 확인 (currentEquipItem != null)
        if (currentEquipItem != null)
        {
            UI_Inventory.inven.AddItemInventory(currentEquipItem);
            UnEquipItem();
        }
        currentEquipItem = new Item(_item);
        itemIcon.sprite = _item.itemIcon;
        itemIcon.enabled = true;
    }

    /// <summary>
    /// 슬롯에서 아이템 장착을 해제하는 함수
    /// </summary>
    public void UnEquipItem()
    {
        if(currentEquipItem != null)
        {
            // 2. currentEquipItem을 null로 설정
            currentEquipItem = null;
            // 3. itemIcon.sprite를 null로 설정
            itemIcon.sprite = null;
            // 4. itemIcon.enabled = false로 설정하여 아이콘 숨기기
            itemIcon.enabled = false; 
        }
    }
    public Item GetEquippedItem()
    {
            return currentEquipItem;
    }
    void Start()
    {
        // itemIcon 초기화 (시작할 때는 아이콘 숨기기)
        if(itemIcon != null)
        {
            itemIcon.enabled = false;
        }
    }
}
