using UnityEngine;
using UnityEngine.UI;

// =====================================================================
// UI_SaveSlot : 불러오기 패널의 세이브 슬롯 하나를 담당하는 UI 스크립트
// =====================================================================
// [사용 방법]
// 1. 슬롯 1개짜리 프리팹을 만들고 이 스크립트를 붙입니다.
// 2. 인스펙터에서 Text, Button 요소들을 연결합니다.
// 3. StartMenu에서 Instantiate로 슬롯을 생성한 뒤 Setup()을 호출합니다.
//
// [프리팹 구성 예시]
// UI_SaveSlot (이 스크립트 부착)
//   ├─ SlotNameText   (Text) ← 세이브 이름
//   ├─ SlotDateText   (Text) ← 저장 날짜
//   ├─ SlotLevelText  (Text) ← 플레이어 레벨
//   ├─ SelectButton   (Button) ← 이 슬롯으로 게임 시작
//   └─ DeleteButton   (Button) ← 이 슬롯 삭제
// =====================================================================
public class UI_SaveSlot : MonoBehaviour
{
    [SerializeField] Text slotNameText;     // 세이브 이름 표시
    [SerializeField] Text slotDateText;     // 마지막 저장 날짜 표시
    [SerializeField] Text slotLevelText;    // 플레이어 레벨 표시 (Lv.5 형태)
    [SerializeField] Button selectButton;   // 클릭하면 이 세이브로 게임 시작
    [SerializeField] Button deleteButton;   // 클릭하면 이 세이브 파일 삭제

    // 이 슬롯이 표시하는 세이브 데이터
    SaveData data;
    // 버튼 이벤트를 실제로 처리할 StartMenu 참조
    StartMenu startMenu;

    // ------------------------------------------------------------------
    // 슬롯 초기화 함수 (StartMenu의 OpenLoadPanel에서 호출)
    // _data    : 이 슬롯에 표시할 세이브 파일 정보
    // _startMenu : 버튼 클릭 시 실행할 함수가 있는 StartMenu 참조
    // ------------------------------------------------------------------
    public void Setup(SaveData _data, StartMenu _startMenu)
    {
        data      = _data;
        startMenu = _startMenu;

        // 텍스트 UI에 세이브 정보 표시
        slotNameText.text  = "저장된 이름 : " + data.saveName;
        slotLevelText.text = "Lv." + data.playerLevel;
        slotDateText.text  = "저장된 날짜 :" + data.saveDate;

    }

    // 선택 버튼 OnClick에 연결 (에디터에서 연결)
    public void SlotSelect()
    {
        Check_ToolTip.Instance.ShowLoadConfirm(() => startMenu.SelectSaveAndLoad(data.saveName));
    }

    // 삭제 버튼 OnClick에 연결 (에디터에서 연결)
    public void SlotDelete()
    {
        Check_ToolTip.Instance.ShowDeleteConfirm(() => startMenu.DeleteSaveSlot(data.saveName));
    }
}
