using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 아이템 드래그 앤 드롭 관리 스크립트 (싱글톤)
public class ToolTip_Drag : MonoBehaviour
{
    public static ToolTip_Drag instance = null;

    [SerializeField] GameObject dragImageObject;  // 미리 만들어둔 드래그 이미지 오브젝트
    [SerializeField] Image dragIcon;              // 드래그 이미지의 아이콘

    Item draggedItem;               // 현재 드래그 중인 아이템 정보
    Image originalIconImage;        // 원본 슬롯의 아이콘 Image (알파값 복구용)

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // 시작 시 비활성화
            dragImageObject.SetActive(false);
        }
    }

    // 드래그 시작
    public void StartDrag(Item _item, Sprite _iconSprite, Image _originalIcon)
    {
        // TODO: 드래그할 아이템 정보 저장
        draggedItem = _item;

        // TODO: 원본 아이콘 이미지 참조 저장 (나중에 알파값 복구용)
        originalIconImage = _originalIcon;

        // TODO: 드래그 이미지 활성화
        dragImageObject.SetActive(true);

        // TODO: 드래그 이미지의 sprite 설정
        dragIcon.sprite = _iconSprite;

        // TODO: 원본 슬롯 아이콘 반투명하게
        Color color = originalIconImage.color;
        color.a = 0.5f;
        originalIconImage.color = color;
    }

    // 드래그 중 (매 프레임 위치 업데이트)
    public void UpdateDrag(Vector3 mousePosition)
    {
        // TODO: 드래그 이미지를 마우스 위치로 이동
        dragImageObject.transform.position = mousePosition;
    }

    // 드래그 종료
    public void EndDrag(PointerEventData eventData)
    {
        // 버리기 처리는 DropBlocker의 OnDrop()에서 처리됨
        // 여기서는 드래그 종료 후 정리만 수행

        // 드래그 이미지 비활성화
        dragImageObject.SetActive(false);

        // 원본 슬롯 아이콘 알파값 복구
        if(originalIconImage != null)
        {
            Color color = originalIconImage.color;
            color.a = 1f;
            originalIconImage.color = color;
        }

        // 참조 초기화
        draggedItem = null;
        originalIconImage = null;
    }

    // 현재 드래그 중인 아이템 정보를 가져오는 함수 (퀵슬롯용)
    public Item GetDraggedItem()
    {
        // TODO: 현재 드래그 중인 아이템 반환
        return draggedItem;
    }
}
