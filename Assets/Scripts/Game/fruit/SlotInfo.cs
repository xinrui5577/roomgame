using System.Collections.Generic;
using YxFramwork.Common;

//运行时记录押注信息
namespace Assets.Scripts.Game.fruit
{
    public class SlotInfo
    {
        /// <summary>
        /// static 区
        /// </summary>
        #region

        static SlotInfo _slotInfo;
        public static SlotInfo GetSlotInfo()
        {
            return _slotInfo ?? (_slotInfo = new SlotInfo());
        }


        //获得本次要走到的点位
        public static int LotteryPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 设定当前比大小需要设置的值
        /// </summary>
        public static int CasinoNum { get; set; }

        /// <summary>
        /// 设定是否赢得casino
        /// </summary>
        public static bool IsWinTheCasino { get; set; }

        #endregion

        //存储每种水果当前压了多少点
        private static Dictionary<FruitType, int> _fruitSlotList = new Dictionary<FruitType, int>();
        public static Dictionary<FruitType, int> FruitSlotList
        {
            get { return _fruitSlotList; }
        }

        //把押注值清零
        public static void ClearFruitSlotListToZero()
        {
            if (_fruitSlotList != null && _fruitSlotList.Count > 0)
            {
                _fruitSlotList[FruitType.Apple] = 0;
                _fruitSlotList[FruitType.Bar] = 0;
                _fruitSlotList[FruitType.Bell] = 0;
                _fruitSlotList[FruitType.Orange] = 0;
                _fruitSlotList[FruitType.Seven] = 0;
                _fruitSlotList[FruitType.Star] = 0;
                _fruitSlotList[FruitType.Watermelon] = 0;
                _fruitSlotList[FruitType.Coco] = 0;
            }
            else
            {
                ResetFruitSlotList();
            }

        }

        private readonly Dictionary<int, SlotOddsInfo> _pointToOddsList = new Dictionary<int, SlotOddsInfo>();

        public Dictionary<int, SlotOddsInfo> pointToOddsList { get { return _pointToOddsList; } }

        public

        //设置点位代表的水果及其赔率
        struct SlotOddsInfo
        {
            public FruitType FruitType;
            public int SlotOdds;
        }

        //test随机 给个位置的测试用
        public void TestRandomLotertyPoint()
        {
            var random = new System.Random();
            LotteryPoint = random.Next(0, 23);//test 设置要走到的点位
        }


        private static List<int> _goodLuckPoints;
        //获得本次goodLuck的点位集合
        public static List<int> GoodLuckPoints
        {
            get { return _goodLuckPoints ?? (_goodLuckPoints = new List<int>()); }
            set
            {
                _goodLuckPoints = value;
            }
        }

        //goodLuckpoint 左
        public int LuckPointLeft { get; private set; }
        public int LuckPointRight { get; private set; }


        private SlotInfo()
        {
            IsWinTheCasino = false;
            //_curWinMoney = 0;

            ResetFruitSlotList();//重置水果押注金币信息列表

            SetSlotPtToFruit();// 设置水果点位映射赔率

            //设置goodluck point的点位
            LuckPointLeft = 21;
            LuckPointRight = 9;
        }
        //重置FruitSlotList
        public static void ResetFruitSlotList()
        {
            if (_fruitSlotList == null)
                _fruitSlotList = new Dictionary<FruitType, int>();

            _fruitSlotList.Clear();
            _fruitSlotList.Add(FruitType.Apple, 0);
            _fruitSlotList.Add(FruitType.Bar, 0);
            _fruitSlotList.Add(FruitType.Bell, 0);
            _fruitSlotList.Add(FruitType.Orange, 0);
            _fruitSlotList.Add(FruitType.Seven, 0);
            _fruitSlotList.Add(FruitType.Star, 0);
            _fruitSlotList.Add(FruitType.Watermelon, 0);
            _fruitSlotList.Add(FruitType.Coco, 0);
        }

