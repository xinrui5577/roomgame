namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NetworkProls
    {
        public const int SelectPiao = 0x1;//弹出选择漂按钮
        public const int ShowPiao = 0x2;//前台玩家选择漂、后台发送展示玩家选择的漂
        public const int ShowRate = 0x3;//游戏底注 
        public const int CPG = 0x50;//普通回应，表示玩家受到服务器通知或者做了取消操作  
        public const int CPGPengGang = 5;
        public const int AlloCate = 0x11;//分牌
        public const int GetInCard = 0x12;//玩家抓牌  
        public const int ThrowoutCard = 0x13;//玩家打牌 
        public const int ZiMo = 0x15;//自摸 
        public const int OpreateType = 0x18;//用户操作吃碰杠胡
        public const int Bao = 23;//宝
        public const int Ting = 0x51;//Ting
        public const int Hu = 0x54;
        public const int LiangDao = 0x14;//亮牌
        public const int Zimo = 0x15;//玩家自摸 
        public const int MoreTime = 0x16;//申请更多时间 
        public const int AllowResponse = 0x21;//服务器允许玩家请求
        public const int QiangGangHu = 0x23;//询问客户端胡不胡这个牌（玩家杠的时候，看看别人能不能抢杠胡） 
        public const int CheckCards = 0x40;
        public const int SelfGang = 0x55;//自己抓牌后 可以杠胡的
        public const int CPGZhuaGang = 4;
        public const int CPGMingGang = 6;
        public const int CPGAnGang = 7;
        public const int CPGLaiziGang = 8;
        public const int CPGXFG = 0x56;//旋风杠       
        public const int LastCd = 0x5A;//流局
        public const int GetNeedCard = 0x20;
        public const int BuZhang = 0x30;//补张
        public const int BuZhangFinish = 0x31;
        public const int BuZhangGetIn = 0x32;
        public const int GetHuCards = 0x5E;//查询胡牌
        public const int Dan = 0x5F;
        public const int ChangeCards = 0x60;//开始换张      
        public const int RotateCards = 0x61;//开始旋转 
        public const int SelectColor = 0x64;//服务器通知开始选花色 
        public const int SelColorRst = 0x65;//把选中的花色发送给服务器       
        public const int GameResult = 0x66;//血战血流玩法的最后胡牌

        public const int RequestStartTing = 0x69;//起手报听
        public const int ReponeseStartTing = 0x70;

        public const int JiaMa = 0x67;//加码
        public const int JiaMaFinish = 0x68;
        public const int JueGang = 0x1a;//绝杠
        public const int CPGAnJuegang = 0xb;
        public const int CPGLigang = 0x71;//立杠     
        public const int FenZhang = 0x74;//分张
        public const int ChoosePao = 0x75;
        public const int UserChoose = 0x76;
        public const int QiFei = 0x77;
        public const int Dingshen = 0x78;
        public const int Huanshen = 0x79;
        public const int LaiZiGang = 0x81;
        public const int DaiGu = 0x83;//农安玩法  
        public const int QiangGangHuType = 1 << 30;
        public const int Youjin = 93;//游金 
        public const int NextBao = 0x84;
        public const int Ligang = 0x71;//立杠\

        public const int NewGameBegin = 10086;//游戏开始
    }
}