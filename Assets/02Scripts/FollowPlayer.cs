using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float cameraShakeTime = 0.5f;
    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin cameraShake;
    private void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.Follow = PlayerController.Inst.transform;

        cameraShake = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        GameManager.GameOverEvent += CameraShake;
    }
    private void CameraShake(Attack obstacle)
    {
        LeanTween.value(obstacle.CameraShakeAmplitude, 0f, cameraShakeTime).setOnUpdate((value) => cameraShake.m_AmplitudeGain = value);
    }

    private void OnDestroy()
    {
        GameManager.GameOverEvent -= CameraShake; // �ı��� �� �̺�Ʈ ���� �����ϰ� ����
    }
}
