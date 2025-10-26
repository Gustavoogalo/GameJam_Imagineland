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
                var playerMov = inventoryplayer.GetComponent<Player_Moviment>();
                transform.SetParent(playerMov.carryPosition);
                transform.position = playerMov.carryPosition.position;
                //Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Collect(other.GetComponent<InventoryResource>());
            }
        }

        public void DestroyResource()
        {
            Destroy(gameObject);
        }
    }
}