using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple phonebook for shared game services.
/// Attach this to your Manager GameObject in the scene.
///
/// REGISTER (in Awake on the owning script):
///   ServiceLocator.Register<ITimeProvider>(this);
///
/// RESOLVE (anywhere else):
///   var tm = ServiceLocator.Get<ITimeProvider>();
/// </summary>
public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance { get; private set; }

    private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void Register<T>(T instance) where T : class
    {
        _services[typeof(T)] = instance;
        Debug.Log($"[ServiceLocator] Registered: {typeof(T).Name}");
    }

    public static T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
            return service as T;

        Debug.LogWarning($"[ServiceLocator] '{typeof(T).Name}' is not registered.");
        return null;
    }

    public static void Clear() => _services.Clear();
}
