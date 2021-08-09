using System;
using UnityEngine;

/// <summary>
/// Класс, хранящий игровые настройки
/// </summary>
[Serializable]
public class GameSettings
{
    /// <summary>
    /// Размер игрового поля
    /// </summary>
    public Vector2Int boardSize = new Vector2Int(4,4);
    
    /// <summary>
    /// Размер тайла
    /// </summary>
    public Vector2 tileSize = new Vector2(100,100);
    
    /// <summary>
    /// Смещение игрового поля относительно количества тайлов
    /// </summary>
    public Vector2 offset = Vector2.zero;
    
    /// <summary>
    /// Время анимации перемещения тайла при перемешивании
    /// </summary>
    public float shuffleTileTime = 0.2f;
    
    /// <summary>
    /// Время анимации перемещения тайла при вводе игрока
    /// </summary>
    public float swapTileTime = 0.5f;
}