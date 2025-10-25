using System;
using System.Collections;
using Camera;
using Helpers;
using Inimigo;
using Player;
using UnityEngine;

namespace HealthM
{
    public class HealthManager : MonoBehaviour
    {
        private HealthSystem _healthSystem;
        private Animator _animator;
        [SerializeField] private GameObject _vfxPrefab;
        private Camera_ShakeController cameraShake;
        private EnemyManager _enemyManager;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _healthSystem = GetComponent<HealthSystem>();
            _enemyManager = GetComponent<EnemyManager>();
        }

        private void OnEnable()
        {
            _healthSystem.OnDeath += OnDamaged;
        }

        private void OnDisable()
        {
            _healthSystem.OnDeath -= OnDamaged;
        }

        private void OnDamaged(HealthSystem death)
        {
            StartCoroutine(Dying());
        }

        private void Start()
        {
            if (cameraShake == null)
            {
                cameraShake = ServiceLocator.Get<Camera_ShakeController>();
            }
        }

        IEnumerator Dying()
        {
           _animator.Play("EnemyDeath");
           _enemyManager.Death();
            yield return new WaitForSeconds(0.2f);
            // if (_vfxPrefab != null)
            // {
            //     _vfxPrefab.SetActive(true);
            // }
            cameraShake.ShakeCamera(5f, 0.5f, 0.1f);
            yield return new WaitForSeconds(0.1f);
            Destroy(this.gameObject);
        }
    }
}