        //设置各个点位所代表的水果
        private void SetSlotPtToFruit()
        {
            var oddspos0 = new SlotOddsInfo { FruitType = FruitType.Orange, SlotOdds = 10 };
            _pointToOddsList.Add(0, oddspos0);

            var oddspos1 = new SlotOddsInfo { FruitType = FruitType.Bell, SlotOdds = 15 };
            _pointToOddsList.Add(1, oddspos1);

            var oddspos2 = new SlotOddsInfo { FruitType = FruitType.Bar, SlotOdds = 50 };
            _pointToOddsList.Add(2, oddspos2);

            var oddspos3 = new SlotOddsInfo { FruitType = FruitType.Bar, SlotOdds = 120 };
            _pointToOddsList.Add(3, oddspos3);

            var oddspos4 = new SlotOddsInfo { FruitType = FruitType.Apple, SlotOdds = 5 };
            _pointToOddsList.Add(4, oddspos4);

            var oddspos5 = new SlotOddsInfo { FruitType = FruitType.Apple, SlotOdds = 3 };
            _pointToOddsList.Add(5, oddspos5);

            var oddspos6 = new SlotOddsInfo { FruitType = FruitType.Coco, SlotOdds = 10 };
            _pointToOddsList.Add(6, oddspos6);

            var oddspos7 = new SlotOddsInfo { FruitType = FruitType.Watermelon, SlotOdds = 20 };
            _pointToOddsList.Add(7, oddspos7);

            var oddspos8 = new SlotOddsInfo { FruitType = FruitType.Watermelon, SlotOdds = 3 };
            _pointToOddsList.Add(8, oddspos8);

            var oddspos10 = new SlotOddsInfo { FruitType = FruitType.Apple, SlotOdds = 5 };
            _pointToOddsList.Add(10, oddspos10);

            var oddspos11 = new SlotOddsInfo { FruitType = FruitType.Orange, SlotOdds = 3 };
            _pointToOddsList.Add(11, oddspos11);

            var oddspos12 = new SlotOddsInfo { FruitType = FruitType.Orange, SlotOdds = 10 };
            _pointToOddsList.Add(12, oddspos12);

            var oddspos13 = new SlotOddsInfo { FruitType = FruitType.Bell, SlotOdds = 15 };
            _pointToOddsList.Add(13, oddspos13);

            var oddspos14 = new SlotOddsInfo { FruitType = FruitType.Seven, SlotOdds = 3 };
            _pointToOddsList.Add(14, oddspos14);

            var oddspos15 = new SlotOddsInfo { FruitType = FruitType.Seven, SlotOdds = 40 };
            _pointToOddsList.Add(15, oddspos15);

            var oddspos16 = new SlotOddsInfo { FruitType = FruitType.Apple, SlotOdds = 5 };
            _pointToOddsList.Add(16, oddspos16);

            var oddspos17 = new SlotOddsInfo { FruitType = FruitType.Coco, SlotOdds = 3 };
            _pointToOddsList.Add(17, oddspos17);

            var oddspos18 = new SlotOddsInfo { FruitType = FruitType.Coco, SlotOdds = 10 };
            _pointToOddsList.Add(18, oddspos18);

            var oddspos19 = new SlotOddsInfo { FruitType = FruitType.Star, SlotOdds = 30 };
            _pointToOddsList.Add(19, oddspos19);

            var oddspos20 = new SlotOddsInfo { FruitType = FruitType.Star, SlotOdds = 3 };
            _pointToOddsList.Add(20, oddspos20);

            var oddspos22 = new SlotOddsInfo { FruitType = FruitType.Apple, SlotOdds = 5 };
            _pointToOddsList.Add(22, oddspos22);

            var oddspos23 = new SlotOddsInfo { FruitType = FruitType.Bell, SlotOdds = 3 };
            _pointToOddsList.Add(23, oddspos23);
            //_pointToOdds
        }


        //标记当前是否允许押注
        private bool _slotEnable = true;
        public bool SlotEnable
        {
            get { return _slotEnable; }
            set { _slotEnable = value; }
        }

        //设置押注点数
        public bool SetFruitSlotPoint(FruitType fruitType, int addPoint)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            //如果现在不允许押注，则直接退出:不让押注状态，或者没钱，不让押注
            if (_slotEnable == false || player.Coin < addPoint)
                return false;


            int fruitCout = _fruitSlotList[fruitType];
            if (fruitCout < 99)
            {
                _fruitSlotList[fruitType] += addPoint;
                // UserMoneyCount -= addPoint;//点击押注点，扣钱
            }
            else
            {
                _fruitSlotList[fruitType] = 0;
            }
            return true;
        }

        //设置押注点数,不考虑允不允许的情况,只考虑有没有钱
        public bool SetFruitSlotPoint_onlyCareMoney(FruitType fruitType, int addPoint)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer();
            //如果没钱，不让押注
            if (player.Coin < addPoint)
                return false;

            int fruitCout = _fruitSlotList[fruitType];
            if (fruitCout < 99)
            {
                _fruitSlotList[fruitType] += addPoint;
                player.Coin -= addPoint;//点击押注1点，扣1块钱
            }
            else
            {
                _fruitSlotList[fruitType] = 0;
            }
            return true;
        }


        public Dictionary<FruitType, int> GetFruitSlotList()
        {
            return _fruitSlotList;
        }
    }
}
