namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CustomProl
    {
        //客戶端自用协议
        public const int CustomLogic = 100001;//自定义逻辑
        public const int ReconnectLogic = 100002;//重连
        public const int ReadyLogic = 100003;//准备
        public const int ResRollDice = 100004;//打骰子
        public const int GameOverLogic = 100005;//游戏结束

        //C2S
        public const int ReqRestartGame = 200001;//重新游戏开始
        public const int ReqUserReady = 200002;//玩家准备
        public const int ReqDismissRoom = 200003;//解散房间
        public const int ReqThrowoutCard = 200004;//出牌
        public const int ReqQueryting = 200005;//麻将查询听牌请求
        public const int ReqOpchi = 200006;
        public const int ReqOppeng = 200007;
        public const int ReqOpgang = 200008;
        public const int ReqOphu = 200009;
        public const int ReqOpting = 200010;
        public const int ReqOpguo = 200011;
        public const int ReqThrowoutTingCard = 200012;//打牌上听
        public const int ReqConfirmChange = 200013;//确定选牌
    }
}
