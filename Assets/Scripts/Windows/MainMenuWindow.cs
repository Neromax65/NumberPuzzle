using Helpers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Windows
{
    /// <summary>
    /// Окно главного меню
    /// </summary>
    public class MainMenuWindow: WindowItem
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newGameButton;
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
            continueButton.interactable = PlayerPrefs.HasKey(PlayerPrefsKeys.MoveCountKey);
        }

        private void Awake()
        {
            continueButton.onClick.AddListener(_gameManager.ContinueGame);
            continueButton.onClick.AddListener(base.Hide);
            newGameButton.onClick.AddListener(_windowManager.SelectionWindow.Show);
            newGameButton.onClick.AddListener(base.Hide);
            exitButton.onClick.AddListener(_gameManager.Exit);
        }
    }
}