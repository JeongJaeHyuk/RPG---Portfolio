using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Qick_Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] Image iconImage;           // 아이템 이미지
    [SerializeField] Text itemCountText;        // 아이템 갯수를 알려줄 텍스트
    [SerializeField] PlayerSpecs playerSpecs;   // 플레이어 스펙 (소비 아이템 사용시 필요)

    int registeredItemID = -1;                  // 등록된 아이템 ID (-1은 빈 슬롯)
    Sprite registeredSprite;                    // 등록된 아이템 스프라이트
    int itemCount;                              // 인벤에 존재하는 동일한 아이템 총합산

    void Start()
    {
        // 나중에 player데이터 저장하고 불러오는 코드가 있을 시 이것도 저장하는 함수사용하고 iconImage가 null이면 사용하게하면될듯
        // 초기 알파값 0으로 설정
        iconImage.color = new Color(1,1,1,0);
        // 텍스트 오브젝트 비활성화
        itemCountText.gameObject.SetActive(false);
    }

    // Update 사용 안 함 (최적화: 필요할 때만 호출)

    // 퀵슬롯에 아이템 등록 함수
    public void RegisterItem(Item _item)
    {
        // TODO: 아이템 정보 저장
        registeredItemID = _item.itemId;
        registeredSprite = _item.itemIcon;

        // TODO: 아이콘 이미지 설정 및 활성화
        iconImage.sprite = registeredSprite;
        // iconImage 알파값 1로 설정 (현재 버그: 알파값이 0임, 1로 수정 필요)
        iconImage.color = new Color(1, 1, 1, 1);

        // TODO: 개수 텍스트 활성화
        // 아이템의 총갯수를 찾아서 itemCount에 저장
        itemCount = UI_Inventory.inven.FindItemIDAllCount(_item.itemId);
        // 아이템 갯수를 알려줄 텍스트 오브젝트 활성화
        itemCountText.gameObject.SetActive(true);
        // 아이템 총갯수를 text에 보내줌
        itemCountText.text = itemCount.ToString();
    }

    // 퀵슬롯 비우기 함수
    public void ClearSlot()
    {
        // TODO: 등록 정보 초기화
        registeredItemID = -1;
        registeredSprite = null;

        // TODO: UI 초기화
        iconImage.sprite = null;
        // iconImage 알파값 0으로 설정
        iconImage.color = new Color (1,1,1,0);
        // itemCountText 비활성화
        itemCountText.gameObject.SetActive(false);
    }

    // 퀵슬롯 사용 함수 (키 입력이나 클릭시 호출)
    public void UseQuickSlot()
    {
        // TODO: 등록된 아이템이 없으면 리턴
        // registeredItemID가 -1이면 return
        if(registeredItemID == -1)
        {
            return;
        }
        // TODO: 인벤토리에서 해당 아이템 찾기 (마지막 슬롯부터)
        Item targetItem = UI_Inventory.inven.FindLastItemByID(registeredItemID);

        // TODO: 아이템이 없으면 (null이면) 퀵슬롯 비우기
        if(targetItem == null)
        {
            ClearSlot();
            return;
        }        
        // TODO: 소비 아이템이면 사용 처리
        if(targetItem.type == ItemType.Consumable)
        {          
            // 체력 또는 마나 회복
            playerSpecs.CURRENT_HP += targetItem.hpRecover;
            playerSpecs.CURRENT_MP += targetItem.mpRecover;
            targetItem.currentCount--;
            // 아이템 개수가 0 이하면 인벤토리에서 제거
            if(targetItem.currentCount <= 0)
            {
                UI_Inventory.inven.RemoveItem(targetItem);
                // 위에 함수에 UI_Inventory.inven.Refresh(); 이 코드가 존재함
            }
            else
            {
                // 인벤토리 UI 갱신 
                UI_Inventory.inven.Refresh();
            }

            // 아이템 총갯수 갱신
            UpdateItemCount();
        }
    }

    // 아이템 개수 갱신 함수 (필요할 때만 호출 - 최적화)
    void UpdateItemCount()
    {
        // TODO: 등록된 아이템이 없으면 리턴
        if(registeredItemID == -1)
        { 
            return;
        }

        // TODO: 인벤토리에서 해당 아이템 총 개수 가져오기
        itemCount = UI_Inventory.inven.FindItemIDAllCount(registeredItemID);

        // TODO: 개수가 0이면 퀵슬롯 비우기
        if(itemCount <= 0)
        {
            ClearSlot();
        }


        // TODO: 개수가 남아있으면 텍스트 업데이트
        else
        {
            itemCountText.text = itemCount.ToString();
        }
    }

    // 드래그한 아이템이 이 퀵슬롯에 드롭되었을 때
    public void OnDrop(PointerEventData eventData)
    {
        // TODO: ToolTip_Drag에서 현재 드래그 중인 아이템 정보 가져오기
        // 현재 드래그중인 아이템 정보 받기
        Item draggedItem = ToolTip_Drag.instance.GetDraggedItem();

        // TODO: 드래그된 아이템이 소비 아이템인 경우만 등록
        if(draggedItem != null && draggedItem.type == ItemType.Consumable)
        {
            RegisterItem(draggedItem);
        }
    }
}
