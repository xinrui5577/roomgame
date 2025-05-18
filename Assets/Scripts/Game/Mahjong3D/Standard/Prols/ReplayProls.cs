namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 1,5:55 --> '1' : 玩家座位号 | '5' : 消息协议号 | '55' : 数据
    /// </summary>
    public class ReplayProls
    {
        public const int Allowcate = 1;
        public const int GetIn = 2;
        public const int ThrowOut = 3;
        public const int Chi = 4;   
        public const int Peng = 5;
        public const int GangZhua = 6;
        public const int GangMing = 7;
        public const int GangAn = 8;
        public const int Hu = 9;
        public const int Zimo = 10;
        public const int LiuJu = 11;
        public const int Laizi = 12;
        public const int XuanFengGang = 13;
        public const int JueTouGang = 14;
        public const int ZhaNiao = 15;
        public const int CaiGang = 16;
        public const int DanGang = 17;
        public const int Ting = 18;
        public const int NiuBiTing = 21;
        public const int HuanBao = 23;
        public const int HouKou = 24;
        public const int FanPai = 25;
        public const int BuZhang = 26;

        public const int DaiGu = 27;
        public const int YouJin = 27;

        //客户端自定义协议 游戏结束
        public const int GameOver = 10000;
    }
}
