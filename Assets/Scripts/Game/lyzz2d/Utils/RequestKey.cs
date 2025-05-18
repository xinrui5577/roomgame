namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class RequestKey
    {
        /// <summary>
        ///     id
        /// </summary>
        public const string KeyId = "id";

        /// <summary>
        ///     玩家ip
        /// </summary>
        public const string KeyIp = "ip";

        /// <summary>
        ///     玩家名称
        /// </summary>
        public const string KeyName = "name";

        /// <summary>
        ///     玩家名称2?
        /// </summary>
        public const string KeyUserName = "username";

        /// <summary>
        ///     昵称(为啥这么多....)
        /// </summary>
        public const string KeyNickName = "nick";

        /// <summary>
        ///     性别
        /// </summary>
        public const string KeySex = "sex";

        /// 胜利次数
        /// </summary>
        public const string KeyWinNum = "win";

        /// <summary>
        ///     失败次数
        /// </summary>
        public const string KeyLostNum = "lost";

        /// <summary>
        ///     总次数
        /// </summary>
        public const string KeyTotal = "total";

        /// <summary>
        ///     签名
        /// </summary>
        public const string KeySign = "signature";

        /// <summary>
        ///     头像
        /// </summary>
        public const string KeyAvatar = "avatar";

        /// <summary>
        ///     在线
        /// </summary>
        public const string KeyIsOnLine = "network";

        /// <summary>
        ///     房间可选参数
        /// </summary>
        public const string KeyCargs = "cargs";

        /// <summary>
        ///     当前的圈
        /// </summary>
        public const string KeyQuan = "quan";

        /// <summary>
        ///     漂
        /// </summary>
        public const string KeyPiao = "piao";

        /// <summary>
        ///     玩家等级
        /// </summary>
        public const string KeyLevel = "level";

        /// <summary>
        ///     指令
        /// </summary>
        public const string Cmd = "cmd";

        /// <summary>
        ///     类型
        /// </summary>
        public const string KeyType = "type";

        /// <summary>
        ///     座位
        /// </summary>
        public const string KeySeat = "seat";

        /// <summary>
        ///     多张牌
        /// </summary>
        public const string KeyCards = "cards";

        /// <summary>
        ///     本次游戏中会出现的所有牌型
        /// </summary>
        public const string KeyPCards = "pcards";

        /// <summary>
        ///     又是很多牌，但是这个是小结算时使用的牌
        /// </summary>
        public const string KeyCardsArr = "cardArr";

        /// <summary>
        ///     小结算胡牌
        /// </summary>
        public const string KeyHuCard = "huCard";

        /// <summary>
        ///     当前的局数
        /// </summary>
        public const string KeyNowRound = "round";

        /// <summary>
        ///     单张牌
        /// </summary>
        public const string KeyCard = "card";

        /// <summary>
        ///     单张牌，这个是可以用来操作的牌
        /// </summary>
        public const string KeyOpCard = "opCard";

        /// <summary>
        ///     重连标识
        /// </summary>
        public const string KeyRejoin = "rejoin";

        /// <summary>
        ///     显示的六位的房间号
        /// </summary>
        public const string KeyShowRoomID = "rid";

        /// <summary>
        ///     监测可以吃碰杠胡的结果 发到服务器(没用过)
        /// </summary>
        public const string PreCheck = "precheck";

        /// <summary>
        ///     整体轮数
        /// </summary>
        public const string KeySeq = "seq";

        /// <summary>
        ///     当前玩家所在轮数
        /// </summary>
        public const string KeySeq2 = "seq2";

        /// <summary>
        ///     bank0 庄的座位
        /// </summary>
        public const string KeyBanker0 = "bank0";

        /// <summary>
        ///     金币
        /// </summary>
        public const string KeyGold = "gold";

        /// <summary>
        ///     金币，总数
        /// </summary>
        public const string KeyTotalGold = "ttgold";

        /// <summary>
        ///     底注
        /// </summary>
        public const string KeyRate = "rate";

        /// <summary>
        ///     游戏房间类型
        /// </summary>
        public const string KeyGameType = "gtype";

        /// <summary>
        ///     房间名称
        /// </summary>
        public const string KeyRoomName = "roomName";

        /// <summary>
        ///     牌的总长度
        /// </summary>
        public const string KeyCardLenth = "cardLen";

        /// <summary>
        ///     用户，通常发送当前玩家的信息时用这个
        /// </summary>
        public const string KeyUser = "user";

        /// <summary>
        ///     多个用户，不包括当前用户
        /// </summary>
        public const string KeyUsers = "users";

        /// <summary>
        ///     又是多个用胡，小结算时使用的
        /// </summary>
        public const string KeyPlayerList = "players";

        /// <summary>
        ///     玩家数量
        /// </summary>
        public const string KeyPlayerNum = "playerNum";

        /// <summary>
        ///     所在地区
        /// </summary>
        public const string Country = "country";

        /// <summary>
        ///     状态值
        /// </summary>
        public const string KeyState = "state";

        /// <summary>
        ///     胡牌具体类型，目前支持类型：
        ///     1<<30 抢杠胡
        /// </summary>
        public const string KeyHuType = "ctype";

        /// <summary>
        ///     吃碰杠胡操作时，操作对应的小类型
        /// </summary>
        public const string KeyTypeType = "ttype";

        /// <summary>
        ///     组牌，所有的吃碰杠牌都在这里面
        /// </summary>
        public const string KeyGroups = "groups";

        /// <summary>
        ///     聊天常用语标记
        /// </summary>
        public const string KeyExp = "exp";

        /// <summary>
        ///     聊天文本标记
        /// </summary>
        public const string KeyText = "text";

        /// <summary>
        ///     小结算胡牌信息
        /// </summary>
        public const string KeyHuName = "hname";

        /// <summary>
        ///     小结算胡牌分数
        /// </summary>
        public const string KeyHuNum = "ghu";

        /// <summary>
        ///     小结算杠分数
        /// </summary>
        public const string KeyGangNum = "ggang";

        /// <summary>
        ///     大结算胡分数
        /// </summary>
        public const string KeyHuScore = "hu";

        /// <summary>
        ///     大结算点炮次数
        /// </summary>
        public const string KeyPaoScore = "pao";

        /// <summary>
        ///     大结算杠分
        /// </summary>
        public const string KeyGangScore = "gang";

        /// <summary>
        ///     大结算暗杠分数
        /// </summary>
        public const string KeyAnGangScore = "anGang";

        /// <summary>
        ///     大结算自摸次数
        /// </summary>
        public const string KeyZimoScore = "zimo";

        /// <summary>
        ///     CD Time
        /// </summary>
        public const string KeyCDTime = "cdTime";

        /// <summary>
        ///     手牌数量
        /// </summary>
        public const string KeyHandCardsNum = "handNum";

        /// <summary>
        ///     重新连接时，返回的手牌Key值
        /// </summary>
        public const string KeyHandCards = "handCards";

        /// <summary>
        ///     打出的牌
        /// </summary>
        public const string KeyOutCards = "outCards";

        /// <summary>
        ///     漂列表
        /// </summary>
        public const string KeyPiaoList = "piaolist";

        /// <summary>
        ///     麻将总数
        /// </summary>
        public const string KeyTotalLenth = "cardLen";

        /// <summary>
        ///     GPS x坐标
        /// </summary>
        public const string KeyGpsX = "gpsx";

        /// <summary>
        ///     GPS y坐标
        /// </summary>
        public const string KeyGpsY = "gpsy";

        /// <summary>
        ///     服务器返回的操作
        /// </summary>
        public const string KeyMenuOperation = "op";

        /// <summary>
        ///     当前操作玩家位置(重连后使用)
        /// </summary>
        public const string KeyCurrentPosition = "currentP";

        /// <summary>
        ///     退出之前打出的最后一张牌（重连使用）
        /// </summary>
        public const string KeyLastOutCard = "lastOutCard";

        /// <summary>
        ///     当前玩家的牌为14张时使用（单独抓的那张牌）
        /// </summary>
        public const string KeyLastIn = "lastin";

        /// <summary>
        ///     每局中谁先打牌（庄）正常逻辑时会发送这个
        /// </summary>
        public const string KeyStartPosition = "startP";

        /// <summary>
        ///     应该是两个骰子的值（roll点）
        /// </summary>
        public const string KeyDiceArray = "diceArray";

        /// <summary>
        ///     癞子牌
        /// </summary>
        public const string KeyLaiZi = "cardlaizi";

        /// <summary>
        ///     宝牌（单宝与双宝都用这个）
        /// </summary>
        public const string KeyBao = "baopai";

        /// <summary>
        ///     明宝配置
        /// </summary>
        public const string KeyMingBao = "-mingbao";

        /// <summary>
        ///     判断杠宝字段
        /// </summary>
        public const string KeyGangBao = "bao";

        /// <summary>
        ///     翻的那张牌
        /// </summary>
        public const string KeyFan = "cardfan";

        /// <summary>
        ///     投票起始时间
        /// </summary>
        public const string KeyHupStart = "hupstart";

        /// <summary>
        ///     最后打牌的座位号
        /// </summary>
        public const string KeyLastOutSeat = "lastOutP";

        /// <summary>
        ///     小结算时的庄标识
        /// </summary>
        public const string KeyNextBank = "nextBank";

        /// <summary>
        ///     回包状态标识（是否真实完成当前操作）
        /// </summary>
        public const string KeyOk = "ok";

        /// <summary>
        ///     选断门
        /// </summary>
        public const string KeyDuanMen = "duanmen";

        /// <summary>
        ///     总局数
        /// </summary>
        public const string KeyTotalRound = "maxRound";

        /// <summary>
        ///     打出后会显示听的牌
        /// </summary>
        public const string KeyTingOutCards = "tingout";

        /// <summary>
        ///     显示的规则
        /// </summary>
        public const string KeyRule = "rule";

        /// <summary>
        ///     是否有听
        /// </summary>
        public const string KeyHasTing = "hasTing";

        /// <summary>
        ///     摸宝
        /// </summary>
        public const string KeyMoBao = "mobao";

        /// <summary>
        ///     漂胡
        /// </summary>
        public const string KeyPiaoHu = "piaohu";

        /// <summary>
        ///     冲宝
        /// </summary>
        public const string KeyChongBao = "chbao";

        /// <summary>
        ///     亮
        /// </summary>
        public const string KeyLiang = "liang";

        /// <summary>
        ///     换宝时的上一张宝
        /// </summary>
        public const string KeyLastBao = "lastlaizi";

        /// <summary>
        ///     解散时间
        /// </summary>
        public const string KeyHupTime = "-tptout";

        /// <summary>
        ///     潇洒听 在潇洒的时候在厅里发送过去
        /// </summary>
        public const string KeyXs = "xs";

        /// <summary>
        ///     跟庄的字段
        /// </summary>
        public const string KeyGenZhuang = "genZhuang";

        /// <summary>
        ///     长毛Key(沈阳麻将用与判断时候在结算加倍的处理）
        /// </summary>
        public const string KeyZhangMao = "mao";

        /// <summary>
        ///     过蛋分数
        /// </summary>
        public const string KeyDanScore = "guoDanScore";

        /// <summary>
        ///     清风分数
        /// </summary>
        public const string KeyQingfengScore = "qingfeng";

        /// <summary>
        ///     选段门的重连状态
        /// </summary>
        public const string KeyDuanMenState = "status2";

        /// <summary>
        ///     房主ID
        /// </summary>
        public const string KeyOwnerId = "ownerId";

        /// <summary>
        ///     房间名称
        /// </summary>
        public const string KeyOwnerName = "ownerName";
    }
}