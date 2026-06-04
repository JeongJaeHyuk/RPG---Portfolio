using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image itemIcon;           //아이템 아이콘
    [SerializeField] Text itemNameText;
    [SerializeField] Text priceText;
    
    BaseShop baseShop;

    [SerializeField] Item currentItem;

    
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
        itemNameText.text = _item.itemName;
        priceText.text = _item.price + "Gold";
    }
       
    public void Clear()
    {
        baseShop = null;
        currentItem = null;
    }

    public void BuyOnClick()
    {
        baseShop.BuyItem(currentItem);
    }
}
