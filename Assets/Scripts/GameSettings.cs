using System;
using UnityEngine;

/// <summary>
/// Класс, хранящий игровые настройки
/// </summary>
public static class GameSettings
{
    /// <summary>
    /// Размер игрового поля
    /// </summary>
    public static Vector2Int BoardSize = new Vector2Int(4,4);
    
    /// <summary>
    /// Размер тайла
    /// </summary>
    public static Vector2 TileSize = new Vector2(100,100);
    
    /// <summary>
    /// Смещение игрового поля относительно количества тайлов
    /// </summary>
    public static Vector2 Offset = Vector2.zero;
}