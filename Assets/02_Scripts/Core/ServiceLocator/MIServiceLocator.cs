using System;
using System.Collections.Generic;

namespace MI.Core.ServiceLocator
{
    public static class MIServiceLocator
    {
        private static readonly Dictionary<Type, object> s_services = new Dictionary<Type, object>();
        
        public static void Register<T> (T service) where T : class => s_services[typeof(T)] = service;
        public static void Unregister<T>() where T : class => s_services.Remove(typeof(T));

        public static T Get<T>() where T : class
        {
            if (s_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        public static void Clear() => s_services.Clear();
    }
}
