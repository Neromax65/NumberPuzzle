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
        string[] indices = new string[Board.Tiles.GetLength(0) * Board.Tiles.GetLength(1)];
        int i = 0;
        for (int y = Board.Tiles.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < Board.Tiles.GetLength(0); x++)
            {
                indices[i++] = Board.Tiles[x, y].Index.ToString();
            }
        }

        IGameSaver<PlayerPrefsSaveInfo> gameSaver = new PlayerPrefsSaver(indices, Board.Columns, Board.Rows, MoveCount);
        gameSaver.Save();
    }

    /// <summary>
    /// Продолжить незаконченную игру
    /// </summary>
    public void ContinueGame()
    {
        IGameLoader<PlayerPrefsSaveInfo> gameLoader = new PlayerPrefsLoader();
        var loadedGameStateInfo = gameLoader.Load();
        
        GameSettings.BoardSize = new Vector2Int(loadedGameStateInfo.ColumnCount, loadedGameStateInfo.RowsCount);
        Board = new Board(GameSettings.BoardSize, loadedGameStateInfo.Indices.Select(int.Parse).ToArray());
        MoveCount = loadedGameStateInfo.MoveCount;
        InitializeBoard();
        CurrentState = GameState.WaitingForInput;
    }
    
    /// <summary>
    /// Начать новую игру
    /// </summary>
    public void NewGame()
    {
        Board = new Board(GameSettings.BoardSize);
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
        Vector2 tileSize = GameSettings.TileSize;
        Vector2 boardSize = GameSettings.BoardSize;
        GameSettings.Offset = new Vector2(tileSize.x/2 - boardSize.x / 2f * tileSize.x, tileSize.y/2 - boardSize.y / 2f * tileSize.y);
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