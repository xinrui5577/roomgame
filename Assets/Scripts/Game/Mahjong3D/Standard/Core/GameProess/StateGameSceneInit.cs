namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameSceneInit : FsmState
    {
        private bool mFlag;

        public override void OnEnter(FsmStateArgs args)
        {
            //执行继承ISceneInitCycle接口脚本
            GameCenter.Lifecycle.SceneInitCycle();
            if (GameCenter.Instance.GameType == GameType.Normal)
            {
                //开启网络监听
                GameCenter.Network.StartNetworkListener();
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}