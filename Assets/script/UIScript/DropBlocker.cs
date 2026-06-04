using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 드롭 영역 감지 스크립트 (버리기 처리용)
// 전체 화면에 알파값 0인 Image를 깔고 이 스크립트를 붙임
// 인벤토리/퀵슬롯 등 자식 UI들이 Raycast를 차단하므로
// 이 Blocker에 도달하는 드롭 = 버리기
public class DropBlocker : MonoBehaviour, IDropHandler
{
    public static DropBlocker instance = null;

    [SerializeField] ToolTip_Check toolTip_Check;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // 시작 시 비활성화 (인벤토리 열릴 때만 활성화)
            gameObject.SetActive(false);
        }
    }

    // 아이템이 이 Blocker 위에 드롭되었을 때
    public void OnDrop(PointerEventData eventData)
    {
        // ToolTip_Drag에서 현재 드래그 중인 아이템 가져오기
        Item draggedItem = ToolTip_Drag.instance.GetDraggedItem();

        // 아이템이 있으면 버리기 확인 UI 표시
        if(draggedItem != null)
        {
            toolTip_Check.SetItem(draggedItem);
        }
    }
}
