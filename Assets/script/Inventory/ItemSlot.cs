using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] Image itemIcon;    //아이템 아이콘
    [SerializeField] Text itemCountText;//아이템 갯수 텍스트

    Item currentItem;  // 현재 슬롯에 있는 아이템 정보
    int slotIndex;     // 이 슬롯의 인덱스 번호

    #region 툴팁 관련
    // 마우스가 슬롯 위에 올라왔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(currentItem == null)
        {
            return;
        }
        ToolTip.instance.Show(currentItem, itemIcon.transform.parent.GetComponent<RectTransform>());
    }

    // 마우스가 슬롯에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.instance.Hide();
    }

    // 마우스 클릭 (좌클릭/우클릭)
    public void OnPointerClick(PointerEventData eventData)
    {
        if(currentItem == null) 
        {
            return;
        }

        // 우클릭
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ToolTip.instance.Hide();
            ToolTip_Fun.Instance.Show(currentItem, itemIcon.transform.parent.GetComponent<RectTransform>());
        }
    }
    #endregion

    #region 드래그 앤 드롭 관련
    // 드래그 시작할 때
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(currentItem == null)
        {
            return;
        }

        // TODO: 상세정보 툴팁 숨기기
        ToolTip.instance.Hide();

        // TODO: ToolTip_Drag에 드래그 시작 알림
        ToolTip_Drag.instance.StartDrag(currentItem, currentItem.itemIcon, itemIcon);
    }

    // 드래그 중일 때 (매 프레임)
    public void OnDrag(PointerEventData eventData)
    {
        // TODO: ToolTip_Drag에 위치 업데이트 전달
        ToolTip_Drag.instance.UpdateDrag(eventData.position);
    }

    // 드래그 끝났을 때 (마우스 놓았을 때)
    public void OnEndDrag(PointerEventData eventData)
    {
        // TODO: ToolTip_Drag에 드래그 종료 알림
        ToolTip_Drag.instance.EndDrag(eventData);
    }

    // 다른 슬롯에서 드래그한 아이템이 이 슬롯 위에 드롭되었을 때
    public void OnDrop(PointerEventData eventData)
    {
        // 자동정렬 인벤토리이므로 슬롯 간 이동은 처리하지 않음
        // 아무 동작 없이 원래 위치로 돌아감
    }
    #endregion

    public void SetItem(Item _item)
    {
        currentItem = _item;  // 아이템 정보 저장

        // 아이템 존재해서 알파값 1로 변경
        SetIconAlpha(1f);

        itemIcon.sprite = _item.itemIcon;
        if(_item.currentCount <= 1)
        {
            itemCountText.gameObject.SetActive(false);
        }
        else
        {
            itemCountText.gameObject.SetActive(true);
            itemCountText.text = _item.currentCount.ToString();
        }
    }

    public void Clear()
    {
        currentItem = null;  // 아이템 정보 초기화
        SetIconAlpha(0f);
        itemCountText.gameObject.SetActive(false);
    }

    void SetIconAlpha(float _alpha)
    {
        Color color = itemIcon.color;
        color.a = _alpha;
        itemIcon.color = color;
    }
}
