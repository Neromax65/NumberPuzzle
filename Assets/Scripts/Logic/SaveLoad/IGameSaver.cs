namespace Logic
{
    public interface IGameSaver<out T> where T : ISaveInfo
    {
        T Save();
    }
}