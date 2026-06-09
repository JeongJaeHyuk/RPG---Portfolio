using UnityEngine;

public class Game_Setting : MonoBehaviour
{
    [SerializeField] GameObject settingObj;

    void Awake()
    {
        settingObj.SetActive(false);
    }

    void Update()
    {
        // 다른 UI가 닫혀있을 때만 ESC로 설정창 토글
        if (Input.GetKeyDown(KeyCode.Escape) && !UI.Instance.IsAnyUIOpen)
        {
            settingObj.SetActive(!settingObj.activeSelf);
        }
    }

    // 저장하기 버튼
    public void GameSave()
    {
        //PlayerManager.Instance.SaveCurrentData();
        Debug.Log("저장 완료");
    }

    // 게임 종료 버튼
    public void GameExit()
    {
        //PlayerManager.Instance.SaveCurrentData();
        Application.Quit();
        Debug.Log("게임 종료");
    }

    // 설정 버튼 (기능 미구현)
    public void GameSetting()
    {
        Debug.Log("설정창 (미구현)");
    }
}
