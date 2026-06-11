using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Check_ToolTip : MonoBehaviour
{
    public static Check_ToolTip Instance { get; private set; } = null;

    // StartScene에서 경고 또는 확인절차 메세지를 띄우는 툴팁
    [SerializeField] Button OkButton;
    [SerializeField] Button cancelButton;
    [SerializeField] Text tooltipText;

    string emptySlotMessage     = "저장된 파일이 없습니다";
    string loadConfirmMessage   = "저장된 파일을 불러오겠습니까?";
    string deleteConfirmMessage = "저장된 파일을 삭제하겠습니까?";

    // 확인 버튼 눌렀을 때 실행할 동작 (외부에서 주입)
    Action onConfirmAction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    // 저장된 파일 없음 경고 (확인 버튼만 표시)
    public void ShowEmptySlot()
    {
        onConfirmAction = null;
        tooltipText.text = emptySlotMessage;
        cancelButton.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    // 불러오기 확인 (확인 + 취소 버튼 표시)
    public void ShowLoadConfirm(Action onConfirm)
    {
        onConfirmAction = onConfirm;
        tooltipText.text = loadConfirmMessage;
        cancelButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    // 삭제 확인 (확인 + 취소 버튼 표시)
    public void ShowDeleteConfirm(Action onConfirm)
    {
        onConfirmAction = onConfirm;
        tooltipText.text = deleteConfirmMessage;
        cancelButton.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    // 에디터에서 확인 버튼 OnClick에 연결
    public void OKButton()
    {
        onConfirmAction?.Invoke();
        gameObject.SetActive(false);
    }

    // 에디터에서 취소 버튼 OnClick에 연결
    public void CancelButton()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        tooltipText.text = string.Empty;
        cancelButton.gameObject.SetActive(true);
    }
}
