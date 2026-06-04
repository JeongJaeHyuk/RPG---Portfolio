using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesLoadManager : MonoBehaviour
{
    #region 싱글톤
    public static ResourcesLoadManager rcManager;
    #endregion

    #region 몬스터 관련
    [SerializeField] GameObject rockGolem;
    [SerializeField] GameObject greenOrc;
    [SerializeField] GameObject blueOrc;
    [SerializeField] GameObject redOrc;
    [SerializeField] GameObject monster3;
    [SerializeField] GameObject boss;

    // 생성할 위치의 오브젝트 이름을 받아와서 리소스에 맞는 오브젝트로 생성하는 함수
    public GameObject GetRcMonster(string _name)
    {
        switch (_name)
        {
            case "RockGolem":
                return rockGolem;
            case "GreenOrc":
                return greenOrc;
            case "BlueOrc":
                return blueOrc;
            case "RedOrc":
                return redOrc;
            case "Monster_3#1":
                return monster3;
            default:
                return null;
        }
    }
    // 리소스로 불러올 몬스터 오브젝트
    public void ResourceLoadMonster()
    {
        rockGolem = Resources.Load<GameObject>("Prefab/Monster/RockGolem_");
        greenOrc = Resources.Load<GameObject>("Prefab/Monster/GreenOrc_");
        blueOrc = Resources.Load<GameObject>("Prefab/Monster/BlueOrc_");
        redOrc = Resources.Load<GameObject>("Prefab/Monster/RedOrc_");
    }
    #endregion
    #region 스킬관련 아이콘 리소스
    [SerializeField] Sprite skillIcon_1;
    [SerializeField] Sprite skillIcon_2;
    [SerializeField] Sprite skillIcon_3;
    [SerializeField] Sprite skillIcon_4;
    [SerializeField] Sprite skillIcon_5;

    // 생성된 skillSlot에서 스킬 이름으로 비교하여 원하는 스킬 아이콘 이미지 가져갈때 사용하는 함수
    public Sprite GetRcSkillIcon(string _name)
    {
        switch (_name)
        {
            case "rolling":
                return skillIcon_1;
            case "bottom cutting":
                return skillIcon_2;
            case "Take it down":
                return skillIcon_3;
            case "Blade storm":
                return skillIcon_4;
            case "Finish Attack":
                return skillIcon_5;

            default:
            return null;
        }
    }
    // 리소스 로드로 sprite타입의 스킬 이미지 받아오기
    public void ResourceLoadSkillIcon()
    {
        skillIcon_1 = Resources.Load<Sprite>("GameAssets/UI/Skill/Skill_Icon/rolling");
        skillIcon_2 = Resources.Load<Sprite>("GameAssets/UI/Skill/Skill_Icon/bottom cutting");
        skillIcon_3 = Resources.Load<Sprite>("GameAssets/UI/Skill/Skill_Icon/Take it down");
        skillIcon_4 = Resources.Load<Sprite>("GameAssets/UI/Skill/Skill_Icon/Blade storm");
        skillIcon_5 = Resources.Load<Sprite>("GameAssets/UI/Skill/Skill_Icon/Finish Attack");
    }
    #endregion
    #region 아이템 아이콘 리소스
    // 아이템 아이콘들을 동적으로 관리하기 위한 Dictionary
    private Dictionary<string, Sprite> itemIcons = new Dictionary<string, Sprite>();

    // 아이템 이름으로 아이콘 가져가기
    public Sprite GetRcItemIcon(string _iconName)
    {
        if (itemIcons.ContainsKey(_iconName))
        {
            return itemIcons[_iconName];
        }
        else
        {
            Debug.LogWarning($"아이템 아이콘을 찾을 수 없습니다: {_iconName}");
            return null;
        }
    }

    // 아이템 아이콘 리소스 로드
    public void ResourceLoadItemIcon()
    {
        // Resources/GameAssets/UI/Item/Item_Icon/ 폴더에서 모든 스프라이트 로드
        Sprite[] icons = Resources.LoadAll<Sprite>("GameAssets/UI/Item/");

        foreach (Sprite icon in icons)
        {
            if (!itemIcons.ContainsKey(icon.name))
            {
                itemIcons.Add(icon.name, icon);
            }
        }
    }
    #endregion
    #region 코인 리소스
    [SerializeField] GameObject copperCoin;
    [SerializeField] GameObject silverCoin;
    [SerializeField] GameObject goldCoin;
    public void ResourceLoadCoin()
    {
        copperCoin = Resources.Load<GameObject>("Prefab/Coin/Copper_Coin");
        silverCoin = Resources.Load<GameObject>("Prefab/Coin/Silver_Coin");
        goldCoin = Resources.Load<GameObject>("Prefab/Coin/Gold_Coin");
    }
    public GameObject GetrcCoin(string _name)
    {
        switch(_name)
        {
            case "Copper":
                    return copperCoin;
            case "Silver":
                    return silverCoin;
            case "Gold":
                    return goldCoin;
            default:
                return null;
        }
    }
    #endregion
    private void Awake()
    {
        // 싱글톤 체크 - 중복 인스턴스 방지
        if (rcManager == null)
        {
            rcManager = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음

            // 리소스 로드
            ResourceLoadMonster(); // 몬스터 리소스 가져오기
            ResourceLoadSkillIcon(); // 스킬아이콘 리소스 가져오기
            ResourceLoadItemIcon(); // 아이템아이콘 리소스 가져오기
            ResourceLoadCoin(); // 코인 리소스 가져오기
        }
        else
        {
            // 이미 인스턴스가 존재하면 중복 생성 방지
            Destroy(gameObject);
        }
    }

}
