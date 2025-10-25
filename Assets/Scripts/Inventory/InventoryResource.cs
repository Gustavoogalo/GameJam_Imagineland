using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq; 

namespace Inventory
{
    public enum ResourceType
    {
       tipo1,
       tipo2,
    }
    public class InventoryResource : MonoBehaviour
    {
        private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
        
        public event Action<ResourceType, int> OnResourceChanged;

        [Header("Debug/Initial Resources")]
        [SerializeField] private List<ResourceData> initialResources = new List<ResourceData>();

        [Serializable]
        public struct ResourceData
        {
            public ResourceType type;
            public int amount;
        }

        private void Awake()
        {
           
            foreach (ResourceData data in initialResources)
            {
                resources[data.type] = data.amount;
            }
        }


        public void AddResource(ResourceType type, int amount)
        {
            if (amount <= 0) return;

            if (resources.ContainsKey(type))
            {
                resources[type] += amount;
            }
            else
            {
                resources.Add(type, amount);
            }
            
            OnResourceChanged?.Invoke(type, resources[type]);
        }

        public bool RemoveResource(ResourceType type, int amount)
        {
            if (amount <= 0) return false;

            if (resources.ContainsKey(type) && resources[type] >= amount)
            {
                resources[type] -= amount;
                OnResourceChanged?.Invoke(type, resources[type]);
                return true;
            }

            return false;
        }

        public int GetResourceAmount(ResourceType type)
        {
            return resources.ContainsKey(type) ? resources[type] : 0;
        }

       
        public void TransferAllResources(InventoryResource recipient)
        {
            if (recipient == null) return;
            
           
            List<ResourceType> typesToTransfer = resources.Keys.ToList();

            foreach (ResourceType type in typesToTransfer)
            {
                int amountToTransfer = resources[type];

                if (amountToTransfer > 0)
                {
                    resources[type] = 0;
                    OnResourceChanged?.Invoke(type, 0);

              
                    recipient.AddResource(type, amountToTransfer);
                    
                    Debug.Log($"Transferido {amountToTransfer} de {type} para a Base.");
                }
            }
        }
        
     
        public bool GetStolenResource(ResourceType type, int amount)
        {
        
            return RemoveResource(type, amount);
        }
    }
}