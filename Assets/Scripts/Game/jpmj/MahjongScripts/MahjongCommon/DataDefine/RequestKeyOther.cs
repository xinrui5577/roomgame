
namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine
{
    class RequestKeyOther
    {
        //性别
        public const string Sex = "sex";
        //赢的局数
        public const string WinTime = "win";
        //输的局数
        public const string LostTime = "lost";
        //一共的局数
        public const string TotalTime = "total";
        //等级
        public const string Level = "level";
        //签名
        public const string Signature = "signature";
        //头像
        public const string Avatar = "avatar";
        //IP
        public const string Ip = "ip";
        /// <summary>
        /// 地区
        /// 
        /// </summary>
        public const string Country = "country";
        //是否短线
        public const string IsBreaked = "isBreaked";
        public const string KeyNetWork = "network";

        public const string KeyTType = "ttype";

        public const string KeyDanType = "dantype";

        public const string KeyOp = "op";

        public const string KeyRound = "round";

        public const string KeyQuan = "quan";

        public const string KeyCargs = "cargs";

        public const string KeyRoomID = "rid";
        /// <summary>
        /// 房间唯一ID
        /// </summary>
        public const string KeyRoomOnlyId = "RoomIdOnly";

        public const string RollDICE = "rDice";

        public const string KeyMjGroup = "groups";

        public const string KeyHardNum = "handNum";

        public const string KeyOutCard = "outCards";

        public const string KeyHandCards = "handCards";

        public const string KeyDiceArray = "diceArray";

        public const string KeyStartP = "startP";

        public const string KeyLastIn = "lastin";
        public const string KeyCurrentP = "currentP";
        public const string KeLastOutCard = "lastOutCard";

        public const string KeyStatus = "status";

        public const string KeyRejoin = "rejoin";

        public const string GangGold = "gangGold";

        public const string CardFan = "cardfan";

        public const string CardLaizi = "cardlaizi";

        //第二个赖子
        public const string CardLaizi1 = "cardlaizi1";

        public const string CardBao = "bao";

        public const string KeyNextBanker = "nextBank";

        public const string KeySeq = "seq";
        public const string KeySeq2 = "seq2";

        public const string KeyGHu = "ghu";
        public const string KeyGGang = "ggang";

        public const string KeyGPiao = "gpiao";

        public const string KeyPlayerNum = "playerNum";

        public const string KeyZhaNiao = "zhaniao";
        //郑谭 鸟 索引
        public const string KeyNiao = "niao";
        //中码
        public const string KeyZhongma = "zhongma";

        public const string KeyChBao = "chbao";

        public const string KeyMoBao = "mobao";

        public const string KeyPiaoHu = "piaohu";

        public const string KeyBao = "bao";

        public const string KeyPiao = "piao";

        public const string KeyNiu = "niu";

        public const string KeyVisibleCards = "visibleCards";
        public const string KeyHasLiang = "hasLiang";
        public const string KeyCardLen = "cardLen";

        public const string KeyGHua = "ghua";
        /// <summary>
        /// 连分
        /// </summary>
        public const string KeyGLian = "glian";
        /// <summary>
        /// 台分
        /// </summary>
        public const string KeyGTai = "gtai";

        public const string KeyWillhucard = "willhucard";
    }

    public class MjRequestData
    {
        /**弹出选择漂按钮**/
        public const int MJRequestTypeSelectPiao = 0x1;
        /**前台玩家选择漂、后台发送展示玩家选择的漂**/
        public const int MJRequestTypeShowPiao = 0x2;
        /**游戏底注*/
        public const int MJRequestTypeShowRate = 0x3;
        /**翻牌*/
        public const int MJRequestTypeFanPai = 0x4;
        /**分牌**/
        public const int MJRequestTypeAlloCate = 0x11;
        /**玩家抓牌**/
        public const int MJRequestTypeGetInCard = 0x12;
        /**
	     * 玩家打牌
	     */
        public const int MJRequestTypeThrowoutCard = 0x13;
        /**
	     * 亮牌
	     */
        public const int MJRequestTypeLiangDao = 0x14;
        /**
	     * 玩家自摸
	     */
        public const int MJRequestTypeZimo = 0x15;

        /// <summary>
        /// 申请更多时间
        /// </summary>
        public const int MJRequestTypeMoreTime = 0x16;

        /// <summary>
        /// 用户操作吃碰杠胡
        /// </summary>
        public const int MJOpreateType = 0x18;
        public const int MJOpreateTypeNone = 0x0;
        public const int MJOpreateTypeChi = 0x1;
        public const int MJOpreateTypePeng = 0x2;
        public const int MJOpreateTypeGang = 0x4;
        public const int MJOpreateTypeHu = 0x8;
        public const int MJOpreateTypeLiang = 0x10;
        public const int MJOpreateTypeTing = 0x80;
        public const int MJOpreateTypeXFG = 0x20;
        public const int MJOpreateTypeXJFD = 1 << 9;

        /// <summary>
        /// 服务器允许玩家请求
        /// </summary>
        public const int MJResponseAllowResponse = 0x21;


        /// <summary>
        /// 询问客户端胡不胡这个牌（玩家杠的时候，看看别人能不能抢杠胡）
        /// </summary>
        public const int MjRequestTypeQiangGangHu = 0x23;

        public const int MjRequestTypeCheckCards = 0x40;
        /**
	     * 普通回应，表示玩家受到服务器通知或者做了取消操作 
	     */
        public const int MJRequestTypeCPG = 0x50;
        public const int CPG_None = 0;
        public const int CPG_Chi = 1;
        public const int CPG_Peng = 2;
        public const int CPG_PengGang = 5;

        public const int MJRequestTypeBao = 23;

        public const int MJRequestTypeHu = 0x54;

        // 自己抓牌后 可以杠胡的
        public const int MJRequestTypeSelfGang = 0x55;
        public const int CPG_ZhuaGang = 4;
        public const int CPG_MingGang = 6;
        public const int CPG_AnGang = 7;
        public const int CPG_LaiziGang = 8;

        // 旋风杠
        public const int MJRequestTypeXFG = 0x56;

        public const int MJReqTypeLastCd = 0x5A; //流局
        public const int MJReqTypeZiMo = 0x15; //自摸

        public const int MJReqGetNeedCard = 0x20;

        /**
	     * 补张
	     * */
        public const int MJRequestTypeBuZhang = 0x30;
        public const int MJRequestTypeBuZhangFinish = 0x31;
        public const int MJRequestTypeBuZhangGetIn = 0x32;


        public const int MJRequestTypeTing = 0x51;
        public const int MJRequestTypeGetHuCards = 0x5E;

        public const int MJRequestTypeDan = 0x5F;

        // 血战麻将
        public const int MJXZChangeCards = 0x60;                    // 开始换张
        public const int MJXZRotateCards = 0x61;                    // 开始旋转
        public const int MJXZSelectColor = 0x64;                    // 服务器通知开始选花色
        public const int MJXZSelColorRst = 0x65;                    // 把选中的花色发送给服务器
        public const int MJXZGameResult = 0x66;
        //起手报听
        public const int MJRequestStartTing = 0x69;
        public const int MJReponeseStartTing = 0x70;  
        //加码
        public const int MJRequestTypeJiaMa = 0x67;
        public const int MJRequestTypeJiaMaFinish = 0x68;
        //绝杠
        public const int MJRequestJueGang = 0x1a;      
        public const int CPG_AnJuegang = 0xb;
        //立杠
        public const int CPG_Ligang = 0x71;
	    //分张
	    public const int MJRequestFenZhang = 0x74;
        
        public const int MJChoosePao = 0x75;
        public const int MJUserChoose = 0x76;
        public const int MJQiFei = 0x77;
        public const int MJDingshen = 0x78;
        public const int MJHuanshen = 0x79;
        public const int QiFeiOut = 0x80;
    }
}
