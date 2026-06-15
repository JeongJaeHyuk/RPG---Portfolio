using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Quest : MonoBehaviour
{
    [Header("NPC 설정")]
    public int npcId;  // Inspector에서 설정 (1, 2, 3...)
    [SerializeField] string npcName; // Inspector에서 설정
    bool IsPlayer = false;  // 플레이어 근처 여부

    void OnTriggerEnter(Collider other)
    {
        // TODO: Player가 NPC 근처에 왔을 때 처리
        if(other.CompareTag("Player"))
        {
            IsPlayer = true;
            // 방문 퀘스트 처리
            UI_Quest.instance.VisitLocationQuest(npcName);
        }

        // - Player 태그 체크
        // - 플레이어가 근처에 있다는 플래그 설정
    }

    void OnTriggerExit(Collider other)
    {
        // TODO: Player가 NPC에서 멀어졌을 때 처리
        // - Player 태그 체크
        if(other.CompareTag("Player"))
        {
            IsPlayer = false;
        }
        // - 플레이어가 멀어졌다는 플래그 해제
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && IsPlayer)
        {
            if(UI_QuestWindow.instance.gameObject.activeSelf)
                UI_QuestWindow.instance.CloseWindow();
            else
                UI_QuestWindow.instance.ShowQuestWindow(npcId);
        }
    }
}
