using System.Collections;
using YxFramwork.Common;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 下注区代码
    /// </summary>
    public class BetRegion : MonoBehaviour
    {
        /// <summary>
        /// 当前的押注类型
        /// </summary>
        public BetType CurBetType;
        /// <summary>
        /// 下注位置
        /// </summary>
        public GameObject BetPos;
        /// <summary>
        /// 押注类型图
        /// </summary>
        public UILabel BetTypeLabel;
        /// <summary>
        /// 下注宽
        /// </summary>
        public int BetW;
        /// <summary>
        /// 下注高
        /// </summary>
        public int BetH;
        /// <summary>
        /// 筹码管理 分种类
        /// </summary>
        public ArrayList[] _bets;
        /// <summary>
        /// 筹码管理 所有筹码
        /// </summary>
        public ArrayList _allBets;
        /// <summary>
        /// 总值显示
        /// </summary>
        public UILabel TotalLabel;
        /// <summary>
        /// 本区域的倍率
        /// </summary>
        public int Rate;
        /// <summary>
        /// 筹码出生点
        /// </summary>
        public GameObject BetBirth;

       protected void Start ()
        {
            SetBetType();
            _bets = new ArrayList[App.GetGameManager<TbsGameManager>().BetManager.Bets.Length];
            for (int i = 0; i < _bets.Length; i++)
            {
                _bets[i] = new ArrayList();
            }
            _allBets = new ArrayList();
        }
	
        /// <summary>
        /// 是否可以下注
        /// </summary>
        /// <param name="gold"></param>
        /// <returns></returns>
        public bool IsBet(int gold)
        {
            var gdata = App.GetGameData<TbsGameData>();
            var gmanager = App.GetGameManager<TbsGameManager>();
            if (!gmanager.BetManager.IsBeginBet)
                return false;

            if (gdata.GetPlayerInfo().CoinA < gold)
            {
                gmanager.Waring.OpenWaring("金币不足,请充值或换更小的筹码!");
                return false;
            }

            if (gdata.GetPlayerInfo() != null && gdata.GetPlayerInfo().Seat != App.GameData.SelfSeat)//只能往自己的区域下注
            {
                return false;
            }

            if (gdata.GetPlayerInfo() != null && gdata.GetPlayerInfo().Seat == gdata.BankerSeat)//庄家不能下注
            {
                return false;
            }

            return true;
        }
    

        //当前筹码个数
        private int _curBetCount;
        //最小深度
        public int MinDepth;
        /// <summary>
        /// 该区域下注总金币
        /// </summary>
        [HideInInspector]
        public int TotalGold;
        /// <summary>
        /// 该区域自己下注的金币
        /// </summary>
        [HideInInspector]
        public int SelfGold;

        /// <summary>
        /// 添加筹码 
        /// </summary>
        /// <param name="bet">筹码</param>
        /// <param name="seat">下注人的座位号  </param>
        /// <param name="gold"></param>
        /// <param name="index"></param>
        public void AddBet(GameObject bet,int seat,int gold,int index)
        {
            _curBetCount ++;
            GameObject gob = GetBet(bet, BetBirth.transform.localPosition);

            gob.GetComponent<TweenPosition>().from = BetBirth.transform.localPosition;
            gob.GetComponent<TweenPosition>().to = GetRandomV3();
            gob.GetComponent<TweenPosition>().PlayForward();
            _bets[index].Add(gob);
            _allBets.Add(gob);
            TotalGold += gold;

            TotalLabel.text = TotalGold > 0 ? YxUtiles.ReduceNumber(TotalGold) : "0";
        }
        /// <summary>
        /// 获取筹码对象
        /// </summary>
        public GameObject GetBet(GameObject bet,Vector3 birth)
        {
            var gob = Instantiate(bet);
            gob.transform.parent = BetPos.transform;
            gob.transform.localPosition = birth;
            gob.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            gob.transform.FindChild("Selected").GetComponent<UISprite>().depth = _curBetCount + MinDepth;
            gob.transform.FindChild("Title").gameObject.SetActive(false);
            gob.GetComponent<UISprite>().enabled = false;
            gob.GetComponent<BoxCollider>().enabled = false;

            return gob;
        }
        /// <summary>
        /// 获取一个下注区域内的随机的位置
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomV3()
        {
            int w = Random.Range(-BetW, BetW);
            int h = Random.Range(-BetH, BetH);
            var v3 = new Vector3(w, h, 0);
            return v3;
        }
        /// <summary>
        /// 从其他玩家手中获得筹码
        /// </summary>
        public void GetBetFromOther(GameObject bet,Vector3 birthV)
        {
            _curBetCount++;

            GameObject gob = GetBet(bet, birthV);
            gob.GetComponent<TweenPosition>().from = BetBirth.transform.localPosition;
            gob.GetComponent<TweenPosition>().to = GetRandomV3();
            gob.GetComponent<TweenPosition>().ResetToBeginning();
            gob.GetComponent<TweenPosition>().PlayForward();
            _allBets.Add(gob);
        }

        /// <summary>
        /// 减少筹码
        /// </summary>
        public void SubBet(int gold,int index)
        {
            if (_bets[index].Count <= 0)
            {
                YxDebug.LogError("没有可移除的筹码!");
                return;        
            }

            int obj = _bets[index].Count - 1;

            var gob = ((GameObject) _bets[index][obj]).GetComponent<TweenPosition>();

            gob.from = gob.transform.localPosition;
            gob.to = BetBirth.transform.localPosition;
        
            gob.ResetToBeginning();
            gob.PlayForward();

            _bets[index].RemoveAt(obj);

            TotalGold += gold;
            TotalLabel.text = TotalGold > 0 ? YxUtiles.ReduceNumber(TotalGold) : "0";

            if (App.GetGameData<TbsGameData>().GetPlayerInfo().Seat ==App.GameData.SelfSeat)
            {
                App.GetGameManager<TbsGameManager>().BetManager.ReSubtractBet();
            }
        }

        /// <summary>
        /// 根据钱数把筹码给其他region
        /// </summary>
        public void SendExistBet(BetRegion region,int gold)
        {
            gold = Mathf.Abs(gold);

            if (gold >= TotalGold)
            {
                SendAllExistBet(region);
            }
            else
            {
                AllBetBackBirth();
                SendNewBet(region,gold);
            }

        }
        /// <summary>
        /// 把自己桌面上的筹码给其他region
        /// </summary>
        public void SendAllExistBet(BetRegion region)
        {
            foreach (object t in _allBets)
            {
                region.ReceiveBet((GameObject)t);
            }
            _allBets.Clear();
            foreach (ArrayList t in _bets)
            {
                t.Clear();
            }
        }

        /// <summary>
        /// 给其他region新的筹码
        /// </summary>
        public void SendNewBet(BetRegion region,int gold)
        {
            //for (int i = 0; i < region._bets.Length; i++)
            //{
            //    for (int j = 0; j < region._bets[i].Count; j++)
            //    {
            //        GameObject gob = GetBet(BetManager.GetInstance().Bets[i], BetPos.transform.localPosition);
            //        region.ReceiveBet(gob);
            //    }
            //}

            gold = Mathf.Abs(gold);

            var gmanager = App.GetGameManager<TbsGameManager>();
            while (gold > 0)
            {
                for (int i = gmanager.BetManager.BetsValue.Length - 1; i >= 0; i--)
                {
                    if (gold >= gmanager.BetManager.BetsValue[i])
                    {
                        GameObject gob = GetBet(gmanager.BetManager.Bets[i], BetPos.transform.localPosition);
                        region.ReceiveBet(gob);
                        gold -= gmanager.BetManager.BetsValue[i];
                        break;
                    }

                    if (i == 0 && gold > 0)
                    {
                        GameObject gob = GetBet(gmanager.BetManager.Bets[i], BetPos.transform.localPosition);
                        region.ReceiveBet(gob);
                        gold -= gmanager.BetManager.BetsValue[i];
                    }
                }
            }

        }
        /// <summary>
        /// 接收其他Region发来的筹码
        /// </summary>
        public void ReceiveBet(GameObject gob)
        {
            gob.transform.parent = BetPos.transform;
            var tw = gob.GetComponent<TweenPosition>();
            tw.from = gob.transform.localPosition;
            tw.to = GetRandomV3();
            tw.duration += _allBets.Count / 100f;
            tw.ResetToBeginning();
            tw.PlayForward();
        
            _allBets.Add(gob);
        }
        /// <summary>
        /// 所有筹码回到出生点
        /// </summary>
        public void AllBetBackBirth()
        {
            foreach (object t in _allBets)
            {
                var gob = (GameObject) t;
                var tw = gob.GetComponent<TweenPosition>();
                tw.from = gob.transform.localPosition;
                tw.to = BetBirth.transform.localPosition;
                tw.ResetToBeginning();
                tw.PlayForward();
            }
        }

        /// <summary>
        /// 清除区域内的筹码
        /// </summary>
        public void ClearBet()
        {

            foreach (ArrayList t in _bets)
            {
                t.Clear();
            }

            foreach (object t in _allBets)
            {
                Destroy((GameObject)t);
            }
            _allBets.Clear();

            _curBetCount = 0;
            TotalGold = 0;
            TotalLabel.text = "";
        }

        /// <summary>
        /// 设置下注区域类型
        /// </summary>
        private void SetBetType()
        {
            switch (CurBetType)
            {
                case BetType.Banker:
                    BetTypeLabel.text = "庄家";
                    break;
                case BetType.Start:
                    BetTypeLabel.text = "初门";
                    break;
                case BetType.Middle:
                    BetTypeLabel.text = "天门";
                    break;
                case BetType.End:
                    BetTypeLabel.text = "末门";
                    break;
            }
        }
       
    }

    /// <summary>
    /// 下注区域类型
    /// </summary>
    public enum BetType
    {
        Banker,
        Start,
        Middle,
        End,
    }
}