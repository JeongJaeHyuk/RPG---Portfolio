using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    // 드랍된 아이템 정보
    private Item droppedItem;

    // DropData를 받아서 Item 정보 설정
    public void SetDropData(DropData dropData)
    {
        // TODO: Item_Manager에서 itemId로 아이템 찾기
        // Item_Manager에서 드랍할 아이템 id값을 비교해서 맞는아이템을 찾기
        Item originalItem = Item_Manager.Instance.GetMaterialItem(dropData.itemId);
        // 찾지못했을 경우
        if(originalItem == null)
        {
            Debug.LogError("아이템을 찾기 못했습니다");
            return;
        }
        // 찾은 아이템을 droppedItem변수에 복사본 생성
        droppedItem = new Item(originalItem);
        // 드랍할 아이템의 갯수 설정 
        droppedItem.currentCount = dropData.dropCount;
    }

    // 플레이어가 전리품 주머니에 닿았을 때 처리
    void OnTriggerEnter(Collider other)
    {
        // 플레이어인지 확인
        if(other.CompareTag("Player"))
        {
            if(droppedItem != null)
            {
                // 인벤토리에 흭득처리
                UI_Inventory.inven.AddItemInventory(droppedItem);
                // 오브젝트 풀에 반환
                LootBagPool.instance.ReturnLootBag(this);
            }
        }
    }

    // 오브젝트 풀로 반환될 때 초기화
    public void Clear()
    {
        // TODO: 초기화 처리
        // 아이템 정보 제거
        droppedItem = null;
        // 2. gameObject.SetActive(false); 로 비활성화
        gameObject.SetActive(false);
    }
}
