using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =====================================================================
// StartMenu : 게임 시작 화면의 UI 흐름 전체를 관리하는 스크립트
// =====================================================================
// [화면 구성]
// 1. 메인 패널   : 새로만들기 / 불러오기 / 게임종료 버튼
// 2. 새로만들기 패널 : 세이브 이름 입력 → 확인/취소
// 3. 불러오기 패널  : 기존 세이브 슬롯 목록 → 슬롯 선택 or 삭제 / 취소
//
// [Unity 에디터 설정 방법]
// 1. 새 씬(StartScene)을 만들고 이 스크립트를 빈 GameObject에 붙입니다.
// 2. Canvas 아래에 패널 3개(메인/새로만들기/불러오기)를 만듭니다.
// 3. 각 버튼의 OnClick 이벤트에 이 스크립트의 함수를 연결합니다.
// 4. Build Settings에서 StartScene(index 0), GameScene(index 1) 순서로 등록합니다.
// =====================================================================
public class StartMenu : MonoBehaviour
{
    // ------------------------------------------------------------------
    // 인스펙터에서 연결할 UI 요소들
    // ------------------------------------------------------------------

    [Header("── 메인 버튼 ──────────────────")]
    [SerializeField] Button newGameButton;  // [새로만들기] 버튼
    [SerializeField] Button loadGameButton; // [불러오기] 버튼 (세이브 없으면 비활성화)
    [SerializeField] Button exitButton;     // [게임종료] 버튼

    [Header("── 새로만들기 패널 ─────────────")]
    [SerializeField] GameObject newGamePanel;       // 이름 입력 팝업 오브젝트
    [SerializeField] InputField saveNameInput;      // 세이브 이름 입력 필드
    [SerializeField] Text errorText;                // 오류 메시지 (중복 이름, 빈 이름 등)

    [Header("── 불러오기 패널 ──────────────")]
    [SerializeField] GameObject loadGamePanel;      // 슬롯 목록 팝업 오브젝트
    [SerializeField] Transform slotParent;          // 슬롯들이 들어갈 부모 (ScrollView의 Content)
    [SerializeField] GameObject saveSlotPrefab;     // UI_SaveSlot 프리팹 (슬롯 1개 단위 UI)

    void Start()
    {
        // 시작 시 팝업 패널은 모두 닫아둡니다
        newGamePanel.SetActive(false);
        loadGamePanel.SetActive(false);

        // 저장된 세이브 파일이 하나도 없으면 [불러오기] 버튼을 비활성화합니다
        loadGameButton.interactable = PlayerDataManager.HasAnySave();
    }

    // ==================================================================
    // 메인 버튼 함수들
    // ==================================================================

    // [새로만들기] 버튼 OnClick에 연결
    // 이름 입력 패널을 열고 이전 입력값과 오류 메시지를 초기화합니다
    public void OnNewGameClick()
    {
        saveNameInput.text = "";
        errorText.gameObject.SetActive(false); // 오류 텍스트는 처음엔 꺼둠
        newGamePanel.SetActive(true);
    }

    // [불러오기] 버튼 OnClick에 연결
    // 저장된 세이브 파일 목록을 읽어서 슬롯 목록 패널을 엽니다
    public void OnLoadGameClick()
    {
        OpenLoadPanel();
    }

    // [게임종료] 버튼 OnClick에 연결
    // 에디터에서는 플레이 모드를 종료하고, 실제 빌드에서는 게임을 종료합니다
    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ==================================================================
    // 새로만들기 패널 함수들
    // ==================================================================

    // [확인] 버튼 OnClick에 연결
    // 입력한 이름으로 세이브 파일을 생성하고 게임씬으로 이동합니다
    public void OnConfirmNewGame()
    {
        string name = saveNameInput.text.Trim(); // 앞뒤 공백 제거

        // 이름이 비어있으면 오류 텍스트 켜고 중단
        if (string.IsNullOrEmpty(name))
        {
            errorText.text = "이름을 입력해주세요.";
            errorText.gameObject.SetActive(true);
            return;
        }

        // 이미 같은 이름의 세이브가 있으면 오류 텍스트 켜고 중단
        if (PlayerDataManager.SaveExists(name))
        {
            errorText.text = "이미 사용 중인 이름입니다.";
            errorText.gameObject.SetActive(true);
            return;
        }

        // 세이브 파일 생성 후 게임씬으로 이동
        PlayerDataManager.CreateSave(name);
        LoadingData.LoadScene(SceneName.TownScene);
    }

    // [취소] 버튼 OnClick에 연결
    // 새로만들기 패널을 닫고 메인 화면으로 돌아갑니다
    public void OnCancelNewGame()
    {
        newGamePanel.SetActive(false);
    }

    // ==================================================================
    // 불러오기 패널 함수들
    // ==================================================================

    // 세이브 파일 목록을 읽어서 슬롯 UI를 동적으로 생성합니다
    void OpenLoadPanel()
    {
        // 기존에 생성된 슬롯 오브젝트를 전부 삭제 (이전 목록 초기화)
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }           

        // PlayerDataManager에서 모든 세이브 파일 정보를 가져옵니다 (최신순 정렬)
        List<SaveData> saves = PlayerDataManager.GetAllSaves();

        // 저장된 파일이 없으면 안내 텍스트만 표시하고 패널 열지 않음
        if (saves.Count == 0)
        {
            Check_ToolTip.Instance.ShowEmptySlot();
            return;
        }
        // 저장된 파일이 있으면 안내 텍스트를 키지않는다.
        Check_ToolTip.Instance.gameObject.SetActive(false);
        
        // 세이브 파일 개수만큼 슬롯 UI를 생성하고 데이터를 넣어줍니다
        foreach (SaveData save in saves)
        {
            // saveSlotPrefab(UI_SaveSlot)을 slotParent 아래에 복제 생성
            GameObject obj = Instantiate(saveSlotPrefab, slotParent);
            UI_SaveSlot slot = obj.GetComponent<UI_SaveSlot>();
            // 슬롯에 세이브 데이터와 이 StartMenu 참조를 전달해서 버튼 이벤트 연결
            slot.Setup(save, this);
        }

        loadGamePanel.SetActive(true);
    }

    // UI_SaveSlot의 [선택] 버튼에서 호출
    // 선택한 세이브를 현재 세이브로 설정하고 게임씬으로 이동합니다
    public void SelectSaveAndLoad(string saveName)
    {
        PlayerDataManager.SelectSave(saveName);
        LoadingData.LoadScene(SceneName.TownScene);
    }

    // UI_SaveSlot의 [삭제] 버튼에서 호출
    // 해당 세이브 파일을 삭제하고 목록을 새로고침합니다
    public void DeleteSaveSlot(string saveName)
    {
        PlayerDataManager.DeleteSave(saveName);

        // 세이브가 하나도 없으면 패널을 닫고 [불러오기] 버튼도 비활성화
        if (!PlayerDataManager.HasAnySave())
        {
            loadGamePanel.SetActive(false);
            loadGameButton.interactable = false;
            return;
        }

        // 남은 세이브가 있으면 목록만 새로고침
        OpenLoadPanel();
    }

    // [취소] 버튼 OnClick에 연결
    // 불러오기 패널을 닫고 메인 화면으로 돌아갑니다
    public void OnCancelLoad()
    {
        loadGamePanel.SetActive(false);
    }
}
