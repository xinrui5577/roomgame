namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class AbsCommandAction
    {
        public virtual void OnInit() { }
        public virtual void OnReset() { }

        private MahjongSceneComponent mGame;
        private DataCenterComponent mDataCenter;

        protected MahjongLocalConfig Config { get { return DataCenter.Config; } }
        protected MahjongConfigData ConfigData { get { return DataCenter.ConfigData; } }
        protected MahjongSceneComponent Game { get { if (null == mGame) { mGame = GameCenter.Scene; } return mGame; } }
        protected DataCenterComponent DataCenter { get { if (null == mDataCenter) { mDataCenter = GameCenter.DataCenter; } return mDataCenter; } }
    }
}
