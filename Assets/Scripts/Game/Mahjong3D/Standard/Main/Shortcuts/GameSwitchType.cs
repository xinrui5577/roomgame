namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 游戏控制开关
    /// </summary>
    public enum GameSwitchType
    {
        AiAgency = 1 << 1,//托管
        HasBuzhang = 1 << 2,//有补张
        PowerAiAgency = 1 << 3,//是否允许开启托管
    }
}