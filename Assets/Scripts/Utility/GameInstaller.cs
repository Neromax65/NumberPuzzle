using Windows;
using UnityEngine;
using Zenject;

namespace Utility
{
    /// <summary>
    /// Моноинсталлер Zenject для инициализации привязки зависимостей
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private WindowManager windowManager;
        [SerializeField] private TileView tileViewPrefab;
        [SerializeField] private Transform tilesParent;
    
        public override void InstallBindings()
        {
            Container.BindInstance(gameManager).AsSingle();
            Container.BindInstance(windowManager).AsSingle();
            Container.BindMemoryPool<TileView, TileView.Pool>().WithInitialSize(16).FromComponentInNewPrefab(tileViewPrefab).UnderTransform(tilesParent);
        }

        public override void Start()
        {
            base.Start();
            SetTileSize();
        }
    
        /// <summary>
        /// Получения размера тайла из префаба
        /// </summary>
        private void SetTileSize()
        {
            var tileRectTransform = tileViewPrefab.GetComponent<RectTransform>();
            gameManager.gameSettings.tileSize = tileRectTransform.sizeDelta * tileRectTransform.lossyScale;
        }
    }
}