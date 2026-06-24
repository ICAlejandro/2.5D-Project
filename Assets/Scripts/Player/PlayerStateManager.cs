using System;
using UnityEngine;

/// <summary>
/// Tracks the current player state so different systems
/// can lock/unlock movement independently without conflicting.
///
/// USAGE:
///   Lock:   PlayerStateManager.SetState(PlayerState.Dialogue);
///   Unlock: PlayerStateManager.SetState(PlayerState.Free);
///   Check:  PlayerStateManager.IsState(PlayerState.Free);
/// </summary>
public enum PlayerState
{
    Free,       // Normal movement, full control
    Dialogue,   // Locked by a dialogue or option UI
    Cutscene,   // Locked by a cutscene (camera, animation, etc.)
}

public static class PlayerStateManager
{
    private static PlayerState _currentState = PlayerState.Free;

    /// <summary>Fires whenever the state changes.</summary>
    public static event Action<PlayerState> OnStateChanged;

    public static PlayerState CurrentState => _currentState;

    public static void SetState(PlayerState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        Debug.Log($"[PlayerState] → {newState}");
        OnStateChanged?.Invoke(newState);
    }

    public static bool IsState(PlayerState state) => _currentState == state;

    public static bool IsFree => _currentState == PlayerState.Free;
}
