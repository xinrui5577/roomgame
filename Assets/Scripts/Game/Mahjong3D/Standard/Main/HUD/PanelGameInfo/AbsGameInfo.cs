namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class AbsGameInfo : AbsSubPanel
    {
        public virtual void OnReadyRefresh() { }

        public virtual void OnGetInfoRefresh() { }

        public virtual void OnStartGameUpdate() { }

        public virtual void UpdateMahjongCount(GameInfoArgs args) { }
    }
}
