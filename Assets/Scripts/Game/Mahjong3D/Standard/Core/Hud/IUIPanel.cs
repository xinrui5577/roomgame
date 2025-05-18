namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IUIPanelControl
    {
        void Close();
        void Open();
    }

    public interface IUIPanelControl<T> : IUIPanelControl
    {
        void Open(T args);
    }
}