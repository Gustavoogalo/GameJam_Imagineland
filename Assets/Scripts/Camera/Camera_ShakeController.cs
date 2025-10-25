using System;
using Unity.Cinemachine;
using System.Collections;
using Helpers;
using UnityEngine;

namespace Camera
{
    public class Camera_ShakeController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera virtualCamera01;
        private CinemachineBasicMultiChannelPerlin perlinNoise01;

        public float startingIntensity = 1f;
        public float frequencyShake = 0.1f;
        public float shakeCounterTimer = 0.5f;
        private float shakeTimer = 0;

        private void Awake()
        {
            ServiceLocator.Register(this);
            perlinNoise01 = virtualCamera01.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        private void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;

                perlinNoise01.AmplitudeGain = startingIntensity;

                perlinNoise01.FrequencyGain = frequencyShake;
            }

            if (shakeTimer < 0)
            {
                shakeTimer = 0;
                perlinNoise01.AmplitudeGain = 0f;

                perlinNoise01.FrequencyGain = 0f;
            }
        }

        public void ShakeCamera(float intensity, float frequency, float shakeTime)
        {
            startingIntensity = intensity;
            frequencyShake = frequency;
            shakeTimer = shakeTime;
        }
    }
}