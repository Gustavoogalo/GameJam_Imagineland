using System;
using Inventory;
using UnityEngine;

namespace Stands.Resource
{
    public class ResourceCrafted : MonoBehaviour
    {
        public ResourceType resourceType;
        public event Action<ResourceCrafted> ResourceCollected;

        public void Collect(InventoryResource inventoryplayer)
        {
            if (inventoryplayer != null)
            {
                inventoryplayer.AddResource(resourceType, 1);
                ResourceCollected?.Invoke(this);
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Collect(other.GetComponent<InventoryResource>());
            }
        }
    }
}