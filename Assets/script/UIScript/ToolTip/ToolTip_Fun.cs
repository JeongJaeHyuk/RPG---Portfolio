using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip_Fun : MonoBehaviour
{
    public static ToolTip_Fun Instance { get; private set; } = null;
    [SerializeField] GameObject uesGameObject;      // 사용하기 또는 장착하기 오브젝트
    [SerializeField] Text useText;                  // 사용하기 또는 장착하기 텍스트
    [SerializeField] GameObject blocker;            // 아무 화면 클릭시 처리에 필요한 오브젝트
    PlayerSpecs playerSpecs;
    [SerializeField] ToolTip_Check toolTip_Check;  // 버리기 확인용 스크립트 (몇개 버릴지 정할떄도 사용)
    Item tagetItem = null;                        // 클릭한 이미지의 아이템 정보
    bool isEquippedItem = false;                   // 장착된 아이템인지 여부


    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
            playerSpecs = objPlayer.GetComponent<PlayerSpecs>();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    // 툴팁키는 함수
    public void Show(Item _item, RectTransform slotRect, bool isEquipped = false)
    {
        blocker.SetActive(true);
        tagetItem = _item;
        isEquippedItem = isEquipped;

        switch(_item.type)
        {
            case ItemType.Weapon:
            case ItemType.Armor:
                uesGameObject.gameObject.SetActive(true);
                // 장착된 아이템이면 "장착 해제", 아니면 "장착하기"
                useText.text = isEquipped ? "장착 해제" : "장착하기";
                break;
            case ItemType.Consumable:
                uesGameObject.gameObject.SetActive(true);
                useText.text = "사용하기";
                break;
            case ItemType.Material:
                uesGameObject.gameObject.SetActive(false);
                break;
        }
        gameObject.SetActive(true);
        // 슬롯의 우상단에 툴팁 배치
        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);
        // corners[0]: 좌하단, [1]: 좌상단, [2]: 우상단, [3]: 우하단
        transform.position = corners[2]; // 슬롯 우상단에 툴팁 좌상단을 맞춤 (피봇 0,1 기준)
    }
    // 툴팁 끄는함수
    public void Hide()
    {
        //이 스크립트 게임오브젝트 비활성화
        gameObject.SetActive(false);
        //정보 null 처리
        tagetItem = null;
        isEquippedItem = false;
        // 아무화면 눌렀을떄 필요한 오브젝트 비활성화
        blocker.SetActive(false);
    }
    public void UseClick()
    {
        if(tagetItem != null)
        {
            switch(tagetItem.type)
            {
                case ItemType.Weapon:
                    if(isEquippedItem)
                    {
                        UI_Equip_Status.Instance.UnEquipWeapon();
                    }
                    else
                    {
                        UI_Equip_Status.Instance.EquipWeapon(tagetItem);
                    }
                    break;
                case ItemType.Armor:
                    if(isEquippedItem)
                    {
                        UI_Equip_Status.Instance.UnEquipArmor(tagetItem);
                    }
                    else
                    {
                        UI_Equip_Status.Instance.EquipArmor(tagetItem);
                    }
                    break;
                case ItemType.Consumable:
                    playerSpecs.CURRENT_HP += tagetItem.hpRecover;
                    playerSpecs.CURRENT_MP += tagetItem.mpRecover;
                    tagetItem.currentCount--;
                    UI_Inventory.inven.Refresh();
                    if(tagetItem.currentCount <= 0)
                    {
                        UI_Inventory.inven.RemoveItem(tagetItem);
                    }
                    break;
            }
        }
        // 툴팁끄기
        Hide();
    }

    public void ThrowAwayClick()
    {
        Debug.Log("버리기버튼 클릭");

        if(tagetItem != null)
        {
            toolTip_Check.SetItem(tagetItem);
        }

        // 툴팁끄기
        Hide();
    }
    public void BlokerCliock()
    {
        Hide();
    }
}
