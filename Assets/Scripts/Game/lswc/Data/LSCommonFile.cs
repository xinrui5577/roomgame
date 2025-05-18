using Assets.Scripts.Game.lswc.Item;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.lswc.Data
{
    /// <summary>
    /// 游戏常量
    /// </summary>
    public class LSConstant
    {
        private LSConstant(){}

        #region 界面中部分sprite的name

        /// <summary>
        /// 界面上方的庄
        /// </summary>
        public const string Banker_Zhuang = "c6";

        /// <summary>
        /// 界面上方的和
        /// </summary>
        public const string Banker_He = "c5";

        /// <summary>
        /// 界面上方的闲
        /// </summary>
        public const string Banker_Xian = "c7";

        /// <summary>
        /// 历史记录中的庄
        /// </summary>
        public const string History_Banker_Zhuang = "c4";

        /// <summary>
        /// 历史记录中的和
        /// </summary>
        public const string History_Banker_He = "c3";

        /// <summary>
        /// 历史记录中的闲
        /// </summary>
        public const string History_Banker_Xian = "c8";

        #endregion

        #region 材质名称
        /// <summary>
        ///晶体正常
        /// </summary>
        public const string Crystal_Normal = "CrystalNormal";

        /// <summary>
        /// 晶体红
        /// </summary>
        public const string Crystal_Red = "CrystalChangeRed";

        /// <summary>
        /// 晶体绿
        /// </summary>
        public const string Crystal_Green = "CrystalChangeGreen";

        /// <summary>
        /// 晶体黄
        /// </summary>
        public const string Crystal_Yellow = "CrystalChangeYellow";

        /// <summary>
        /// 默认颜色区域的颜色
        /// </summary>
        public const string ColorItem_Default = "back_default";

        /// <summary>
        /// 颜色区域绿色
        /// </summary>
        public const string ColorItem_Green = "gem_green";

        /// <summary>
        /// 颜色区域黄色
        /// </summary>
        public const string ColorItem_Yellow = "gem_yellow";

        /// <summary>
        /// 颜色区域红色
        /// </summary>
        public const string ColorItem_Red = "gem_red";

        #endregion

        #region 游戏中数字控制

        /// <summary>
        /// 一万
        /// </summary>
        public const int Num_TenThousand = 10000;

        /// <summary>
        /// 一亿
        /// </summary>
        public const int Num_OneHanred_Million = 100000000;

        /// <summary>
        /// 红利上限
        /// </summary>
        public const int Num_Bonus_UpLimited = 30000;

        /// <summary>
        /// 红利下限
        /// </summary>
        public const int Num_Bonus_FlLimited = 1000;

        /// <summary>
        ///颜色区域数量
        /// </summary>
        public const int Num_ColorItemNumber =24;

        /// <summary>
        /// 动物区域数量
        /// </summary>
        public const int Num_AnimalItemNumber =24;

        /// <summary>
        /// 下注区域数量
        /// </summary>
        public const int Num_BetNumber = 15;

        /// <summary>
        /// item默认数量
        /// </summary>
        public const int Num_ItemDefaultNumber=1;

        /// <summary>
        /// 默认的颜色区域半径
        /// </summary>
        public const float Num_ColorRadius = 9;

        /// <summary>
        /// 默认的动物区域半径
        /// </summary>
        public const float Num_AnimalRadius = 18;

        /// <summary>
        /// 默认半径
        /// </summary>
        public const float Num_DefaultRadius = 1;

        /// <summary>
        /// 等待动画播放速度
        /// </summary>
        public const float Num_BetAnimationSpeed = 0.3f;

        /// <summary>
        /// 奖励动画与观看动画的播放速度
        /// </summary>
        public const float Num_WatchOrRewardAnimationSpeed = 0.6f;

        /// <summary>
        /// 历史记录的长度
        /// </summary>
        public const int History_Lenth = 10;

        /// <summary>
        /// 指针旋转圈数
        /// </summary>
        public const float Num_RotateNumber = 5;

        /// <summary>
        /// 动物旋转圈数
        /// </summary>
        public const float Num_AnimalRotateNumber = 7;

        /// <summary>
        /// 指针区域分块数量
        /// </summary>
        public const int Num_TurnTable_Normal = 24;
        #endregion

        #region 游戏中的各种时间
        /// <summary>
        /// 旋转时间(送灯旋转时间与正常旋转通用)
        /// </summary>
        public const float RotationTime = 15;

        /// <summary>
        /// 看向动物的时间
        /// </summary>
        public const float LookAtAnimalTime = 2;

        /// <summary>
        /// 移动到领奖台中间的时间
        /// </summary>
        public const float AnimoveMoveToCenterTime = 2;

        /// <summary>
        /// 动物领奖动画时间
        /// </summary>
        public const float ShowAnimationTime = 5;

        /// <summary>
        /// 显示环节类型,怎么这个也要等？尝试取消掉试试。
        /// </summary>
        public const float ShowGameTypeTime = 5;

        /// <summary>
        /// 结算UI时间
        /// </summary>
        public const float ShowResultUITime = 5;

        /// <summary>
        /// 送灯数目旋转时间
        /// </summary>
        public const float ShowSendLightNumTime = 5;

        #endregion

        #region 游戏中的适用的Key值相关
        /// <summary>
        /// 当前游戏的GameKey
        /// </summary>
        public const string GameKey = "lswc";

        /// <summary>
        /// GameKey带点
        /// </summary>
        public const string GameKeyWithPoint = "lswc.";

        /// <summary>
        /// 所有金币
        /// </summary>
        public const string KeyTotalGold = "ttgold";

        /// <summary>
        /// 历史记录（int32 array）
        /// </summary>
        public const string KeyHistoryResult = "history";

        /// <summary>
        /// round本游戏中用于作为伪随机的种子
        /// </summary>
        public const string KeyRound = "round";

        /// <summary>
        /// 下注请求
        /// </summary>
        public const string KeyAntes = "antes";

        /// <summary>
        /// 动物位置
        /// </summary>
        public const string KeyAnimalsPosition = "animals";

        /// <summary>
        /// 颜色位置
        /// </summary>
        public const string KeyColorPosition = "colors";

        /// <summary>
        /// 倍率区域初始化
        /// </summary>
        public const string KeyRates = "rates";

        /// <summary>
        /// 当前游戏的状态
        /// </summary>
        public const string KeyGameStatus = "status";

        /// <summary>
        /// 本局游戏开始时间long(13位，毫秒)
        /// </summary>
        public const string KeyGameStartTime = "startTime";

        /// <summary>
        /// 每局游戏的cd时间int（秒）
        /// </summary>
        public const string KeyCDTime = "cd";

        /// <summary>
        /// 每局游戏结果的键值
        /// </summary>
        public const string KeyRes = "rs";

        /// <summary>
        /// 指针位置
        /// </summary>
        public const string KeyPointIndex = "lastIdx";

        /// <summary>
        /// 历史纪录索引位置
        /// </summary>
        public const string KeyHistoryIndex = "hisIdx";

        /// <summary>
        /// 闪电倍率
        /// </summary>
        public const string KeyMultiple = "ligh";
        #endregion

        #region 游戏中音效的名称

        /// <summary>
        /// 大三元与大四喜的单独提示音
        /// </summary>
        public const string ThreeOrFourWaring = "dsydsxwaring";

        /// <summary>
        /// 按钮选中音效
        /// </summary>
        public const string ButtonSelectVoice = "ui_btn_select";

        /// <summary>
        /// 按钮选中失败音效
        /// </summary>
        public const string ButtonSelectFailVoice = "ui_btn_select_fail";

        /// <summary>
        /// 文字下落声音
        /// </summary>
        public const string LabelDown = "wenzidiaoxia";

        /// <summary>
        /// 文字飞出声音
        /// </summary>
        public const string LabelOut = "wenzifeichu";

        /// <summary>
        /// 播放等待动物出场音效
        /// </summary>
        public const string WaitForShowAnimal = "WaitforAnimal";

        /// <summary>
        /// 下注剩余五秒时倒计时的声音
        /// </summary>
        public const string BetCountDownVoice = "game_countdown";

        /// <summary>
        /// 中彩金音效
        /// </summary>
        public const string HandselVoice = "prize_bonus";

        /// <summary>
        /// 闪电特效
        /// </summary>
        public const string ShanDianVoice = "effect_shandian";
        #endregion

        #region 游戏中动画名称

        /// <summary>
        /// 转盘上方展开动画
        /// </summary>
        public const string Animation_DisplayTurnTable = "Display";

        /// <summary>
        /// 大四喜阶段前播放的灯下落动画
        /// </summary>
        public const string Animation_BigFourTypeRotate = "Rotate360";

        #endregion

    }

    #region 游戏中的枚举定义
    /// <summary>
    /// 游戏中的状态
    /// </summary>
    public enum ELswcGameState
    { 
        /// <summary>
        /// 等待游戏
        /// </summary>
        WaitState = 1,
        /// <summary>
        /// 押注
        /// </summary>
        BetState,   
        /// <summary>
        /// 结果状态
        /// </summary>
        ResultState,
        /// <summary>
        /// 什么状态？
        /// </summary>
        GameOver = 4
    }

    /// <summary>
    /// 服务器返回的结果，每位代表的意义
    /// </summary>
    public enum SC_ResultType
    {
        BANKER=0,           //庄和闲       
        RESOULT_TYPE,       //结果类型
        ANIMAL_INDEX,       //动物索引 
        
    }

    /// <summary>
    /// 游戏结果类型
    /// </summary>
    public enum LSRewardType
    {
        NORMAL=0,            //普通模式
        BIG_THREE,           //大三元
        BIG_FOUR,            //大四喜
        LIGHTING,            //闪电
        SENDLAMP,            //送灯
        HANDSEL,             //彩金
    }

    /// <summary>
    /// 庄和闲
    /// </summary>
    public enum LSBankerType
    {
        BANKER=0,           //庄 
        EQUAL,              //和
        PLAYER,             //闲
    }

    /// <summary>
    /// 动物类型
    /// </summary>
    public enum LSAnimalType
    {
        TZ=0,
        HZ,
        XM,
        SZ,
        GOLD_TZ,
        GOLD_HZ,
        GOLD_XM,
        GOLD_SZ,
    }

    /// <summary>
    /// 动物播放的动画类型
    /// </summary>
    public enum LSAnimalAnimationType
    {
        BET=0,
        RAWARD,
        WATCH
    }

    /// <summary>
    /// 颜色区域
    /// </summary>
    public enum LSColorType
    {
        RED=0,
        GREEN,
        YELLOW,
        DEFAULT
    }

    /// <summary>
    /// 动物图片类型
    /// </summary>
    public enum LSAnimalSpriteType
    {
        ar_tz_r = 0,
        ar_tz_g,
        ar_tz_y,
        ar_hz_r,
        ar_hz_g,
        ar_hz_y,
        ar_xm_r,
        ar_xm_g,
        ar_xm_y,
        ar_sz_r,
        ar_sz_g,
        ar_sz_y,
        ar_zx_z,
        ar_zx_h,
        ar_zx_x
    }

    /// <summary>
    ///  筹码按钮图片样式
    /// </summary>
    public enum LSAnteType
    {
        a0=0,
        a1,
        a2,
        a3,
        a31,
        a32,
        a33,
        a34,
    }

    /// <summary>
    /// 游戏中的请求类型
    /// </summary>
    public enum LSRequestMessageType
    {
        /// <summary>
        /// 下注阶段开始时间
        /// </summary>
        ON_BEGIN_BET = 1,       
        /// <summary>
        /// 服务器收集下注阶段
        /// </summary>
        ON_COLLECT_BET,      
        /// <summary>
        /// 获得结果阶段
        /// </summary>
        ON_GET_RESULT,         
        /// <summary>
        /// 下注
        /// </summary>
        BET,                   
    }
    #endregion
    /// <summary>
    /// 游戏结果
    /// </summary>
    public class LSResult
    {
        public LSResult(int index, int type, int banker, int multiple = 1)
        {
            _animalIndex = index;
            _resoultType = (LSRewardType)type;
            _bankerType = (LSBankerType)banker;

        }

        private int _animalIndex;

        private LSRewardType _resoultType;

        private LSBankerType _bankerType;

        private int _multiple;

        public int BetIndex
        {
            get { return _animalIndex; }
        }

        public LSRewardType ResultType
        {
            get { return _resoultType; }
        }

        public int Multiple
        {
            get { return _multiple; }
        }

        public LSBankerType Banker
        {
            get { return _bankerType; }
        }


        public override string ToString()
        {
            return ""; //"历史数据,动物 " + Ani + " 颜色是:" + Cor;
        }
    }

    /// <summary>
    /// 详细结果
    /// </summary>
    public struct LS_Detail_Result
    {

        /// <summary>
        /// 指针位置
        /// </summary>
        public int TurnTablePosition;
        /// <summary>
        /// 下注索引
        /// </summary>
        public int BetIndex;
        /// <summary>
        /// 动物类型
        /// </summary>
        public LSAnimalType Ani;
        /// <summary>
        /// 动物颜色
        /// </summary>
        public LSColorType Cor;

        /// <summary>
        /// 庄和闲
        /// </summary>
        public LSBankerType Banker;

        /// <summary>
        /// 游戏结果类型
        /// </summary>
        public LSRewardType Reward;

        /// <summary>
        /// 倍数
        /// </summary>
        public int Multiple;

        /// <summary>
        /// 赢的金币数量
        /// </summary>
        public long WinBets;
        /// <summary>
        /// 最终指向的动物，得在动物位置初始化之后可以确定
        /// </summary>
        public LSAnimalItem ToAnimal;
        /// <summary>
        /// 最终指向的颜色，根据index进行查找
        /// </summary>
        public LSColorItem ToColor;

        /// <summary>
        /// 初始位置移动到对应位置的角度
        /// </summary>
        public Vector3 ToAngle;

        /// <summary>
        /// 用于最后的结果面板显示
        /// </summary>
        public List<int> ShowResults;

        /// <summary>
        /// 结果播放音效
        /// </summary>
        public string lastResultVoice;
    }
}
