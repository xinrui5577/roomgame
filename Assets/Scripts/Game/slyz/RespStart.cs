using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.slyz
{
// 游戏中每一横行的牌
    public class CardTeam {

        public const int TYPE_HJTHS = 9;             // 皇家同花顺
        public const int TYEP_THS = 8;               // 同花顺
        public const int TYPE_ZD = 7;                // 炸弹
        public const int TYPE_HL = 6;                // 葫芦
        public const int TYPE_TH = 5;                // 同花
        public const int TYPE_SZ = 4;                // 顺子
        public const int TYPE_ST = 3;                // 三条
        public const int TYPE_LD = 2;                // 两对
        public const int TYPE_YD = 1;                // 一对
        public const int TYPE_GP = 0;                // 高牌

        public int[] cards;					// 牌值
        public int gold;					// 获得的金币
        public string typeName;				// 牌型名称
        public int rate;					// 倍率
        public int caijin;					// 获得的彩金
        public int type;					// 类型
	
        public void ParseData(ISFSObject temp)
        {
            cards = temp.GetIntArray("cards");
            string cardsValue = "";
            for ( int i = 0; i < cards.Length; i++ ) {

                cardsValue = cardsValue + "," + cards[i].ToString();
            }
            YxDebug.Log(cardsValue);
            gold = temp.GetInt("gold");		
            typeName = temp.GetUtfString("typeName");
            rate = temp.GetInt("rate");				
            caijin = temp.GetInt("caijin");
            type = temp.GetInt("type");				
        }
    }

/**
 * 记录好牌的记录和好牌的统计计数
 * 6 <= TYPE 的牌型为好牌
 * 好牌记录只保存前20条数据
 * 
 */

    public class CardRecord
    {
        public int Type;                            // 好牌类型ID
        public string TypeName;                     // 好牌类型名称
        public string TypeTime;                     // 好牌类型时间
    }


    public class CardStatistics
    {
        public int Type;                            // 好牌类型ID
        public string TypeName;                     // 好牌类型名字
        public int TypeCount;                       // 好牌类型次数
    }

/**
 * 用户点击开始之后 处理回包
 * 
 */
    public class RespStart{

        private int mGoodCardMinType = CardTeam.TYPE_HL;                                    // 好牌记录 的最小type值
        private int mAcountCardMinType = CardTeam.TYPE_YD;                                  // 数据统计 的最小type值

        private List<CardTeam> _cardTeamList = new List<CardTeam>();                        // 牌组列表		        				
        public List<CardTeam> CardTeamList { get { return _cardTeamList; } }

        private List<CardRecord> _mCardRecord = new List<CardRecord>();                       // 好牌记录列表 最大20个
        public List<CardRecord> CardRecord { get { return _mCardRecord; } }

        public List<CardStatistics> mCardStatistics = new List<CardStatistics>();           // 好牌统计列表 6<=type的为好牌
        public List<CardStatistics> CardStatistics { get { return mCardStatistics; } }

        // 解析游戏刚开始时 服务端发送过来的好牌历史纪录
        public void ParseCardRecordInGameInfo(string[] records)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            var recordLen = records.Length;
            for (var i = 0; i < recordLen; i++)
            {
                var words = records[i];
                //19-01-17 17:10,皇家同花顺,1000000,28660,michaelADQ
                var infos = words.Split(',');
                if (infos.Length < 5) continue;
                var time = infos[0];
                var type = infos[1];
                //var ante = infos[2];
                //var coin = infos[3];
                //var nick = infos[4];
                var cardRecord = new CardRecord
                {
                    TypeName = type,
                    TypeTime = time
                };
                _mCardRecord.Add(cardRecord);
            }
        }

        public void ParseData(ISFSObject temp) {

            _cardTeamList.Clear ();

            var sfArray = temp.GetSFSArray ("data");
            var count = sfArray.Size();
            for (var i = 0; i < count; i++ ) {
                var team = new CardTeam ();
                team.ParseData (sfArray.GetSFSObject(i));
                _cardTeamList.Add(team);

                // 好牌记录 统计葫芦以上的牌型
                if (team.type >= mGoodCardMinType)
                {
                    // 如果该组牌为好牌 记录下来
                    var cardRecord = new CardRecord
                    {
                        Type = team.type,
                        TypeName = team.typeName,
                        TypeTime = System.DateTime.Now.ToString("yy-MM-dd HH:mm")
                    };
                    _mCardRecord.Insert(0, cardRecord);

                    if (20 < _mCardRecord.Count)
                    {
                        _mCardRecord.RemoveAt(_mCardRecord.Count - 1);
                    }
                    Facade.EventCenter.DispatchEvent<ESlyzEventType,object>(ESlyzEventType.FreshRecord,null);
                }

                // 数据统计 统计获奖牌型出现次数
                if (mAcountCardMinType <= team.type)
                {
                    var bIsFound = false;
                    var len = mCardStatistics.Count;
                    for (var j = 0; j < len; j++)
                    {
                        if (team.type == mCardStatistics[j].Type)
                        {
                            bIsFound = true;
                            mCardStatistics[j].TypeCount += 1;
                            break;
                        }
                    }
                    if (bIsFound == false)
                    {
                        // 列表里没有的牌型 新建对象 并找到位置插入列表
                        var cardStatistics = new CardStatistics
                        {
                            Type = team.type,
                            TypeName = team.typeName,
                            TypeCount = 1
                        };
                        int iFindPos = 0, iLen = mCardStatistics.Count;
                        for (var m = 0; m < iLen; m++)
                        {
                            if (mCardStatistics[m].Type < cardStatistics.Type)
                            {
                                iFindPos = m;
                                break;
                            }
                        }
                        mCardStatistics.Insert(iFindPos, cardStatistics);
                    }
                }

            }
        }
    }
}