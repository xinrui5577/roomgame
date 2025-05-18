namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>     
    /// 手牌控制状态     
    /// </summary>
    public enum HandcardStateTyps
    {
        None = -1,
        Normal,
        Ting,
        ChooseTingCard,
        Daigu,
        NiuTing,
        ChooseNiuTing,
        SingleHu,//一个玩家胡牌之后游戏不结束
        ExchangeCards,//换牌  
        Dingqueing,//定缺中   
        DingqueOver,//定缺结束
        TingAndShowCard,
        Youjin,
        Fenggang,//dbsmj 风杠
    }
}