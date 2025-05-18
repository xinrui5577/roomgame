namespace Assets.Scripts.Game.mx97
{ 
    public enum Mx97EventType
    {
        /// <summary>
        ///  刷新押注分数
        /// </summary>
        RefreshBetScore,
        /// <summary>
        /// 刷新当前分数
        /// </summary>
        RefreshCurScore,
        /// <summary>
        /// 刷新奖池分数
        /// </summary>
        RefreshJackpot,
        /// <summary>
        /// 游戏开始时切换各种按钮状态
        /// </summary>
        SwitchBtnWhenStart,
        SwitchBtnWhenStop,
        /// <summary>
        /// 游戏开始时滚动水果
        /// </summary>
        StartScrollAni,
        /// <summary>
        /// 游戏开始时滚动水果
        /// </summary>
        StopScrollAni,
        /// <summary>
        /// 初始化中奖红框、红点、红线、分数信息
        /// </summary>
        InitGameResult,
        ShowGameResult,
        /// <summary>
        /// 改变每条线对应的分数值
        /// </summary>
        ChangeLineScore
    }
}

