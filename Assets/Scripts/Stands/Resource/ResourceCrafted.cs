using System;
using UnityEngine;

namespace Stands.Resource
{
    public class ResourceCrafted : MonoBehaviour
    {
        public event Action<ResourceCrafted> ResourceCollected;

        public void Collect()
        {
            ResourceCollected?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }
    }
}