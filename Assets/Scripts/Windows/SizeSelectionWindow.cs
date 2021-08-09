using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Windows
{
    public class SizeSelectionWindow : WindowItem
    {
        [SerializeField] private Vector2Int[] sizes =
        {
            new Vector2Int(3,3),
            new Vector2Int(4,4),
            new Vector2Int(5,5)
        };
        [SerializeField] private Dropdown fieldSizeDropdown;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button backButton;

        private GameManager _gameManager;
        private WindowManager _windowManager;
        
        [Inject]
        private void Construct(GameManager gameManager, WindowManager windowManager)
        {
            _gameManager = gameManager;
            _windowManager = windowManager;
        }

        private void Awake()
        {
            startGameButton.onClick.AddListener(_gameManager.NewGame);            
            startGameButton.onClick.AddListener(base.Hide);            
            backButton.onClick.AddListener(_windowManager.MainMenuWindow.Show);            
            backButton.onClick.AddListener(base.Hide);            
        }

        private void Start()
        {
            GenerateDropdownItems();
        }

        private void GenerateDropdownItems()
        {
            fieldSizeDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var size in sizes)
            {
                options.Add($"{size.x}x{size.y}");
            }
            fieldSizeDropdown.AddOptions(options);
            ChangeBoardSize(0);
            fieldSizeDropdown.onValueChanged.AddListener(ChangeBoardSize);
        }
        
        private void ChangeBoardSize(int optionIndex)
        {
            _gameManager.gameSettings.boardSize = new Vector2Int(sizes[optionIndex].x, sizes[optionIndex].y);
        }
    }
}