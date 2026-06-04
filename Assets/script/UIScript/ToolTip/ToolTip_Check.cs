using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip_Check : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] Slider slider;
    [SerializeField] Text itemCountText;
    Item targetItem = null;


    void Awake()
    {
        gameObject.SetActive(false);
    }
    // 버리기 버튼눌럿을시 그 아이템정보받아오는 함수
    public void SetItem(Item _item)
    {
        targetItem = _item;
        itemIcon.sprite = _item.itemIcon;
        if(targetItem != null)
        {
            gameObject.SetActive(true);
            if(targetItem.currentCount <= 1)
            {
                slider.gameObject.SetActive(false);
            }
            else
            {
                slider.gameObject.SetActive(true);
                int minValue = 1;
                slider.maxValue = targetItem.currentCount;
                slider.minValue = minValue;
                slider.value = minValue;
                itemCountText.text = slider.minValue + "/" + slider.maxValue;
                OnSliderValueChanged();
            }
        }
    }

    // 슬라이더 값이 변경될 때 호출되는 함수 (슬라이더의 OnValueChanged에 연결)
    public void OnSliderValueChanged()
    {
        int thorwAwayCount = (int)slider.value;
        itemCountText.text = thorwAwayCount + " / " + targetItem.currentCount;
    }

    // 확인버튼눌렀을 시 버리기 (확인버튼에 연결)
    public void TrueButtonClick()
    {
        int thorwAwayCount;

        if(slider.gameObject.activeSelf)
        {
            thorwAwayCount = (int)slider.value;
        }
        else
        {
            thorwAwayCount = targetItem.currentCount;
        }

        targetItem.currentCount -= thorwAwayCount;

        if(targetItem.currentCount <= 0)
        {
            UI_Inventory.inven.RemoveItem(targetItem);
            targetItem = null;
            itemIcon.sprite = null;
        }
        else
        {
            // 개수만 줄어든 경우 UI 업데이트
            UI_Inventory.inven.Refresh();
        }

        gameObject.SetActive(false);
    }

    // 취소했을때 오브젝트 끄면서 참조되었던 변수들 null로 변경 취소 버튼에 연결
    public void FalseButtonClick()
    {
        // 취소
        gameObject.SetActive(false);
        targetItem = null;
        itemIcon.sprite = null;
    }
}
