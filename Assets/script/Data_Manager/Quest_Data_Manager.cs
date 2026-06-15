using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Quest_Data_Manager : MonoBehaviour
{
    public static Quest_Data_Manager instance;

    // 프로퍼티로 변경 - 매번 호출 시 최신 경로 반환
    string questDataPath => Application.dataPath + "/Resources/QuestData/QuestData.csv";

    // 게임 시작 시 로드된 데이터
    Dictionary<int, List<Quest>> questsByNPC; // NPC별 퀘스트 (private으로 변경)

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadQuestData();
    }

    // CSV 로드 (게임 시작 시 한 번만 호출)
    void LoadQuestData()
    {
        questsByNPC = new Dictionary<int, List<Quest>>();

        if (File.Exists(questDataPath))
        {
            string line = string.Empty;
            using (StreamReader sr = new StreamReader(questDataPath))
            {
                line = sr.ReadLine(); // 첫번째줄 헤더 읽고 넘김
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(",");
                    if (data.Length >= 15)
                    {
                        int questId = int.Parse(data[0].Trim());
                        string questName = data[1].Trim();
                        int npcId = int.Parse(data[2].Trim());
                        string questTypeString = data[3].Trim();
                        bool isRepeatable = int.Parse(data[4].Trim()) == 1;
                        int requiredLevel = int.Parse(data[5].Trim());
                        int preQuestId = int.Parse(data[6].Trim());
                        string description = data[7].Trim();
                        string targetType = data[8].Trim();
                        string targetNameKorean = data[9].Trim();
                        int targetCount = int.Parse(data[10].Trim());
                        int rewardExp = int.Parse(data[11].Trim());
                        int rewardGold = int.Parse(data[12].Trim());
                        int rewardItemId = int.Parse(data[13].Trim());
                        int rewardItemCount = int.Parse(data[14].Trim());

                        // QuestType 열거형으로 변환
                        QuestType questType;
                        if (!Enum.TryParse(questTypeString, out questType))
                        {
                            Debug.Log(questName + "의 퀘스트 타입을 찾을 수 없어 기본값 Kill로 설정합니다");
                            questType = QuestType.Kill;
                        }

                        // Quest 객체 생성
                        Quest quest = new Quest
                        {
                            questId = questId,
                            questName = questName,
                            npcId = npcId,
                            questType = questType,
                            isRepeatable = isRepeatable,
                            requiredLevel = requiredLevel,
                            preQuestId = preQuestId,
                            description = description,
                            targetType = targetType,
                            targetNameKorean = targetNameKorean,
                            targetCount = targetCount,
                            rewardExp = rewardExp,
                            rewardGold = rewardGold,
                            rewardItemId = rewardItemId,
                            rewardItemCount = rewardItemCount
                            // CurrentProgress는 기본 생성자에서 0으로 초기화됨
                        };

                        // NPC별 퀘스트 Dictionary에 추가
                        if (!questsByNPC.ContainsKey(npcId))
                        {
                            questsByNPC[npcId] = new List<Quest>();
                        }
                        questsByNPC[npcId].Add(quest);
                    }
                }
            }

            Debug.Log($"퀘스트 데이터 로드 완료: {questsByNPC.Count}개 NPC");
        }
        else
        {
            Debug.LogError($"퀘스트 데이터 파일을 찾을 수 없습니다: {questDataPath}");
        }
    }

    // NPC ID로 퀘스트 리스트 반환
    public List<Quest> GetQuestsByNPC(int npcId)
    {
        if (questsByNPC.TryGetValue(npcId, out List<Quest> quests))
        {
            return quests;
        }
        else
        {
            Debug.LogWarning($"NPC ID {npcId}에 해당하는 퀘스트가 없습니다.");
            return null;
        }
    }

    // 퀘스트 ID로 원본 퀘스트 반환 (세이브 로드 시 사용)
    public Quest GetQuestById(int _questId)
    {
        foreach (List<Quest> quests in questsByNPC.Values)
        {
            Quest found = quests.Find(q => q.questId == _questId);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}
