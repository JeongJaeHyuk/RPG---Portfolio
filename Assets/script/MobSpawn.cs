using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawn : MonoBehaviour
{
    // RcManager가 따로있어서 시리얼라이즈필드로 드래그엔드랍해서 정보 알고있게 만들것
    // 그리고 리스트에서 넣고 생성하고 생성위치를 monster스크립트에서 SpwanPos 변수를 만들어서 그걸만들고 몬스터가 죽으면
    // 몬스터스크립트에 자기부모스크립트인 MobSpawn의 함수 Repawn을 불러와서 n초후에 재생성되는 코드를 만들면 되겠구나  

    // 여기서 처음생성을 하고 오브젝트 껏다켯다하는기능은 다른곳에서 실행
    // 여기 자식에 MobReSpawn이라는 오브젝트를 n개 가지고있으며 각자 한개의 프리팹의 리스폰을 담당하는 오브젝트이다.
    // 그 리스폰을 담당하는 오브젝트 이름으로 생성할 오브젝트를 정할 것이며 이름뒤에 _를 만들어서 뒤에 생기는 이름을 버리고 리소스에잇는 순수이름을 가져와 오브젝트를 생성한다.
    [SerializeField] ResourcesLoadManager rcManager; // 돈 디스트로이보다 그냥 모든씬에서 이 매니저를 가지고있고 지금 이 오브젝트와 스크립트도 그럴 것
    [SerializeField] Transform[] spawn; // 배열은 씬에서 직접내가 만져서 생성수를 조절할 수 있다.
    private void Awake()
    {
        spawn = new Transform[transform.childCount];
    }
    void Start()
    {
        // 오브젝트 자식으로 생성만하고 할당을 자동을 할수있게 만듬
        for (int i = 0; i < spawn.Length; i++)
        {
            spawn[i] = transform.GetChild(i);
        }

        for (int i = 0; i < spawn.Length; i++)
        {
            GameObject mob = GameObject.Instantiate<GameObject>(rcManager.GetRcMonster(spawn[i].name.Substring(0,spawn[i].name.IndexOf("_"))));
            mob.name = rcManager.GetRcMonster(spawn[i].name.Substring(0, spawn[i].name.IndexOf("_"))).name;
            mob.transform.SetParent(spawn[i]);
            mob.transform.localPosition = Vector3.zero;
        }
    }
}
