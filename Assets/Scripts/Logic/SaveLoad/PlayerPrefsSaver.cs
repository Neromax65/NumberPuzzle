using Helpers;
using UnityEngine;

namespace Logic
{
    public class PlayerPrefsSaver : IGameSaver<PlayerPrefsSaveInfo>
    {
        private readonly PlayerPrefsSaveInfo _saveInfo;
        
        public PlayerPrefsSaver(PlayerPrefsSaveInfo saveInfo)
        {
            _saveInfo = saveInfo;
        }
        
        public PlayerPrefsSaver(string[] indices, int columnCount, int rowCount, int moveCount)
        {
            _saveInfo = new PlayerPrefsSaveInfo
            {
                Indices = indices,
                MoveCount = moveCount,
                ColumnCount = columnCount,
                RowsCount = rowCount
            };
        }
        
        public PlayerPrefsSaveInfo Save()
        {
            PlayerPrefs.SetString(PlayerPrefsKeys.IndicesKey, string.Join(",", _saveInfo.Indices));
            PlayerPrefs.SetInt(PlayerPrefsKeys.MoveCountKey, _saveInfo.MoveCount);
            PlayerPrefs.SetInt(PlayerPrefsKeys.ColumnsKey, _saveInfo.ColumnCount);
            PlayerPrefs.SetInt(PlayerPrefsKeys.RowsKey, _saveInfo.RowsCount);
            return _saveInfo;
        }
    }
}