using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DropData
{
    public int itemId;          // 드랍할 아이템 ID
    public float dropRate;      // 드랍 확률 (0~1)
    public int dropCount;       // 드랍 개수
}

public class DropTable_Manager : MonoBehaviour
{
    public static DropTable_Manager instance;

    // 프로퍼티로 변경 - 매번 호출 시 최신 경로 반환
    string dropTablePath => Application.dataPath + "/Resources/ItemDataPath/DropTable.csv";

    // 몬스터별 드랍 테이블 (한 몬스터가 여러 아이템 드랍 가능)
    Dictionary<string, List<DropData>> dropTableByMonster;

    void Awake()
    {
        // 싱글톤 설정
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 자동으로 CSV 로드
        LoadDropTable();
    }

    // CSV 로드 (게임 시작 시 한 번만 호출)
    void LoadDropTable()
    {
        dropTableByMonster = new Dictionary<string, List<DropData>>();

        if (File.Exists(dropTablePath))
        {
            string line = string.Empty;
            using (StreamReader sr = new StreamReader(dropTablePath))
            {
                line = sr.ReadLine(); // 첫번째줄 헤더 읽고 넘김
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    if (data.Length >= 4)
                    {
                        string monsterName = data[0].Trim();
                        int itemId = int.Parse(data[1].Trim());
                        float dropRate = float.Parse(data[2].Trim());
                        int dropCount = int.Parse(data[3].Trim());

                        // DropData 객체 생성
                        DropData dropData = new DropData
                        {
                            itemId = itemId,
                            dropRate = dropRate,
                            dropCount = dropCount
                        };

                        // 몬스터별 드랍 테이블 Dictionary에 추가
                        if (!dropTableByMonster.ContainsKey(monsterName))
                        {
                            dropTableByMonster[monsterName] = new List<DropData>();
                        }
                        dropTableByMonster[monsterName].Add(dropData);
                    }
                }
            }

            Debug.Log($"드랍 테이블 로드 완료: {dropTableByMonster.Count}개 몬스터");
        }
        else
        {
            Debug.LogError($"드랍 테이블 파일을 찾을 수 없습니다: {dropTablePath}");
        }
    }

    // 몬스터 이름으로 드랍 아이템 결정 (확률 계산)
    // 반환값: 드랍된 아이템의 DropData (드랍 실패 시 null)
    public DropData GetDropItem(string _monsterName)
    {
        if (dropTableByMonster.TryGetValue(_monsterName, out List<DropData> dropList))
        {
            // 몬스터의 드랍 테이블 순회하며 확률 체크
            foreach (DropData dropData in dropList)
            {
                float randomValue = Random.Range(0f, 1f);
                if (randomValue <= dropData.dropRate)
                {
                    // 드랍 성공하면 dropData 리턴
                    return dropData;
                }
            }

            // 모든 확률 실패 - 드랍 없음
            return null;
        }
        else
        {
            Debug.LogWarning($"몬스터 '{_monsterName}'의 드랍 테이블이 없습니다.");
            return null;
        }
    }
}
