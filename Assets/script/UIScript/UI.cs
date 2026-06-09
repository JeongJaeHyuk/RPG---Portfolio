using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance { get; private set; } = null;
    [SerializeField] GameObject equip;
    [SerializeField] GameObject skiilBackGround;
    [SerializeField] GameObject inventoryBackGround;
    [SerializeField] GameObject questBackGround;
    [SerializeField] GameObject bottombar_SkillCoolTime;    // 플레이어 스킬쿨타임 연결시킬 변수
    [SerializeField] Player player = null;
    [SerializeField] PlayerSpecs plsl = null;
    [SerializeField] PlayerProgression pps = null;
    public bool IsAnyUIOpen => isInvenOpen || isSkillOpen || isShopOpen || isPlayerStatOpen;
    public bool isInvenOpen = false;
    public bool isSkillOpen = false;
    public bool isShopOpen = false;
    public bool isPlayerStatOpen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            // Player가 가지고있는 스크립트를 각자 변수에 할당
            player = objPlayer.GetComponent<Player>();
            plsl = objPlayer.GetComponent<PlayerSpecs>();
            pps = objPlayer.GetComponent<PlayerProgression>();
        }
        else
        {
            Debug.LogError("UI스크립트에서 player가 할당되지않았습니다");
        }    
        equip.gameObject.SetActive(false);
        //StartCoroutine(RefreshSkill());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            switch (true)
            {
                case bool _ when Input.GetKeyDown(KeyCode.K):
                    skiilBackGround.SetActive(!skiilBackGround.activeSelf);
                    break;

                case bool _ when Input.GetKeyDown(KeyCode.U):
                    equip.SetActive(!equip.activeSelf);
                    break;

                case bool _ when Input.GetKeyDown(KeyCode.I):
                    // I 키에 대한 처리 (인벤토리 ON OFF키로 사용할듯)
                    inventoryBackGround.SetActive(!inventoryBackGround.activeSelf);
                    break;
                case bool _ when Input.GetKeyDown(KeyCode.O):
                    questBackGround.SetActive(!questBackGround.activeSelf);
                    break;
                default:
                    // 예외 처리
                    break;
            }
        }
    }
    // 스킬 csv파일 정보를 받아 UI에 뿌릴떄 스킬창을 한번눌러야 갱신되는 문제가 있어서 만들어놈
    IEnumerator RefreshSkill()
    {
        yield return new WaitForSeconds(2f);
        skiilBackGround.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        skiilBackGround.gameObject.SetActive(false);
    }

    // UI중 현재 캐릭터 정보를 보여주는 창 끄는 함수 BackGround_UI 자식으로 Close오브젝트에서 사용중 
    public void Close_Tap(Button _button )
    {
        Transform parentTrans = _button.transform.parent;
        if (parentTrans != null)
        {
            parentTrans.gameObject.SetActive(false);
        }
    }

    // 이건 뭘까?
    public Player GetPlayer()
    {
        return player;
    }
    // UIBOTTOMbar에서 PlayerSpec스크립트 정보를 가져올떄 사용하는 함수
    public PlayerSpecs GetPlayerSpecs()
    {
        return plsl;
    }
    public PlayerProgression GetPlayerProg()
    {
        return pps;
    }
    public Skill_CoolTime[] GetSkillCoolTimes()
    {
        return bottombar_SkillCoolTime.transform.GetComponentsInChildren<Skill_CoolTime>();
    }
        
}
