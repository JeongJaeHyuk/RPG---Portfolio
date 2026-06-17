using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum MonsterType
{
    BlueOrc   = 1,
    GreenOrc  = 2,
    RedOrc    = 3,
    RockGolem = 4
}

public class MonsterData
{
    public int    id;
    public string monsterName;
    public float  maxHp;
    public float  damage;
    public float  attackRange;
    public float  attackSpeed;
    public float  exp;
}

public class Monster_Data_Manager : MonoBehaviour
{
    public static Monster_Data_Manager Instance { get; private set; }

    Dictionary<int, MonsterData> monsterTable = new Dictionary<int, MonsterData>();

    string monsterDataPath => Application.dataPath + "/Resources/MonsterDataPath/MonsterData.csv";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCSV();
    }

    void LoadCSV()
    {
        if (!File.Exists(monsterDataPath))
        {
            Debug.LogError("MonsterData.csv 파일이 존재하지 않습니다 : " + monsterDataPath);
            return;
        }

        using (StreamReader sr = new StreamReader(monsterDataPath))
        {
            sr.ReadLine(); // 헤더 스킵

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;

                string[] col = line.Split(',');

                MonsterData data  = new MonsterData();
                data.id          = int.Parse(col[0].Trim());
                data.monsterName = col[1].Trim();
                data.maxHp       = float.Parse(col[2].Trim());
                data.damage      = float.Parse(col[3].Trim());
                data.attackRange = float.Parse(col[4].Trim());
                data.attackSpeed = float.Parse(col[5].Trim());
                data.exp         = float.Parse(col[6].Trim());

                monsterTable[data.id] = data;
            }
        }
    }

    public MonsterData GetData(MonsterType type)
    {
        int id = (int)type;
        if (monsterTable.TryGetValue(id, out MonsterData data))
            return data;

        Debug.LogError($"[Monster_Data_Manager] ID {id} 에 해당하는 몬스터 데이터가 없습니다.");
        return null;
    }
}
