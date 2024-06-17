using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class FollowPlayer : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CinemachineVirtualCamera>().Follow = PlayerController.Inst.transform;
    }
}
