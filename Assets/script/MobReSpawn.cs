using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobReSpawn : MonoBehaviour
{
    // 이오브젝트들은 내가 생성할 오브젝트로 이름을 바꾸고이름다음에 #를 꼭붙일것 그뒤에 쓸대없는 이름을 잘라서 사용하기 위해
    float timer = 0f; // 실제 몬스터 생성 타이머
    float reSpawnTime; // 몬스터 재생성시간
    void Start()
    {
        string monsterName = gameObject.name.Substring(0, gameObject.name.IndexOf("_"));
        switch (monsterName) // 이 오브젝트 이름을 받아서 #뒤는 자르고 그 몬스터마다 재생성시간 할당하기
        {
            case "RockGolem":
                reSpawnTime = 5f;
                
                break;
            case "GreenOrc":
                reSpawnTime = 5f;
                
                break;
            case "BlueOrc":
            case "RedOrc":
            {
                reSpawnTime = 5f;
            }            
                break;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.childCount != 0) // 자기 자식이 존재할 경우 실행
        {
            if (gameObject.transform.GetChild(0).gameObject.activeSelf == false) // 자기자식에 있는 오브젝트가 비활성화(죽을) 될경우 실행
            {
                timer += Time.deltaTime;
                if (timer >= reSpawnTime) // 타이머가 리스폰시간보다 커질경우 실행
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true); // 자식에잇는 몬스터 다시 활성화(재생성)
                    gameObject.transform.GetChild(0).transform.position = transform.position; // 처음 생성위치로 이동
                    timer = 0f; // 재생성됏으니 타이머를 0초로 대입
                    return;
                }
            }
        }
    }
}
