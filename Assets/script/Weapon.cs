using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] enum Type { Melee, Range }; // 근접, 원거리
    [SerializeField] Type type;
    [SerializeField] float damage; // 무기공격력
    [SerializeField] string weaponsGrade; // 무기등급
    [SerializeField] float attackSpeed; //공격속도
    [SerializeField] TrailRenderer effect; //공격효과


    //public void Use()
    //{
    //    if (type == Type.Melee)
    //    {
    //        StopCoroutine("Swing");//같은 코루틴함수를 실행할떄 겹치치않도록 실행중인 코루틴함수 멈추게하는 코루틴 정지 함수
    //        StartCoroutine("Swing");
    //    }
    //}
    //IEnumerator Swing()
    //{
    //    //1로직 실행
    //    yield return new WaitForSeconds(0.1f);// 0.1초 대기
    //    boxCollider.enabled = true;
    //    effect.enabled = true;
        
    //    //2로직 실행
    //    yield return new WaitForSeconds(0.3f);// 0.3초 대기
    //    boxCollider.enabled = false;
        
    //    //3로직 실행
    //    yield return new WaitForSeconds(0.3f);// 0.3초 대기
    //    effect.enabled = false;


    //    //yield return null;// 1프레임 대기

    //    //yield break; //코루틴 함수 탈출
    //}
}
