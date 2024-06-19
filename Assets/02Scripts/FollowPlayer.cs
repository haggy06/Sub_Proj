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

        GameManager.Inst.GameOverEvent += CameraShake;
    }
    private void CameraShake(Obstacle obstacle)
    {
        LeanTween.value(obstacle.CameraShakeAmplitude, 0f, cameraShakeTime).setOnUpdate((value) => cameraShake.m_AmplitudeGain = value);
    }

    private void OnDestroy()
    {
        GameManager.Inst.GameOverEvent -= CameraShake; // �ı��� �� �̺�Ʈ ���� �����ϰ� ����
    }
}
