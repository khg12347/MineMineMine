using System;
using System.Collections.Generic;

namespace MI.Core.ServiceLocator
{
    public static class MIServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        
        public static void Register<T> (T service) where T : class => _services[typeof(T)] = service;
        public static void Unregister<T>() where T : class => _services.Remove(typeof(T));

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
            throw new Exception($"Service of type {typeof(T)} not found.");
        }

        public static void Clear() => _services.Clear();
    }
}
