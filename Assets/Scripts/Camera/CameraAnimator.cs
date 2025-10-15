using UnityEngine;
using Cinemachine;
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraAnimator : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Noise Profiles")]
    [SerializeField] private NoiseSettings idleNoise;   // <-- Cinemachine.NoiseSettings
    [SerializeField] private NoiseSettings walkNoise;
    [SerializeField] private NoiseSettings runNoise;
    [SerializeField] private NoiseSettings jumpNoise;
    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    void OnEnable()
    {
        PlayerEvents.OnPlayerRun += HandleRun;
        PlayerEvents.OnPlayerIdle += HandleIdle;
        PlayerEvents.OnPlayerWalk += HandleWalk;
        PlayerEvents.OnPlayerJump += HandleJump;
    }
    void OnDisable()
    {
        PlayerEvents.OnPlayerRun -= HandleRun;
        PlayerEvents.OnPlayerIdle -= HandleIdle;
        PlayerEvents.OnPlayerWalk -= HandleWalk;
        PlayerEvents.OnPlayerJump -= HandleJump;
    }
    private void HandleIdle()
    {
        noise.m_NoiseProfile = idleNoise;
    }

    private void HandleWalk()
    {
        noise.m_NoiseProfile = walkNoise;
    }

    private void HandleJump()
    {
        noise.m_NoiseProfile = jumpNoise;
    }
    private void HandleRun()
    {
        noise.m_NoiseProfile = runNoise;
    }

}
