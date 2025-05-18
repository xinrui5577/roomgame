using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Data
{
    public class ConstantData
    {
        ConstantData()
        {
        }

        #region 游戏中通用的Key

        #endregion

        /// <summary>
        /// 任务ID
        /// </summary>
        public const string KeyTaskId = "taskId";

        /// <summary>
        /// 玩家ID
        /// </summary>
        public const string KeyUserId = "uid";

        /// <summary>
        /// 游戏中的状态
        /// </summary>
        public const string KeyStatus = "status";

        /// <summary>
        /// CD
        /// </summary>
        public const string KeyCdTime = "cdTime";

        /// <summary>
        /// 默认头像
        /// </summary>
        public const string DefaultHeadName = "HS_9";

        /// <summary>
        /// 麻将背景类型
        /// </summary>
        public const string KeyMahjongBgType="MahjongBGType";

        /// <summary>
        /// 麻将值类型
        /// </summary>
        public const string KeyMahjongValueType = "MahjongValueType";

        /// <summary>
        /// 桌面颜色比例
        /// </summary>
        public const string KeyTableColorPercent = "MahjongTableColorPercent";

        /// <summary>
        /// 桌面颜色比例
        /// </summary>
        public const string KeyTableColor = "MahjongTableColor";
        /// <summary>
        /// 小结算胜利item的Bg
        /// </summary>
        public const string KeyWinerBg = "Bg_Winer";
        /// <summary>
        /// 小结算item的Bg
        /// </summary>
        public const string KeyNormalBg = "Bg_Normal";
        /// <summary>
        /// 潇洒的图片名
        /// </summary>
        public const string XiaoSa = "text_xiaosa";
        /// <summary>
        /// 听得图片名
        /// </summary>
        public const string Ting = "text_ting";
        /// <summary>
        /// 房主
        /// </summary>
        public const string RoomOwner = "房主:";
        /// <summary>
        /// 托管模式
        /// </summary>
        public const string KeyRobotToggle = "Robot";
        /// <summary>
        /// 托管请求
        /// </summary>
        public const string KeyRobotToggleRequest = "RobotRequest";
        /// <summary>
        /// 清风玩法
        /// </summary>
        public const string RuleQingFeng = "-qingfeng";
        /// <summary>
        /// 当前记录背景索引
        /// </summary>
        public const string KeyNowBgIndex = "bgIndex";

        #region 音乐相关 

        /// <summary>
        /// 游戏开始音效
        /// </summary>
        public const string VoiceGameBegin = "duijukaishi";

        /// <summary>
        /// 亮牌音效
        /// </summary>
        public const string VoiceLiangPai = "Crumple";

        /// <summary>
        /// 抓牌音效
        /// </summary>
        public const string VoiceZhuaPai = "zhuapai";

        /// <summary>
        /// 碰的音效
        /// </summary>
        public const string VoicePeng = "peng";

        /// <summary>
        /// 杠的音效
        /// </summary>
        public const string VoiceGang = "gang";

        /// <summary>
        /// 三风杠的音效
        /// </summary>
        public const string VoiceThreeFengGang = "sanFeng";

        /// <summary>
        /// 四风杠的音效
        /// </summary>
        public const string VoiceFourFengGang = "siFeng";

        /// <summary>
        /// 吃牌的音效
        /// </summary>
        public const string VoiceChi = "chi";

        /// <summary>
        /// 自摸音效
        /// </summary>
        public const string VoiceZiMo = "zimo";

        /// <summary>
        /// 胡音效
        /// </summary>
        public const string VoiceHu = "hu";

        /// <summary>
        /// 胡牌音效格式
        /// </summary>
        public const string HuFormat = "hu{0}";

        /// <summary>
        /// 听音效
        /// </summary>
        public const string VoiceTing = "ting";

        /// <summary>
        /// 截屏音效
        /// </summary>
        public const string VoiceScreenShot = "screenshot";

        /// <summary>
        /// 选中一张牌时的音效
        /// </summary>
        public const string VoiceSelect = "Tap";

        #endregion

        #region 常量数据相关
        /// <summary>
        /// 胡牌时抢杠胡标记位从type
        /// </summary>
        public const int GangHu = 1 << 30;
        /// <summary>
        /// Waiting面板消失时间
        /// </summary>
        public const float WaitingEscapeTime = 70;
        /// <summary>
        /// 默认颜色值哈哈哈
        /// </summary>
        public const float DefultTableColorPercent = 0.528f;
        /// <summary>
        /// 漂持续到牌局结束对应字段
        /// </summary>
        public const int DefPiaoForever = 99;
        /// <summary>
        /// 小结算胜利title名称
        /// </summary>
        public const string ResultWinTitle="Win";
        /// <summary>
        /// 小结算胜利title名称
        /// </summary>Lose
        public const string ResultLoseTitle = "Lose";
        /// <summary>
        /// 小结算胜利title名称
        /// </summary>
        public const string ResultRunOutTitle = "LiuJu";
        /// <summary>
        /// 小结算胜利title名称
        /// </summary>
        public const string ResultEqualTitle = "Equle";
        /// <summary>
        /// 杠底
        /// </summary>
        public const string KeyGangDiCard = "lastCard";
        /// <summary>
        /// 立杠列表
        /// </summary>
        public const string KeyGangList= "lilist";
        /// <summary>
        /// 杠类型
        /// </summary>
        public const string KeyGangType = "gangtype";
        /// <summary>
        /// 默认的显示的牌的层级
        /// </summary>
        public const int ShowItemLayler = 105;
        /// <summary>
        /// 选择过蛋对应的数字，这个需要与UI对应的name一致
        /// </summary>
        public const int DanSelectNum = 1;
        /// <summary>
        /// 常用语通用标记
        /// </summary>
        public const string TalkConmmonTag = "#common:";
        /// <summary>
        /// 默认的男性的索引标记
        /// </summary>
        public const int SexMan = 1;
        /// <summary>
        /// 默认的女性的索引标记
        /// </summary>
        public const int SexWoMen = 0;
        /// <summary>
        /// 选段门选择状态标识
        /// </summary>
        public const int DuanMenSelectState = 2;
        #region 麻将常量数据

        /// <summary>
        /// 水平飞的大小X
        /// </summary>
        public const int BgHorizontalFlySizeX = 130;

        /// <summary>
        /// 水平飞的大小Y
        /// </summary>
        public const int BgHorizontalFlySizeY = 82;

        /// <summary>
        /// 放倒水平X
        /// </summary>
        public const int BgHorizontalLieSizeX = 78;

        /// <summary>
        /// 放倒水平Y
        /// </summary>
        public const int BgHorizontalLieSizeY =90;

        /// <summary>
        /// 水平方向的牌站立的大小X
        /// </summary>
        public const int BgHorizontalStandSizeX = 44;

        /// <summary>
        /// 水平方向的牌站立的大小Y
        /// </summary>
        public const int BgHorizontalStandSizeY = 112;

        /// <summary>
        /// 竖直飞的大小X
        /// </summary>
        public const int BgVerticalFlySizeX = 78;

        /// <summary>
        /// 竖直飞的大小Y
        /// </summary>
        public const int BgVerticalFlySizeY = 139;

        /// <summary>
        /// 水平方向显示value的X
        /// </summary>
        public const int ValueHorizontalLieSizeX = 66;

        /// <summary>
        /// 水平方向显示value的Y
        /// </summary>
        public const int ValueHorizontalLieSizeY = 101;

        /// <summary>
        /// 常规背景X
        /// </summary>
        public const int BgNormalSizeX = 75;

        /// <summary>
        /// 常规背景Y
        /// </summary>
        public const int BgNormalSizeY = 107;

        /// <summary>
        /// 竖直方向麻将站立时，value显示的位置X
        /// </summary>
        public const int ValueVerticalStandPosY = 0;

        /// <summary>
        /// 竖直方向麻将站立时，value显示的位置X
        /// </summary>
        public const int ValueVerticalStandPosX = 0;

        /// <summary>
        /// 右侧方向麻将放倒时，value显示的位置X
        /// </summary>
        public const int ValueHorizontalLiePosX = -6;

        /// <summary>
        /// 右侧方向麻将放倒时，value显示的位置Y
        /// </summary>
        public const int ValueHorizontalLiePosY = 12;

        /// <summary>
        /// 竖直方向麻将放倒时，value显示的位置X
        /// </summary>
        public const int ValueVerticalLiePosX = 0;

        /// <summary>
        /// 竖直方向麻将放倒时，value显示的位置Y
        /// </summary>
        public const int ValueVerticalLiePosY = 20;

        /// <summary>
        /// 对面玩家亮牌时，牌值的X坐标
        /// </summary>
        public const int ValueOppsetLiePosX = -2;

        /// <summary>
        /// 对面玩家亮牌时，牌值的Y坐标
        /// </summary>
        public const int ValueOppserLiePosY = 6;

        /// <summary>
        /// 对面玩家亮牌时，牌值的X坐标
        /// </summary>
        public const int ValueOppsetStandPosX = -2;

        /// <summary>
        /// 对面玩家亮牌时，牌值的Y坐标
        /// </summary>
        public const int ValueOppserStandPosY = -20;

        /// <summary>
        /// 水平方向标记位置X
        /// </summary>
        public const int TagHorizontalPosX = 5;

        /// <summary>
        /// 水平方向标记位置Y
        /// </summary>
        public const int TagHorizontalPosY = 12;

        /// <summary>
        /// 竖直方向标记位置X
        /// </summary>
        public const int TagVerticalPosX = 15;

        /// <summary>
        /// 竖直方向标记位置Y
        /// </summary>
        public const int TagVerticalPosY = 10;

        /// <summary>
        /// 选段门
        /// </summary>
        public const int DuanMenSelect = 1;
        
        /// <summary>
        /// 不断门
        /// </summary>
        public const int BuDuanMen = 0;

        /// <summary>
        /// 选段门默认移动时间
        /// </summary>
        public const float DuanMenMoveTime = 3;
        /// <summary>
        /// 旋风杠 中发白的牌型
        /// </summary>
        public static readonly int[] XfgThree =
            {
                (int) EnumMahjongValue.Zhong,
                (int)EnumMahjongValue.Fa,
                (int)EnumMahjongValue.Bai 
            };
        /// <summary>
        /// 旋风杠 东南西北的牌型
        /// </summary>
        public static readonly int[] XfgFour =
            {
                (int) EnumMahjongValue.Dong,
                (int)EnumMahjongValue.Nan,
                (int)EnumMahjongValue.Xi ,
                (int)EnumMahjongValue.Bei 
            };
        /// <summary>
        /// 彩杠 中发白中牌型
        /// </summary>
        public static readonly int[] CgFourZhong =
            {
                (int) EnumMahjongValue.Zhong,
                (int) EnumMahjongValue.Zhong,
                (int)EnumMahjongValue.Fa,
                (int)EnumMahjongValue.Bai 
            };
        /// <summary>
        /// 彩杠 中发白发牌型
        /// </summary>
        public static readonly int[] CgFourFa =
            {
                (int) EnumMahjongValue.Zhong,
                (int)EnumMahjongValue.Fa,
                (int)EnumMahjongValue.Fa,
                (int)EnumMahjongValue.Bai 
            };
        /// <summary>
        /// 彩杠 中发白白牌型
        /// </summary>
        public static readonly int[] CgFourBai =
            {
                (int) EnumMahjongValue.Zhong,
                (int)EnumMahjongValue.Fa,
                (int)EnumMahjongValue.Bai,
                (int)EnumMahjongValue.Bai 
            };
        /// <summary>
        /// 营口麻将表情发送的特殊需要（表示表情）
        /// </summary>
        public const int ExpPlush = 1000;
        /// <summary>
        /// 同上表示的是带语音的话
        /// </summary>
        public const int SortTalkPlush = 2000;
        ///// <summary>
        ///// 水平方向标记位置X
        ///// </summary>
        //public const int Tag_Horizontal_PosX = 5;
        ///// <summary>
        ///// 水平方向标记位置Y
        ///// </summary>
        //public const int Tag_Horizontal_PosY = 12;

        #endregion

        #endregion
    }

    #region GameKeys      
    //todo 待处理，后期干掉，游戏中不应该存在gamekey判断，不灵活，应该对功能抽离
    /// <summary>
    /// 游戏gamekey，用于区分不同游戏的不同显示问题
    /// </summary>
    public enum EnumGameKeys
    {
        lyzz2d,  //转转2d
        pjmj,    //盘锦麻将
        shmj,    //绥化麻将
        fxmj,    //阜新麻将
        ykmj,    //营口麻将
        dlmj,    //大连麻将
        symj,    //沈阳麻将
        tamj,    //台安麻将
        dbsmj2d, //调兵山麻将2D

    }

    #endregion

    public enum TotalState
    {
        Init = 0, //游戏初始化状态 玩家不够，或者玩家够了，但是未准备状态
        Waiting, // 游戏准备状态
        Gaming, // 游戏开始中
        Account, // 结算 等待玩家点击连局 如果有人退出 则进入Waiting状态
        Over, // 游戏结束 不连局
    }

    #region  枚举

    #region 麻将相关

    /// <summary>
    /// 麻将的值
    /// </summary>
    public enum EnumMahjongValue
    {
        None = 0,
        Wan_1 = 17,
        Wan_2 = 18,
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
        ChunF = 96,
        XiaF = 97,
        QiuF = 98,
        DongF,
        MeiF,
        LanF,
        ZuF,
        JuF,
        AnBao=10086,
    }

    /// <summary>
    /// 麻将摆放方向，当前玩家与对家显示为竖直的，左右玩家显示为水平的
    /// </summary>
    public enum EnumMahJongDirection
    {
        Horizontal,
        Vertical,
    }
    /// <summary>
    /// 牌面颜色
    /// </summary>
    public enum EnumMahJongColorType
    {
        G=0,                                         //绿色
        Z,                                           //紫色
        B,                                           //蓝色
        Y,                                           //黄色
        Yl,                                          //浅黄
    }

    public enum EnumMahJongValueType
    {
        A=0,
        B,
        C
    }

    /// <summary>
    /// 显示方向
    /// </summary>
    public enum EnumShowDirection
    {
        Self,
        Right,
        Oppset,
        Left,
    }

    /// <summary>
    /// 麻将的几种状态
    /// Fly: 飞入过场
    /// Push:扣下，处于俯卧状态
    /// StandNo:站立没有数值状态，即为背面状态
    /// Lie：亮开状态，处于仰卧状态
    /// StandWith:站立且存在值，即当前玩家的手牌
    /// </summary>
    public enum EnumMahJongAction
    {
        Fly,
        Push,
        StandNo,
        Lie,
        StandWith,
    }

    #endregion

    #region 交互

    /// <summary>
    /// 服务器返回的消息类型
    /// </summary>
    public enum EnumRequest
    {
        None, //无
        SelectPiao = 0x1, //弹出选择漂按钮
        ShowPiao = 0x2, //前台玩家选择漂、后台发送展示玩家选择的漂
        ShowRate = 0x3, //显示底注
        GiveHun = 0x4, //会，也就是癞子
        XuanDuanMen = 0x5, //选断门菜单提示
        AlloCate = 0x11, //分牌
        GetCard = 0x12, //玩家抓牌
        ThrowOutCard = 0x13, //玩家打牌
        LiangCard = 0x14, //亮牌
        ZiMo = 0x15, //自摸
        MoreTime = 0x16, //申请更多时间
        BaoCard=0x17, //宝牌
        CpghMenu = 0x18, //吃碰杠胡菜单提示
        JueGang = 0x1a, //绝杠
        Card = 0x20, //获得一张牌
        AllowResponse = 0x21, //服务器允许玩家请求？？？
        QiangGangHu = 0x23, //询问玩家是否抢杠胡
        CheckCards = 0x40, //客户端出现问题时，从服务器同步数据
        CPHType = 0x50, //吃碰杠胡统一标记响应
        Ting = 0x51, //打听牌
        Hu = 0x54, //胡牌
        SelfGang = 0x55, //抓杠
        LiuJu = 0x5A, //流局
        HaiDi = 0x5C, //海底牌 ctype:0:显示“要”按钮,1玩家选择要海底牌,-1玩家不要海底牌
        XFG = 0x56, //旋风杠
        CaiGang = 0x57, //彩杠（中发白三张加上任意中发白）
        GuoDan=0x62,         //过蛋
        GuoDanSelect=0x63,   //过蛋选择
        LiGang=0x71,         //立杠
        FenZhang=0x74,        //分张
        Auto=0x85,            //托管
    }

    /// <summary>
    /// 牌组的类型
    /// </summary>
    public enum GroupType
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
        /// 赖子杠
        /// </summary>
        LaiZiGang = 8,

        /// <summary>
        /// 粘牌
        /// </summary>
        NianPai = 9,

        /// <summary>
        /// 旋风杠
        /// </summary>
        FengGang = 10,

        /// <summary>
        /// 绝杠
        /// </summary>
        JueGang = 11,

        /// <summary>
        /// 彩杠
        /// </summary>
        CaiGang = 12,
        /// <summary>
        /// 潇洒
        /// </summary>
        XiaoSa,
        Other,
    }

    /// <summary>
    /// 服务器返回的操作小类型，用于显示菜单使用
    /// </summary>
    public enum EnumCpghMenuType
    {
        None = 0,                               //无
        Chi = 1,                                //吃
        Peng = 1 << 1,                          //碰
        Gang = 1 << 2,                          //杠
        Hu = 1 << 3,                            //胡
        Liang = 1 << 4,                         //亮
        FengGang = 1 << 5,                      //旋风杠
        JueGang = 1 << 6,                       //绝杠
        Ting = 1 << 7,                          //听
        CaiGang=1<<8,                           //彩杠
		GuoDan=1<<9,							//长春麻将中的过蛋
        DanSelect=1<<10,                        //过蛋选择（盛将特殊要求，不在菜单中显示，但是为了服务器端处理重连状态，这个地方使用了op接口来处理）
        LiGang=1<<12,                           //立杠调兵山立杠
        ThreeFengGang= 1 << 13,                 //三风杠
        FourFengGang = 1 << 14,                 //四风杠
    }

    /// <summary>
    /// 吃碰杠胡确认小类型
    /// </summary>
    public enum EnumCpgType
    {
        None = 0,                                //无
        Chi,                                     //吃
        Peng,                                    //碰
        ZhuaGang = 4,                            //有刻子，抓一张相同的
        PengGang,                                //有三张，别人打一张
        MingGang,                                //自己有四张，服务器返回明杠标记（正面朝上？）
        AnGang,                                  //自己有四张，服务器返回暗杠标记（反面朝上？）
        LaiZiGang,                               //癞子杠，应该会有特殊的标记显示癞子牌吧
        Hu,                                      //胡
        ZiMo,                                    //自摸
        MoBao,                                   //摸宝
        PiaoHu,                                  //飘胡
        ChongBao,                                //冲宝
        HuanBao,                                 //换宝
        NiuBiHu,                                 //牛x
        Ting,                                    //听
        CaiGang,                                 //彩钢
        Xst,                                     //潇洒听
        QiangGangHu,                             //抢杠胡
        ThreeFengGang,                           //三风杠
        FourFengGang,                            //四风杠
        LiGang,                                  //立杠
    }
    #endregion

    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum EnumGameState
    {
        Init,
        Waitting,
        Gaming,
        Account,
        Over
    }

    #endregion

    #region Class

    public class RequestData
    {
        /// <summary>
        /// 响应类型
        /// </summary>
        public int Type;

        /// <summary>
        /// 当前玩家的手牌
        /// </summary>
        public int[] Cards = new[] {0, 0, 0, 0};

        /// <summary>
        /// 每次用于接收的牌(如果返回的值为0，则从wall中取)
        /// </summary>
        public int OpCard;

        /// <summary>
        /// 服务器返回的用于操作的牌
        /// </summary>
        public int Card;

        /// <summary>
        /// 桌位
        /// </summary>
        public int Sit;

        /// <summary>
        /// 吃碰杠胡类型
        /// </summary>
        public int Op;
        public int[] GenZhuang;
        public int[] TingOutCards;
        public int Xs;
        /// <summary>
        /// 当前操作的seq
        /// </summary>
        public int Seq;
        /// <summary>
        /// 分张数据
        /// </summary>
        public List<KeyValuePair<int, int>> FenZhangData =new List<KeyValuePair<int, int>>(); 

        public ISFSObject Data;

        public RequestData(ISFSObject data,bool specialFenZhang)
        {
            Data = data;
            Parse(data, specialFenZhang);
        }

        public RequestData(int seat,int card)
        {
            Sit = seat;
            Card = card;
            OpCard = card;
        }

        private void Parse(ISFSObject data,bool specialFenZhang=false)
        {
            GameTools.TryGetValueWitheKey(data, out Type, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out Sit, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(data, out OpCard, RequestKey.KeyOpCard);
            if (Type ==(int)EnumRequest.FenZhang)
            {
                FenZhangData.Clear();
                if (specialFenZhang)
                {
                    int[] array;
                    GameTools.TryGetValueWitheKey(data, out array, RequestKey.KeyCards);
                    var count = array.Length;
                    for (int i = 0; i < count; i++)
                    {
                        FenZhangData.Add(new KeyValuePair<int, int>(i,array[i]));
                    }
                }
                else
                {
                    ISFSArray array;
                    GameTools.TryGetValueWitheKey(data, out array, RequestKey.KeyCards);
                    var count = array.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var obj = array.GetSFSObject(i);
                        int seat = 0;
                        int card = 0;
                        GameTools.TryGetValueWitheKey(obj, out card, RequestKey.KeyCard);
                        GameTools.TryGetValueWitheKey(obj, out seat, RequestKey.KeySeat);
                        FenZhangData.Add(new KeyValuePair<int, int>(seat, card));
                    }
                }
            }
            else
            {
                GameTools.TryGetValueWitheKey(data, out Cards, RequestKey.KeyCards);
            }
            
            GameTools.TryGetValueWitheKey(data, out Card, RequestKey.KeyCard);
            GameTools.TryGetValueWitheKey(data, out Op, RequestKey.KeyMenuOperation);
            GameTools.TryGetValueWitheKey(data, out GenZhuang, RequestKey.KeyGenZhuang);
            GameTools.TryGetValueWitheKey(data, out Xs, RequestKey.KeyXs);
            GameTools.TryGetValueWitheKey(data, out Seq, RequestKey.KeySeq);
            if (Op > 0)
            {
                GameTools.TryGetValueWitheKey(data, out TingOutCards, RequestKey.KeyTingOutCards);
            }
            if (Cards.Length > 0)
            {
                if (Card == 0)
                {
                    Card = Cards[0];
                }
            }
        }
    }

    /// <summary>
    /// 大结算的数据
    /// </summary>
    public class OverData
    {
        public int gang;
        public int anGang;
        public int gold;
        public string nick;
        public int pao;
        public int hu;
        public int id;
        public int seat;
        public int zimo;
        public bool HouseOwner;

        /// <summary>
        /// 目前进入游戏后玩家的座位号是不变的，头像直接从对应玩家的牌桌上拿来，节省一次下载头像的操作。
        /// </summary>
        public UITexture avatar;

        public bool isPaoShou;
        public bool isYingJia;

        public OverData(ISFSObject data,int ownerId)
        {
            Parse(data);
            HouseOwner = id.Equals(ownerId);
        }

        private void Parse(ISFSObject user)
        {
            GameTools.TryGetValueWitheKey(user, out gang, RequestKey.KeyGangScore);
            GameTools.TryGetValueWitheKey(user, out anGang, RequestKey.KeyAnGangScore);
            GameTools.TryGetValueWitheKey(user, out gold, RequestKey.KeyGold);
            GameTools.TryGetValueWitheKey(user, out nick, RequestKey.KeyNickName);
            GameTools.TryGetValueWitheKey(user, out pao, RequestKey.KeyPaoScore);
            GameTools.TryGetValueWitheKey(user, out hu, RequestKey.KeyHuScore);
            GameTools.TryGetValueWitheKey(user, out id, RequestKey.KeyId);
            GameTools.TryGetValueWitheKey(user, out zimo, RequestKey.KeyZimoScore);
            GameTools.TryGetValueWitheKey(user, out seat, RequestKey.KeySeat);
        }
    }

    /// <summary>
    /// 聊天数据
    /// </summary>
    public class TalkData
    {
        public AudioClip Clip;
        public int Seat;
        public int Lenth;
    }

    /// <summary>
    /// 投票数据
    /// </summary>
    public class HupData
    {
        public int ID;
        public string Name;
        public int Operation; //-1:拒绝 2.发起 3同意。

        public void Parse(ISFSObject data)
        {

        }
    }

    /// <summary>
    /// 游戏内的工具类
    /// </summary>
    public class GameTools
    {
        /// <summary>
        /// 检查一个值是否存在
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="key"></param>
        public static void TryGetValueWitheKey(ISFSObject data, out int value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetInt(key);
            }
            else
            {
                value = 0;
                TryGetLog(key);
            }
        }

        public static void TryGetValueWitheKey(ISFSObject data, out long value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetLong(key);
            }
            else
            {
                value = 0;
                 TryGetLog(key);
            }
        }

        public static void TryGetValueWitheKey(ISFSObject data, out short value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetShort(key);
            }
            else
            {
                value = 0;
                TryGetLog(key);
            }
        }


        public static void TryGetValueWitheKey(ISFSObject data, out string value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetUtfString(key);
            }
            else
            {
                value = "";
                TryGetLog(key);
            }
        }

        public static void TryGetValueWitheKey(ISFSObject data, out bool value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetBool(key);
            }
            else
            {
                value = false;
                TryGetLog(key);
            }
        }

        public static void TryGetValueWitheKey(ISFSObject data, out int[] value, string key)
        {
            if (data.ContainsKey(key))
            {
                int[] arr = data.GetIntArray(key);
                value = (int[]) arr.Clone();
            }
            else
            {
                value = new int[0];
                TryGetLog(key);
            }
        }

        public static void TryGetValueWitheKey(ISFSObject data, out ISFSArray value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetSFSArray(key);
            }
            else
            {
                value = new SFSArray();
                TryGetLog(key);
            }
        }
        
        public static void TryGetValueWitheKey(ISFSObject data, out ISFSObject value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetSFSObject(key);
            }
            else
            {
                value = new SFSObject();
                TryGetLog(key);
            }
        }
        /// <summary>
        /// 尝试获得对应的数据时打的log
        /// </summary>
        /// <param name="key"></param>
        public static void TryGetLog(string key)
        {
            //YxDebug.LogError(string.Format("There is not exist a value with Key \"{0}\" from Server", key));
        }
        /// <summary>
        /// 获得对应类型的数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SFSObject getSFSObject(int type)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, type);
            return sfsObject;
        }
        /// <summary>
        /// 获得常用语语音
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetNormalTalkVoice(int sex, int index)
        {
            return string.Format("p{0}_Chat_{1}", sex, index);
        }
        /// <summary>
        /// 获得操作音频
        /// </summary>
        /// <param name="sex">性别</param>
        /// <param name="audioName">操作名称</param>
        /// <param name="randNum">随机</param>
        /// <returns></returns>
        public static string GetOperationVoice(int sex, string audioName, int randNum)
        {
            return string.Format("{0}-{1}-{2}", sex, audioName, randNum);
        }
        /// <summary>
        /// 创建一堆麻将
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<Transform> CreatMahjongItems(List<int> values)
        {
            List<Transform> _maList = new List<Transform>();
            for (int i = 0, lenth = values.Count; i < lenth; i++)
            {
                _maList.Add(CreateMahjong(values[i]));
            }
            return _maList;
        }
        /// <summary>
        /// 添加子对象到对应父级
        /// </summary>
        /// <param name="parent">父级</param>
        /// <param name="trans">自对象</param>
        /// <param name="offsetScaleX">默认x轴缩放倍数</param>
        /// <param name="offsetScaleY">默认y轴缩放倍数</param>
        /// <param name="setPosition">是否重置位置</param>
        /// <param name="refreshTran">是否刷新对应的物体显示</param>
        /// <returns></returns>
        static public Transform AddChild(Transform parent, Transform trans, float offsetScaleX = 1,
            float offsetScaleY = 1, bool setPosition = true,bool refreshTran=true)
        {
            if (trans != null && parent != null)
            {
                trans.parent = parent;
                if (setPosition)
                {
                    trans.localPosition = Vector3.zero;
                }
                trans.localRotation = Quaternion.identity;
                trans.localScale = new Vector3(offsetScaleX, offsetScaleY);
                trans.gameObject.layer = parent.gameObject.layer;
            }
            if (refreshTran)
            {
                RefreshTrans(trans);
            }
            return trans;
        }
        /// <summary>
        /// 创建一个新的麻将牌
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numBerChange"></param>
        /// <returns></returns>
        public static Transform CreateMahjong(int value, bool numBerChange = true)
        {
            MahjongItem item = App.GetGameManager<Mahjong2DGameManager>().GetNextMahjong(numBerChange);
            item.Value = value;
            return item.transform;
        }

        /// <summary>
        /// 排序，带癞子，从大到小排序，癞子默认最小（牌局中使用排序）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithLaiZi(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[j].Equals(laiZiNum))
                    {
                        continue;
                    }
                    if (list[i] < list[j] || list[i].Equals(laiZiNum))
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 排序，带癞子，从小到大排序，癞子默认最小（结算时的处理）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithLaiZiOppset(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                if (list[i].Equals(laiZiNum))
                {
                    continue;
                }
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[j].Equals(laiZiNum))
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                        break;
                    }
                    if (list[i] > list[j])
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 排序，移除癞子
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithOutLaiZi(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[i] > list[j])
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            list.Remove(laiZiNum);
            return list;
        }
        /// <summary>
        /// 获取组牌数据
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static List<MahjongGroupData> GetGroupData(ISFSArray groups)
        {
            List<MahjongGroupData> datas = new List<MahjongGroupData>();
            for (int i = 0, lenth = groups.Size(); i < lenth; i++)
            {
                ISFSObject g = groups.GetSFSObject(i);
                MahjongGroupData groupData = CreateGroupFromSfsObject(g);
                datas.Add(groupData);
            }
            return datas;
        }
        /// <summary>
        /// 删除麻将控制脚本
        /// </summary>
        /// <param name="item"></param>
        public static void DestroyUserContorl(MahjongItem item)
        {
            UserContorl uc = item.GetComponent<UserContorl>();
            if (uc != null)
                Object.DestroyImmediate(uc);
        }

        /// <summary>
        /// 删除麻将控制脚本
        /// </summary>
        /// <param name="item"></param>
        public static void DestroyUserContorl(Transform item)
        {
            UserContorl uc = item.GetComponent<UserContorl>();
            if (uc != null)
                Object.DestroyImmediate(uc);
        }
        /// <summary>
        /// 删除拖拽组件
        /// </summary>
        /// <param name="item"></param>
        public static void DestroyDragObject(MahjongItem item)
        {
            UIDragObject drag = item.GetComponent<UIDragObject>();
            if(drag!=null)Object.Destroy(drag);
        }

        /// <summary>
        /// 创建一个牌组(重连时使用这个，这个时候是需要处理Group中的数据即可)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MahjongGroupData CreateGroupFromSfsObject(ISFSObject data)
        {
            int gtype;
            int value;
            int[] cards;
            GameTools.TryGetValueWitheKey(data, out gtype, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out value, RequestKey.KeyCard);
            GameTools.TryGetValueWitheKey(data, out cards, RequestKey.KeyCards);
            GroupType type = (GroupType) gtype;
            MahjongGroupData groupData = new MahjongGroupData(type);
            if (cards.Length == 3)
            {
                groupData.values = new int[3];
            }
            switch (type)
            {
                case GroupType.Chi:
                    groupData.AddValue(cards[0]);
                    groupData.AddValue(value);
                    groupData.AddValue(cards[1]);
                    break;
                case GroupType.FengGang:
                case GroupType.CaiGang:
                    for (int i = 0; i < cards.Length; i++)
                    {
                        groupData.AddValue(cards[i]);
                    }
                    break;
                default:
                    for (int i = 0, max = groupData.values.Length; i < max; i++)
                    {
                        groupData.AddValue(value);
                    }
                    break;
            }
            return groupData;
        }

        /// <summary>
        /// 判断当前应用的网络制式: 0 无网络 1.移动网络 2.wifi 。-1应该不会有，是出错了
        /// </summary>
        /// <returns></returns>
        public static int JudgeNetworkType()
        {
            int netWork = 0;
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    netWork = 0;
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    netWork = 1;
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    netWork = 2;
                    break;
                default:
                    netWork = -1;
                    Debug.LogError("There is no such network type,please check network about");
                    break;
            }
            return netWork;
        }
        /// <summary>
        /// 给对应tran上的麻将赋值
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="value"></param>
        public static void SetMahjongValueByTrans(Transform trans,EnumMahjongValue value)
        {
            if (trans==null)
            {
                return;
            }
            if(trans.gameObject.activeInHierarchy==false)
            {
                trans.gameObject.SetActive(true);
            }
            MahjongItem item = trans.GetComponent<MahjongItem>();
            if (item!=null)
            {
                item.SelfData.Value = value;
                item.SelfData.Direction=EnumMahJongDirection.Vertical;
                item.SelfData.ShowDirection=EnumShowDirection.Self;
                item.SelfData.Action=EnumMahJongAction.StandWith;
            }
            else
            {
                Debug.LogError("这里有一张牌，你想设置它的牌");
            }
        }

        [DllImport("__Internal")]
        private static extern float getiOSBatteryLevel();
        /// <summary>
        /// 获得系统电量,多平台处理
        /// </summary>
        /// <returns></returns>
        public static int GetBatteryLevel()
        {
#if UNITY_ANDROID
            try
            {
                string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
                return int.Parse(CapacityString);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to read battery power; " + e.Message);
            }
            return -1;
#elif UNITY_IOS
            return (int)(100*getiOSBatteryLevel());
#endif
            return 1;
        }
        #endregion
        /// <summary>
        /// 获得当前的系统时间。默认开始时间为格林威治时间1970.1.1.0，即北京时间1970.1.1.8
        /// unix 为秒数转换
        /// js为毫秒
        /// </summary>
        /// <param name="svt"></param>
        /// <returns></returns>
        public static DateTime GetSvtTime(long svt)
        {
            DateTime s = new DateTime(1970, 1, 1, 8, 0, 0);
            s = s.AddSeconds(svt);
            return s;
        }
        /// <summary>
        /// 将色值转换为制定格式str,便于存储
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ParseColorToStr(Color color)
        {
            return string.Format("{0}_{1}_{2}_{3}", color.r, color.g, color.b, color.a);
        }
        /// <summary>
        /// 将str转换成色值，用于对Tex进行赋值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color ParseStrToColor(string str)
        {
            string[] colors = str.Split('_');
            Color color=new Color();
            for (int i = 0,lenth=colors.Length; i < lenth; i++)
            {
                color[i] = float.Parse(colors[i]);
            }
            return color;
        }
        /// <summary>
        /// 获得保存的颜色数据
        /// </summary>
        /// <param name="defColor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color GetSaveColor(Color defColor,out float value)
        {
            string saveStr=PlayerPrefs.GetString(ConstantData.KeyTableColor, ParseColorToStr(defColor));
            Color saveColor = ParseStrToColor(saveStr);
            value = PlayerPrefs.GetFloat(ConstantData.KeyTableColorPercent,ConstantData.DefultTableColorPercent);
            return saveColor;
        }
        /// <summary>
        /// 刷新对象
        /// </summary>
        public static void RefreshTrans(Transform trans)
        {
            int childNum = trans.childCount;
            UIWidget selfWidget = trans.GetComponent<UIWidget>();
            if (selfWidget!=null)
            {
                selfWidget.ParentHasChanged();
                selfWidget.MarkAsChanged();
            }
            for (int i = 0; i < childNum; i++)
            {
                Transform childTrans = trans.GetChild(i);
                RefreshTrans(childTrans);
            }
        }
    }
}