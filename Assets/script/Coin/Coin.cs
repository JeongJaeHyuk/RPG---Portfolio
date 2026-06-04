using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int gold;  // 코인값(코인량)
    Vector3 oriPos;             // 초기위치(position값들고잇음)
    string coinType;            // 코인타입 동 은 금 어떤건지 확인하기위함

    float despwanTime = 15f;    // 플레이어가 돈을 줍지않을 경우 15초후에 사라지는 시간
    Coroutine despawnCoroutine; // 현재 실행중이 코루틴함수를 받아오는 변수;
    void OnEnable()
    {
        if(despawnCoroutine == null)
        {
            despawnCoroutine = StartCoroutine(Despwan());
        }
        
    }
    void OnDisable()
    {
        gold = 0;
        if (CoinManager.Instance.coinQueues.ContainsKey(coinType))
        {
            CoinManager.Instance.coinQueues[coinType].Enqueue(gameObject);
        }
        gameObject.transform.position = oriPos;
        if(despawnCoroutine != null)
        {
            StopCoroutine(despawnCoroutine);
            despawnCoroutine = null;
        }
    }
    void Awake()
    {
        int index = gameObject.name.IndexOf("_");
        if(index == -1)
        {
            Debug.LogError("코인 이름에 _없어서 문제가 생깁니다");
            
        }
        else
        {
            coinType = gameObject.name.Substring(0,index);
        }
    }
     void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(despawnCoroutine != null)
            {
                StopCoroutine(despawnCoroutine);
                despawnCoroutine = null;
            }
            UI_Inventory.inven.GOLD += gold;
            gameObject.SetActive(false);
        }
    }
    public void Gold(int _gold)
    {
        gold = _gold;
    }
    // 처음 이스크립트를 가지고있는 오브젝트 생성시 처음설정된 위치값을 얻기위한함수
    public void SetOriPos(Vector3 _position)
    {
        oriPos = _position;
    }    
    // 코인 디스폰 코루틴 함수
    IEnumerator Despwan()
    {
        yield return new WaitForSeconds(despwanTime);
        gameObject.SetActive(false);
        despawnCoroutine = null;
    }
}
