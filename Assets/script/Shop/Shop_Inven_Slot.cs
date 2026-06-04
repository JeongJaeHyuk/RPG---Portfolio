using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop_Inven_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image itemIcon;           //아이템 아이콘

    [SerializeField] Text itemCount;
    [SerializeField] Text itemNameText;
    [SerializeField] Text saleText;
    
    BaseShop baseShop;
    Item currentItem;

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(currentItem == null)
        {
            return;
        }
        else
        {
            ToolTip.instance.Show(currentItem, itemIcon.transform.parent.GetComponent<RectTransform>());
        }        
    }

    // 마우스가 슬롯에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.instance.Hide();
    }
    public void SetItem(BaseShop _shop ,Item _item)
    {
        baseShop = _shop;
        currentItem = _item;
        itemIcon.sprite = _item.itemIcon;
        if(_item.currentCount <= 1)
        {
            itemCount.gameObject.SetActive(false);
        }
        else
        {
            itemCount.gameObject.SetActive(true);
            itemCount.text = _item.currentCount.ToString();
        }
        itemNameText.text = _item.itemName;
        saleText.text = _item.price * 0.6 + "Gold";
        
    }   
    public void Clear()
    {
        baseShop = null;
        currentItem = null;
    }


    public void SaleOnClick()
    {
        // 이미 판매 완료된 아이템이거나 없는 경우 무시
        if(currentItem == null || currentItem.currentCount <= 0)
        {
            return;
        }

        // 판매할 아이템을 로컬 변수에 저장하고 즉시 참조 해제 (중복 클릭 방지)
        Item itemToSell = currentItem;
        currentItem = null;

        itemToSell.currentCount--;
        UI_Inventory.inven.GOLD += (int)(itemToSell.price * 0.6);

        if(itemToSell.currentCount <= 0)
        {
            itemToSell.currentCount = 0;
            UI_Inventory.inven.RemoveItem(itemToSell);
        }

        baseShop.SellRefresh();  // SetItem()이 호출되어 currentItem이 다시 설정됨
    }
}
