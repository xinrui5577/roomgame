namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 解散房间类型
    /// </summary>
    public enum DismissFeedBack
    {
        Refuse = -1,
        None = 0,
        ApplyFor = 2,
        Agree = 3,
    }

    /// <summary>
    /// 锚点类型
    /// </summary>
    public enum Anchor
    {
        /// <summary>
        /// 正上方
        /// </summary>
        MarginTop,
        //上
        TopLeft,
        TopCenter,
        TopRight,
        //中
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        //下
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    /// <summary>
    /// 相对本家，其他玩家的座位
    /// </summary>
    public enum RelativeSeat
    {
        None = 0,
        /// <summary>
        /// 下家
        /// </summary>
        Behind = 1,
        /// <summary>
        /// 对家
        /// </summary>
        Opposite = 2,
        /// <summary>
        /// 上家
        /// </summary>
        Front = 3,
    }

    //CPG类型
    public enum EnGroupType
    {
        Chi = 1,
        Peng = 2,
        WZhuaGang = 3,
        /// <summary>
        /// 先碰，后杠
        /// </summary>
        ZhuaGang = 4,
        /// <summary>
        /// 直接杠别人的
        /// </summary>
        PengGang = 5,
        /// <summary>
        /// 明杠
        /// </summary>
        MingGang = 6,
        /// <summary>
        /// 暗杠
        /// </summary>
        AnGang = 7,
        /// <summary>
        /// 旋风杠
        /// </summary>
        XFGang = 10,
        /// <summary>
        /// 绝杠 -暗杠
        /// </summary>
        AnJueGang = 0xb,
        /// <summary>
        /// 绝杠
        /// </summary>
        JueGang = 0xd,
        /// <summary>
        /// 旋风杠，以下四条长春麻将专用
        /// </summary>
        XiaoJi = 100,
        YaoDan = 101,
        JiuDan = 102,
        ZFBDan = 103,
        XFDan = 104,
        None,
    }

    /// <summary>
    /// 牌值枚举
    /// </summary>
    public enum MahjongValue
    {
        None = 0,
        Wan_1 = 17,
        Wan_2,
        Wan_3,
        Wan_4,
        Wan_5,
        Wan_6,
        Wan_7,
        Wan_8,
        Wan_9,
        Tiao_1 = 33,
        Tiao_2,
        Tiao_3,
        Tiao_4,
        Tiao_5,
        Tiao_6,
        Tiao_7,
        Tiao_8,
        Tiao_9,
        Bing_1 = 49,
        Bing_2,
        Bing_3,
        Bing_4,
        Bing_5,
        Bing_6,
        Bing_7,
        Bing_8,
        Bing_9,
        Dong = 65,
        Nan = 68,
        Xi = 71,
        Bei = 74,
        Zhong = 81,
        Fa = 84,
        Bai = 87,
        //花牌
        ChunF = 96,
        XiaF,
        QiuF,
        DongF,
        MeiF,
        LanF,
        ZuF,
        JuF,
        //苏州麻将
        Other = 7 << 4,
        Laoshu = Other + 1,
        Mao = Other + 4,
        Caishen = Other + 7,
        Jubao = Other + 10,
        //百搭
        Baida = (8 << 4) + 1,
        //大白板
        BigBai = Baida + 4,
    }
}
