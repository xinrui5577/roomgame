using YxFramwork.Common;
using YxFramwork.Framework.Core;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 骰子管理 单例类
    /// </summary>
    public class DiceManager : MonoBehaviour
    {
    
        /// <summary>
        /// 骰子动画
        /// </summary>
        public GameObject DiceAnim;
        /// <summary>
        /// 骰子最后显示
        /// </summary>
        public GameObject[] Dices;
        /// <summary>
        /// 骰子盒
        /// </summary>
        public GameObject DiceBox;
        /// <summary>
        /// 当前掷骰子类型
        /// </summary>
        public DiceType CurDiceType;
        /// <summary>
        /// 投掷骰子间隔
        /// </summary>
        private int _rollTime;

      protected  void Start ()
        {
            CloseDice();
            DiceBox.SetActive(false);
            DiceVBelong = new[]
                {
                new[]{5,9},
                new[]{2,6,10},
                new[]{3,7,11},
                new[]{4,8,12}
                };
        }
	
        /// <summary>
        /// 开始掷骰子
        /// </summary>
        public void BeginRollDice(float spaceTime)
        {
            foreach (var dice in Dices)
            {
                dice.SetActive(false);
            }

            DiceBox.SetActive(true);

            Facade.Instance<MusicManager>().Play("roll");

            for (int i = 0; i < Dices.Length; i++)
            {
                Dices[i].GetComponent<UISprite>().spriteName = "shaizi" + _dicev[i]; //dicev[index][i];
            }

            DiceAnim.SetActive(true);

            Invoke("ShowDice", spaceTime);
        }
        /// <summary>
        /// 播放骰子动画
        /// </summary>
        public void PlayDiceAnim()
        {
            foreach (var dice in Dices)
            {
                dice.SetActive(false);
            }

            DiceBox.SetActive(true);
            Facade.Instance<MusicManager>().Play("roll");

            DiceAnim.SetActive(true);
        }

        private void ShowDice()
        {
            var gmanager = App.GetGameManager<TbsGameManager>();
            Facade.Instance<MusicManager>().Play("kill");
            DiceAnim.SetActive(false);
            foreach (var dice in Dices)
            {
                dice.SetActive(true);
            }

            int diceN = _dicev[0] + _dicev[1];
            gmanager.TimeMgr.PlayDiceNum(diceN, gmanager.BetTypeNames[App.GetGameData<TbsGameData>().DealMen], 1.5f, CurDiceType);
            Invoke("CloseBox",3f);
        }
        /// <summary>
        /// 关闭骰子盒
        /// </summary>
        public void CloseBox()
        {
            DiceBox.SetActive(false);
            foreach (var dice in Dices)
            {
                dice.SetActive(false);
            }
            switch (CurDiceType)
            {
                case DiceType.Deal:
                    break;
                case DiceType.Result:
                    App.GetGameManager<TbsGameManager>().BeginDistribute();
                    break;
                default:
                    YxDebug.Log("不存在的掷骰子类型!");
                    break;
            }
        }

        /// <summary>
        /// 骰子值属于哪家
        /// </summary>
        public int[][] DiceVBelong;

        /// <summary>
        /// 骰子值
        /// </summary>
        private int[] _dicev;

        /// <summary>
        /// 设置骰子的值
        /// </summary>
        /// <param name="dice0"></param>
        /// <param name="dice1"></param>
        /// <param name="dType"></param>
        public void SetDiceV(int dice0, int dice1, DiceType dType = DiceType.Deal)
        {
            var gdata = App.GetGameData<TbsGameData>();
            CurDiceType = dType;

            if (_dicev == null)
            {
                _dicev = new int[2];
            }
            _dicev[0] = dice0;
            _dicev[1] = dice1;

            switch (dType)
            {
                case DiceType.Deal:
                case DiceType.Result:
                    /**********判断谁先发牌************/
                    int dice = dice0 + dice1;

                    for (int i = 0; i < DiceVBelong.GetLength(0); i++)
                    {
                        for (int j = 0; j < DiceVBelong[i].GetLength(0); j++)
                        {
                            if (dice == DiceVBelong[i][j])
                            {
                                gdata.DealMen = i;
                                gdata.DealFirstSeat = (gdata.BankerSeat + i) % gdata.PlayerList.Length;
                                i = DiceVBelong.GetLength(0);
                                break;
                            }
                        }
                    }

                    break;
                default:
                    YxDebug.Log("不存在的掷骰子类型!");
                    break;
            }
        }

        /// <summary>
        /// 关闭骰子
        /// </summary>
        public void CloseDice()
        {
            DiceAnim.SetActive(false);
            foreach (var dice in Dices)
            {
                dice.SetActive(false);
            }
        }
        /// <summary>
        /// 开始下注的时间
        /// </summary>
        public float BeginTime;

        /// <summary>
        /// 发送掷骰子交互
        /// </summary>
        public void SendRollDice()
        {
            if (Time.time > BeginTime + _rollTime && App.GetGameData<TbsGameData>().BankerSeat ==App.GameData.SelfSeat && App.GetGameManager<TbsGameManager>().BetManager.IsBeginBet)
            {
                App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.StopBet, null);
            }
        }
        /// <summary>
        /// 发送开始骰子动画交互
        /// </summary>
        public void SendStartRollDice()
        {
            if (Time.time > BeginTime + _rollTime && App.GetGameData<TbsGameData>().BankerSeat == App.GameData.SelfSeat && App.GetGameManager<TbsGameManager>().BetManager.IsBeginBet)
            {
                App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.StartRollDice, null);
            }
        }

        /// <summary>
        /// 设置掷骰子间隔
        /// </summary>
        /// <param name="rt"></param>
        public void setRollTime(int rt)
        {
            _rollTime = rt;
        }
    }

    /// <summary>
    /// 掷骰子类型
    /// </summary>
    public enum DiceType
    {
        Deal, //分牌顺序
        Result, //结算顺序
    }
}