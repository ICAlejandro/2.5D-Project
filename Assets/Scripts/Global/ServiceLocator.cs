using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple phonebook for shared game services.
/// Instead of searching the whole scene with FindFirstObjectByType,
/// scripts register themselves here once and others look them up instantly.
///
/// REGISTER (in Awake on the owning script):
///   ServiceLocator.Register<TimeManager>(this);
///
/// RESOLVE (anywhere else):
///   var tm = ServiceLocator.Get<TimeManager>();
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void Register<T>(T instance) where T : class
    {
        _services[typeof(T)] = instance;
    }

    public static T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
            return service as T;

        Debug.LogWarning($"[ServiceLocator] '{typeof(T).Name}' is not registered.");
        return null;
    }

    /// <summary>Call this on scene unload to avoid stale references between scenes.</summary>
    public static void Clear() => _services.Clear();
}
