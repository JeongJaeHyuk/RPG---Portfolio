using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BasicAttack"))
        {
            Debug.Log("기본공격감지");
        }
        else if (other.gameObject.CompareTag("Spear"))
        {
            Debug.Log("찌르기 감지");
        }
    }
}
