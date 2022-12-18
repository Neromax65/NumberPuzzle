using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Logic
{
    /// <summary>
    /// Класс игрового поля
    /// </summary>
    public class Board
    {
        public readonly int Columns;
        public readonly int Rows;
        
        /// <summary>
        /// Двумерный массив, хранящий все тайлы, индексы являются координатами
        /// </summary>
        public readonly Tile[,] Tiles;
        
        /// <summary>
        /// Ссылка на пустой тайл
        /// </summary>
        public readonly Tile EmptyTile;
        
        /// <summary>
        /// Событие об окончании перемешивания
        /// </summary>
        public event Action ShuffleEnded;
        
        /// <summary>
        /// Идет ли сейчас перемешивание какого-либо тайла
        /// </summary>
        private bool _isTileShuffling;

        /// <summary>
        /// Конструктор для новой игры
        /// </summary>
        /// <param name="boardSize">Размер игрового поля</param>
        public Board(Vector2Int boardSize)
        {
            Tiles = new Tile[boardSize.x, boardSize.y];
            Columns = boardSize.x;
            Rows = boardSize.y;
            int i = 0;
            for (int y = Rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Tiles[x, y] = new Tile(i++)
                    {
                        X = x,
                        Y = y
                    };
                    if (Tiles[x, y].Index == boardSize.x * boardSize.y - 1)
                        EmptyTile = Tiles[x, y];
                    Tiles[x, y].Swapped += UpdateIndices;
                }
            }
        }
        
        /// <summary>
        /// Конструктор для загружаемой игры
        /// </summary>
        /// <param name="boardSize">Размер игрового поля</param>
        /// <param name="indices">Загруженные индексы</param>
        public Board(Vector2Int boardSize, int[] indices)
        {
            Tiles = new Tile[boardSize.x, boardSize.y];
            Columns = boardSize.x;
            Rows = boardSize.y;
            int i = 0;
            for (int y = Rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Tiles[x, y] = new Tile(indices[i++])
                    {
                        X = x,
                        Y = y
                    };
                    if (Tiles[x, y].Index == boardSize.x * boardSize.y - 1)
                        EmptyTile = Tiles[x, y];
                    Tiles[x, y].Swapped += UpdateIndices;
                }
            }
        }

        /// <summary>
        /// Обновить индексы при смене координат у тайла
        /// </summary>
        /// <param name="tile">Тайл, который изменил координаты</param>
        /// <param name="lastTile">Последний ли тайл</param>
        private void UpdateIndices(Tile tile, bool lastTile)
        {
            Tiles[tile.X, tile.Y] = tile;
        }
        
        /// <summary>
        /// Проверка выполнения условия победы
        /// </summary>
        /// <returns>true - условие выполнено, false - условие не выполнено</returns>
        public bool CheckForWinCondition()
        {
            
            int lastIndex = 0;
            for (int y = Rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int tilesIndex = Tiles[x, y].Index;
                    if (tilesIndex < lastIndex) return false;
                    lastIndex = tilesIndex;
                }
            }
            return true;
        }

        /// <summary>
        /// Перемешать тайлы на игровом поле
        /// </summary>
        /// <returns></returns>
        public IEnumerator Shuffle()
        {
            Random random = new Random();
            Tile lastShuffledTile = null;

            int shuffleTimes = Columns * Rows * 3;
            do
            {
                for (int i = 0; i < shuffleTimes; i++)
                {
                    ShuffleOnce(random, ref lastShuffledTile); 
                    yield return new WaitUntil(() => !_isTileShuffling);
                }
                
            } while (CheckForWinCondition());
            ShuffleEnded?.Invoke();
        }

        /// <summary>
        /// Перемешать единожды
        /// </summary>
        /// <param name="random">Ссылка на экземпляр рандома</param>
        /// <param name="lastShuffledTile">Ссылка на последний перемешиваемый тайл</param>
        void ShuffleOnce(Random random, ref Tile lastShuffledTile)
        {
            var adjacentTiles = GetAdjacentTiles(EmptyTile);
            if (lastShuffledTile != null && adjacentTiles.Contains(lastShuffledTile))
                adjacentTiles.Remove(lastShuffledTile);
            var tileToShuffle = adjacentTiles[random.Next(adjacentTiles.Count)];
            tileToShuffle.Swap(EmptyTile);
            lastShuffledTile = tileToShuffle;
            _isTileShuffling = true;
        }

        /// <summary>
        /// Сигнал для продолжения перемешивания
        /// </summary>
        public void ShuffleNext()
        {
            _isTileShuffling = false;
        }


        /// <summary>
        /// Получить смежные тайлы (не считая диагонали)
        /// </summary>
        /// <param name="middleTile">Центральный тайл</param>
        /// <returns>Список смежных тайлов</returns>
        private List<Tile> GetAdjacentTiles(Tile middleTile)
        {
            var adjacentTiles = new List<Tile>(4);
            if (middleTile.X < Columns - 1 && Tiles[middleTile.X + 1, middleTile.Y] != null)
                adjacentTiles.Add(Tiles[middleTile.X + 1, middleTile.Y]);

            if (middleTile.X > 0 && Tiles[middleTile.X - 1, middleTile.Y] != null)
                adjacentTiles.Add(Tiles[middleTile.X - 1, middleTile.Y]);

            if (middleTile.Y < Rows - 1 && Tiles[middleTile.X, middleTile.Y + 1] != null)
                adjacentTiles.Add(Tiles[middleTile.X, middleTile.Y + 1]);

            if (middleTile.Y > 0 && Tiles[middleTile.X, middleTile.Y - 1] != null)
                adjacentTiles.Add(Tiles[middleTile.X, middleTile.Y - 1]);

            return adjacentTiles;
        }
    }
}