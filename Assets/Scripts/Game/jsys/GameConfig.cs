namespace Assets.Scripts.Game.jsys
{
    public class GameConfig
    {
        /// <summary>
        /// 游戏状态类型
        /// </summary>
        public enum GoldSharkState
        {
            Bet = 0,
            Marquee = 1,
            Finish = 2
        }
        /// <summary>
        /// 游戏状态变量
        /// </summary>
        public int TurnTableState;
        /// <summary>
        /// 游戏赔率倍数数组
        /// </summary>
        public int[] Imultiplying= new int[12];

        /// <summary>
        /// 跑马灯闪烁间隔
        /// </summary>
        public float MarqueeInterval = 0.03f;

        /// <summary>
        ///游戏结果(跑马灯跑马次数) 
        /// </summary>
        public int TurnTableResult = 100;
        
        /// <summary>
        /// bonu 数
        /// </summary>
        public int BonuNumber = 0;

        public bool IsBetPanelOnShow = false;

        /// <summary>
        /// 是否中金鲨
        /// </summary>
        public bool IsGoldShark = false;       
        /// <summary>
        /// 是否中银鲨
        /// </summary>
        public bool IsSliverShark = false;


    }
}
