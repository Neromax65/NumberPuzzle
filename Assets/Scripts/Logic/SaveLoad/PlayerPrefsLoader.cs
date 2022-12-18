using System;
using Helpers;
using UnityEngine;

namespace Logic
{
    public class PlayerPrefsLoader : IGameLoader<PlayerPrefsSaveInfo>
    {
        public PlayerPrefsSaveInfo Load()
        {
            int columns = PlayerPrefs.GetInt(PlayerPrefsKeys.ColumnsKey);
            int rows = PlayerPrefs.GetInt(PlayerPrefsKeys.RowsKey);
            int moveCount = PlayerPrefs.GetInt(PlayerPrefsKeys.MoveCountKey);
            string[] indices = PlayerPrefs.GetString(PlayerPrefsKeys.IndicesKey)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return new PlayerPrefsSaveInfo
            {
                Indices = indices,
                MoveCount = moveCount,
                ColumnCount = columns,
                RowsCount = rows
            };
        }
    }
}