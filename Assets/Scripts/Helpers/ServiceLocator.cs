using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogWarning($"Servi�o do typo {type.Name} j� registrado. Sobrescrevendo");
                services[type] = service;
            }
            else
            {
                Debug.Log($"Registrando servi�o do tipo {type.Name}");
                services.Add(type, service);
            }
        }

        public static T Get<T>()
        {
            Type type = typeof(T);
            if(!services.TryGetValue(type, out var service))
            {
                throw new Exception($"Servi�o do tipo {type.Name} n�o registrado.");
            }
            return (T)service;
        }

        public static void Unregister<T>(T service)
        {
            Type type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
                Debug.Log($"Servi�o do tipo {type.Name} desregistrado.");
            }
            else
            {
                Debug.LogWarning($"Servi�o do tipo {type.Name} n�o encontrado para desregistro.");
            }
        }

        public static void UnregisterAll()
        {
            services.Clear();
            Debug.Log("Todos os servi�os foram desregistrados.");
        }

    }
}