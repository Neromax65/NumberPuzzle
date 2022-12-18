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
    }
}