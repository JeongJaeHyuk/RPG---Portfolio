using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Manager : MonoBehaviour
{
    public enum NPCType
    {
        Consume,
        Equip
    }
    public static Shop_Manager Instance { get; private set; } = null;
    [SerializeField] GameObject consumeTab; // 소비상점UI
    [SerializeField] GameObject equipTab;   // 장비상점UI
    public bool isConsumeNPC = false;   // 소비상인npc가 근처에 있는가 여부체크
    public bool isEquipNPC = false;     // 장비상인npc가 근처에 있는가 여부체크
    public bool isShopOpen => consumeTab.activeSelf || equipTab.activeSelf;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllShopTab();
            ToolTip.instance.Close();
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isConsumeNPC)
            {
                ToggleShop(consumeTab);
            }
            else if(isEquipNPC)
            {
                ToggleShop(equipTab);
            }
        }
        
    }
    // 상점 열기/닫기 함수
    void ToggleShop(GameObject _shopTab)
    {
        _shopTab.SetActive(!_shopTab.activeSelf);
    }
    // 모든 상점닫기
    public void CloseAllShopTab()
    {
        consumeTab.SetActive(false);
        equipTab.SetActive(false);
    }
    // NPC 상호작용 설정 함수
    public void IsSetNpc(NPCType _type, bool _is)
    {
        switch(_type)
        {
            case NPCType.Consume:
                isConsumeNPC = _is;
                break;
            case NPCType.Equip:
                isEquipNPC = _is;
                break;
        }
    }
}
