using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Windows
{
    public class VictoryWindow : WindowItem
    {
        [SerializeField] private Text moveCountText;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button exitButton;
        
        private GameManager _gameManager;
        private WindowManager _windowManager;
        
        [Inject]
        private void Construct(GameManager gameManager, WindowManager windowManager)
        {
            _gameManager = gameManager;
            _windowManager = windowManager;
        }

        public override void Show()
        {
            base.Show();
            moveCountText.text = _gameManager.MoveCount.ToString();
        }

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(_windowManager.MainMenuWindow.Show);
            mainMenuButton.onClick.AddListener(base.Hide);
            exitButton.onClick.AddListener(_gameManager.Exit);
        }
    }
}