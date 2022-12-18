using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// Модель тайла
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Координата X на доске
        /// </summary>
        public int X;
        
        /// <summary>
        /// Координата Y на доске
        /// </summary>
        public int Y;
        
        /// <summary>
        /// Порядковый номер тайла
        /// </summary>
        public readonly int Index;
        
        /// <summary>
        /// Событие о смене мест с другим тайлом, Tile передаёт сам тайл, а bool нужна для того, чтобы событие не
        /// дублировалось 
        /// </summary>
        public event Action<Tile, bool> Swapped;

        /// <summary>
        /// Событие уничтожения тайла
        /// </summary>
        public event Action Destroyed;

        public Tile(int index)
        {
            Index = index;
        }
        
        /// <summary>
        /// Поменяться местами с другим тайлом, триггерит событие
        /// </summary>
        /// <param name="otherTile">Другой тайл</param>
        public void Swap(Tile otherTile)
        {
            (X, otherTile.X) = (otherTile.X, X);
            (Y, otherTile.Y) = (otherTile.Y, Y);

            Swapped?.Invoke(this, false);
            otherTile.Swapped?.Invoke(otherTile, true);
        }
        
        /// <summary>
        /// Определяет, является ли текущий тайл смежным (кроме диагоналей) другому тайлу
        /// </summary>
        /// <param name="otherTile">Другой тайл</param>
        /// <returns>true - является смежным, false - не является смежным</returns>
        public bool IsAdjacentTo(Tile otherTile)
        {
            return Mathf.Abs(otherTile.X - X) + Mathf.Abs(otherTile.Y - Y) == 1;
        }

        /// <summary>
        /// Уничтожение тайла
        /// </summary>
        public void Destroy()
        {
            Swapped = null;
            Destroyed?.Invoke();
        }
        
        public override string ToString()
        {
            return $"Tile {Index} [X:{X} Y:{Y}]";
        }
    }
}