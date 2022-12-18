using System;
using System.Collections;
using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Графическая составляющая тайла
/// </summary>
public class TileView : MonoBehaviour, IPointerClickHandler
{
    private const float SwapAnimationTime = 0.2f;
    private const float ShuffleAnimationTime = 0.1f;
    
    /// <summary>
    /// Событие об окончании анимации перемещения тайла
    /// </summary>
    public event Action<bool> AnimationPlayed;
    
    /// <summary>
    /// Событие клика на тайл
    /// </summary>
    public event Action Clicked;
    
    [SerializeField] private Image tileImage;
    [SerializeField] private Text tileNumberText;

    private GameManager _gameManager;
    private Tile _tile;
    
    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
    /// <summary>
    /// Инициализация и привязка к модели
    /// </summary>
    /// <param name="tile">Модель тайла</param>
    /// <param name="empty">Должен ли этот тайл быть пустым</param>
    public void Init(Tile tile, bool empty = false)
    {
        _tile = tile;
        tile.Swapped += UpdateWorldPosition;
        transform.localPosition = GetWorldCoords(tile);
        tileNumberText.text = $"{_tile.Index+1}";
#if UNITY_EDITOR
        gameObject.name = _tile.ToString();
#endif
        if (empty)
        {
            tileImage.color = new Color(tileImage.color.r, tileImage.color.g, tileImage.color.b, 0);
            tileNumberText.enabled = false;
        }
    }

    /// <summary>
    /// Обновить позицию в мировых координатах с учётом смещения
    /// </summary>
    /// <param name="tile">Модель тайла</param>
    /// <param name="lastTile">Последний ли тайл</param>
    private void UpdateWorldPosition(Tile tile, bool lastTile)
    {
        float time = _gameManager.CurrentState == GameManager.GameState.ShufflingAnimation
            ? ShuffleAnimationTime
            : SwapAnimationTime;
        transform.DOLocalMove(GetWorldCoords(_tile), time).SetEase(Ease.Linear).OnComplete(() => AnimationPlayed?.Invoke(lastTile));
        // StartCoroutine(UpdateWorldPositionCoroutine(lastTile, time));
#if UNITY_EDITOR
        gameObject.name = _tile.ToString();
#endif
    }

    /// <summary>
    /// Корутина обновления позиции
    /// </summary>
    /// <param name="lastTile">Последний ли тайл</param>
    /// <param name="time">Время анимации</param>
    /// <returns></returns>
    private IEnumerator UpdateWorldPositionCoroutine(bool lastTile, float time)
    {
        Vector2 startPosition = transform.localPosition;
        var targetPosition = GetWorldCoords(_tile);

        if (time > 0)
            for (float t = 0; t < 1; t += Time.deltaTime * 1 / time)
            {
                transform.localPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        transform.localPosition = targetPosition;

        var eventHandler = AnimationPlayed;
        eventHandler?.Invoke(lastTile);
    }
    
    /// <summary>
    /// Обработка клика на тайл
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_gameManager.CurrentState != GameManager.GameState.WaitingForInput) return;

        var emptyTile = _gameManager.Board.EmptyTile;
        if (_tile != emptyTile && _tile.IsAdjacentTo(emptyTile))
        {
            var eventHandler = Clicked;
            eventHandler?.Invoke();
            _tile.Swap(emptyTile);
        }
    }

    /// <summary>
    /// Получить мировые координаты с учётом смещения
    /// </summary>
    /// <param name="x">Координата X игрового поля</param>
    /// <param name="y">Координата Y игрового поля</param>
    /// <returns></returns>
    private Vector2 GetWorldCoords(int x, int y)
    {
        float worldX = x * GameSettings.TileSize.x + GameSettings.Offset.x;
        float worldY = y * GameSettings.TileSize.y + GameSettings.Offset.y;
        return new Vector2(worldX, worldY);
    }
    
    /// <summary>
    /// Перегруженный метод получения мировых координат
    /// </summary>
    /// <param name="tile">Тайл</param>
    /// <returns></returns>
    private Vector2 GetWorldCoords(Tile tile)
    {
        return GetWorldCoords(tile.X, tile.Y);
    }
    
    /// <summary>
    /// Пул для создания графического объекта тайла
    /// </summary>
    public class Pool : MonoMemoryPool<TileView>
    {
        protected override void Reinitialize(TileView item)
        {
            base.Reinitialize(item);
            item.tileImage.color = new Color(item.tileImage.color.r, item.tileImage.color.g, item.tileImage.color.b, 1);
            item.transform.localPosition = Vector3.zero;
            item.tileNumberText.enabled = true;
            item.AnimationPlayed = null;
            item.Clicked = null;
        }
    }
}
