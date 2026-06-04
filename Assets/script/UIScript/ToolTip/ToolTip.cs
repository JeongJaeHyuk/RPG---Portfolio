using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToolTip : MonoBehaviour
{
    public static ToolTip instance = null;
    [SerializeField] Image icon;            // 아이콘 이미지
    [SerializeField] Text itemName;         // 아이템 이름
    [SerializeField] Text description;      // 아이템 설명
    [SerializeField] Text spec;             // 아이템 스펙 공격력,방어력, 회복량
    [SerializeField] Text sale;             // 판매 금액(아직설정안됨)


    public void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            gameObject.SetActive(false);
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Show(Item _item, RectTransform slotRect)
    {
        icon.sprite = _item.itemIcon;
        itemName.text = _item.itemName;
        description.text = _item.description;
        spec.text = string.Empty; // 텍스트 먼저 초기화(기존의 텍스트떄문에 코드가 꼬임방지)
        switch(_item.type)
        {
            case ItemType.Weapon:
                spec.text = "공격력 : " + _item.attackPower.ToString();
                break;
            case ItemType.Armor:
                spec.text = "방어력 : " + _item.defense.ToString();
                break;
            case ItemType.Consumable:
                if(_item.hpRecover > 0)
                {
                    spec.text = "HP : +" + _item.hpRecover.ToString();
                }
                if(_item.mpRecover > 0)
                {
                    // hp가 존재할경우 다음줄로 이동
                    if(spec.text != string.Empty)
                    {
                        spec.text += "\n";
                    }

                    spec.text += "MP : +" + _item.mpRecover.ToString();
                }
                break;
            case ItemType.Material:
                spec.text = string.Empty; // 기타 아이템은 스펙이 필요없어서 텍스트를 비움
                break;
        }
        sale.text = "판매금액 : " + (int)(_item.price * 0.6) + " Gold";

        // 슬롯의 우상단에 툴팁 배치
        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);
        // corners[0]: 좌하단, [1]: 좌상단, [2]: 우상단, [3]: 우하단
        transform.position = corners[2]; // 슬롯 우상단에 툴팁 좌상단을 맞춤 (피봇 0,1 기준)

        // 이 스크립트 게임오브젝트활성화
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        //이 스크립트 게임오브젝트 비활성화
        gameObject.SetActive(false);
    }
}
