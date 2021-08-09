using System;
using System.Linq;
using Windows;
using Helpers;
using Logic;
using UnityEngine;
using Zenject;

/// <summary>
/// Класс, управляющий игровым процессом и настройками
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Состояние игры:
    /// ShufflingAnimation - Идет стадия анимации перемешивания
    /// WaitingForInput - Ожидание ввода игрока
    /// SwapAnimation - Идёт анимация движения тайлов
    /// </summary>
    public enum GameState { ShufflingAnimation, WaitingForInput, SwapAnimation }
    
    /// <summary>
    /// Текущее состояние игры
    /// </summary>
    public GameState CurrentState { get;  private set; }
    
    /// <summary>
    /// Игровое поле
    /// </summary>
    public Board Board { get; private set; }
    
    /// <summary>
    /// Количество ходов, которое совершил игрок
    /// </summary>
    public int MoveCount { get; private set; }
    
    /// <summary>
    /// Игровые настройки
    /// </summary>
    public GameSettings gameSettings;
    
    /// <summary>
    /// Пул для создания графических объектов для тайлов
    /// </summary>
    private TileView.Pool _tileViewPool;
    
    /// <summary>
    /// Ссылка на менеджер окон
    /// </summary>
    private WindowManager _windowManager;

    [Inject]
    public void Construct(TileView.Pool tileViewPool, WindowManager windowManager)
    {
        _tileViewPool = tileViewPool;
        _windowManager = windowManager;
    }

    /// <summary>
    /// Реакция на событие окончания анимации движения тайла
    /// </summary>
    /// <param name="lastTile">Был ли это последний тайл</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    void OnTileAnimationPlayed(bool lastTile)
    {
        if (!lastTile) return;
            
        switch (CurrentState)
        {
            case GameState.ShufflingAnimation:
                Board.ShuffleNext();
                break;
            case GameState.SwapAnimation:
                MoveCount++;
                if (Board.CheckForWinCondition())
                {
                    foreach (var gridTile in Board.Tiles)
                    {
                        gridTile.Destroy();
                    }
                    _windowManager.VictoryWindow.Show();
                    MoveCount = 0;
                    PlayerPrefs.DeleteAll();
                    return;
                }
                SaveGameState();
                CurrentState = GameState.WaitingForInput;
                break;
            default:
                throw new ArgumentOutOfRangeException(CurrentState.ToString());
        }
    }

    /// <summary>
    /// Сохранить состояние игры
    /// </summary>
    private void SaveGameState()
    {
        string[] indices = new string[Board.Tiles.GetLength(0) * Board.Tiles.GetLength(0)];
        int i = 0;
        for (int y = Board.Tiles.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < Board.Tiles.GetLength(0); x++)
            {
                indices[i++] = Board.Tiles[x, y].Index.ToString();
            }
        }
        PlayerPrefs.SetString(PlayerPrefsKeys.IndicesKey, string.Join(",", indices));
        PlayerPrefs.SetInt(PlayerPrefsKeys.MoveCountKey, MoveCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.ColumnsKey, Board.Columns);
        PlayerPrefs.SetInt(PlayerPrefsKeys.RowsKey, Board.Rows);
    }

    /// <summary>
    /// Загрузить состояние игры
    /// </summary>
    /// <returns>Массив индексов тайлов</returns>
    private int[] LoadGameState()
    {
        string[] indices = PlayerPrefs.GetString(PlayerPrefsKeys.IndicesKey).Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);
        return indices.Select(int.Parse).ToArray();
    }
    
    /// <summary>
    /// Продолжить незаконченную игру
    /// </summary>
    public void ContinueGame()
    {
        gameSettings.boardSize = new Vector2Int(PlayerPrefs.GetInt(PlayerPrefsKeys.ColumnsKey), PlayerPrefs.GetInt(PlayerPrefsKeys.RowsKey));
        Board = new Board(gameSettings.boardSize, LoadGameState());
        MoveCount = PlayerPrefs.GetInt(PlayerPrefsKeys.MoveCountKey);
        InitializeBoard();
        CurrentState = GameState.WaitingForInput;
    }
    
    /// <summary>
    /// Начать новую игру
    /// </summary>
    public void NewGame()
    {
        Board = new Board(gameSettings.boardSize);
        InitializeBoard();
        CurrentState = GameState.ShufflingAnimation;
        Board.ShuffleEnded += () => CurrentState = GameState.WaitingForInput;
        StartCoroutine(Board.Shuffle());
    }

    /// <summary>
    /// Инициализация игрового поля
    /// </summary>
    private void InitializeBoard()
    {
        Vector2 tileSize = gameSettings.tileSize;
        Vector2 boardSize = gameSettings.boardSize;
        gameSettings.offset = new Vector2(tileSize.x/2 - boardSize.x / 2f * tileSize.x, tileSize.y/2 - boardSize.y / 2f * tileSize.y);
        foreach (var tile in Board.Tiles)
        {
            var tileView = _tileViewPool.Spawn();
            tileView.AnimationPlayed += OnTileAnimationPlayed;
            tileView.Clicked += () => CurrentState = GameState.SwapAnimation;
            tile.Destroyed += () => _tileViewPool.Despawn(tileView);
            tileView.Init(tile, tile == Board.EmptyTile);
        }
    }
        
    /// <summary>
    /// Выход из игры
    /// </summary>
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}