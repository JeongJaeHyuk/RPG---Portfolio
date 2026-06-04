using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class LootBagPool : MonoBehaviour
{
    public static LootBagPool instance;

    [Header("오브젝트 풀 설정")]
    [SerializeField] GameObject lootBagPrefab;  // Inspector에서 LootBag 프리팹 할당
    [SerializeField] int poolSize = 20;         // 초기 풀 크기

    // 전리품 주머니 오브젝트 풀
    Queue<LootBag> lootBagPool = new Queue<LootBag>();

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
        // 게임시작시 전리품주머니 오브젝트 풀 설정과 생성
        for(int i = 0; i < poolSize; i++)
        {
            // 생성과 동시에 부모설정
            GameObject obj = GameObject.Instantiate(lootBagPrefab,gameObject.transform);
            LootBag lootBag= obj.GetComponent<LootBag>();
            // 큐에 추가
            lootBagPool.Enqueue(lootBag);
            // 오브젝트 비활성화
            obj.gameObject.SetActive(false);
        }
    }

    // 풀에서 LootBag 가져오기
    public LootBag GetLootBag()
    {
        // TODO: 풀에서 LootBag 가져오기 또는 새로 생성
        // 1. if (lootBagPool.Count > 0) 풀에 사용 가능한 오브젝트가 있으면:
        if(lootBagPool.Count > 0)
        {
            // lootBagPool 꺼내기
            LootBag bag = lootBagPool.Dequeue();
            // 꺼낸 오브젝트 활성화
            bag.gameObject.SetActive(true);
            return bag;
        }
        // 풀에 사용 가능한 오브젝트가 없을경우
        else
        {
            // 생성과 동시에 부모설정
            GameObject obj = GameObject.Instantiate(lootBagPrefab,gameObject.transform);
            LootBag bag = obj.GetComponent<LootBag>();
            return bag;
        }

    }

    // LootBag을 풀로 반환
    public void ReturnLootBag(LootBag _bag)
    {
        // TODO: LootBag을 풀로 반환
        // 1. bag.Clear(); 로 초기화
        _bag.Clear();
        // 2. lootBagPool.Enqueue(bag); 로 큐에 다시 추가
        lootBagPool.Enqueue(_bag);
        // 좌표를 다시 원래 있던 lootBagPool로 이동
        _bag.transform.position = gameObject.transform.position;
    }
}
