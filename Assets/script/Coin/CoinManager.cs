using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    // 싱글톤 버전으로 몬스터가 죽엇을 때 몬스터 스크립트에서 자기 좌표를 주고 코인오브젝트를 보내주는형식으로
    public static CoinManager coinManager = null;
    public Dictionary<string, Queue<GameObject>> coinQueues;
    const int constCopperMax = 500;
    const int constSilverMax = 1000;


    void Awake()
    {
        if(coinManager == null)
        {
            coinManager = this;
            InitializeQueues();
        } 
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        CreateCoin();
    }
    // 초기화 선언
    void InitializeQueues()
    {
        coinQueues = new Dictionary<string, Queue<GameObject>>()
        {
            {"Copper", new Queue<GameObject>()},
            {"Silver", new Queue<GameObject>()},
            {"Gold", new Queue<GameObject>()}
        };
    }

    // 코인매니저 자식의 이름에따라 코인오브젝트를 20개씩 생성하고 부모지정하는 하고 딕셔너리에 알맞게 넣는 함수
    void CreateCoin()
    {
        int count = transform.childCount;
        for(int i = 0; i < count;i++)
        {
            Transform parentTrans = transform.GetChild(i);
            string coinType = parentTrans.name;
            GameObject prefab = ResourcesLoadManager.rcManager.GetrcCoin(coinType);
            for(int j = 0; j < 20; j++)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(prefab);
                obj.transform.SetParent(parentTrans);
                obj.transform.position = parentTrans.position;

                Coin coinScript = obj.GetComponent<Coin>();
                if(coinScript != null)
                {
                    coinScript.SetOriPos(obj.transform.position);
                }
                obj.name = prefab.name;
                obj.SetActive(false);
                coinQueues[coinType].Enqueue(obj);
            }
        }
    }
    public void DropCoin(GameObject _monster,int _coinValue)
    {
        string coinType = string.Empty;
        if(_coinValue > 0 && _coinValue < constCopperMax)
        {
            coinType = "Copper";
        }
        else if(_coinValue >= constCopperMax && _coinValue < constSilverMax)
        {
            coinType = "Silver";
        }
        else if(_coinValue >= constSilverMax)
        {
            coinType = "Gold";
        }
        else
        { 
            return;
        }
        if(coinQueues.ContainsKey(coinType) && coinQueues[coinType].Count > 0)
        {
            GameObject coin = coinQueues[coinType].Dequeue();
            coin.transform.position = _monster.transform.position;
            coin.SetActive(true);

            Coin coinScript = coin.GetComponent<Coin>();
            if(coinScript != null)
            {
                coinScript.Gold(_coinValue);
            }
        }
        else
        {
            Debug.LogWarning($"{coinType} 코인 풀이 부족합니다 !");
            // 새로 생성코드 할까말까 고민중
        }       
    }
    
}
