using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject Target;

    [SerializeField] float offsetX = 0.0f;            // 카메라의 x좌표
    [SerializeField] float offsetY = 8.0f;           // 카메라의 y좌표
    [SerializeField] float offsetZ = -7.0f;          // 카메라의 z좌표

    [SerializeField] float CameraSpeed = 10.0f;       // 카메라의 속도
    Vector3 TargetPos;                      // 타겟의 위치

    void Start()
    {
        Target = null;
        offsetY = 8.0f;
        offsetZ = -7.0f;
    }
    void Update()
    {
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player");
        }
        // 타겟의 x, y, z 좌표에 카메라의 좌표를 더하여 카메라의 위치를 결정
        TargetPos = new Vector3(
                                Target.transform.position.x + offsetX,
                                Target.transform.position.y + offsetY,
                                Target.transform.position.z + offsetZ);

        // 카메라의 움직임을 부드럽게 하는 함수(Lerp)
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * CameraSpeed);
    }

}
