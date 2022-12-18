namespace Logic
{
    public interface IGameLoader<out T> where T : ISaveInfo
    {
        T Load();
    }
}