namespace Logic
{
    public class PlayerPrefsSaveInfo : ISaveInfo
    {
        public string[] Indices { get; set; }
        public int MoveCount { get; set; }
        public int ColumnCount { get; set; }
        public int RowsCount { get; set; }
    }
}