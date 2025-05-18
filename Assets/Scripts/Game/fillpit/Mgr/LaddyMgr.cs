using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class LaddyMgr : MonoBehaviour
    {

        /// <summary>
        /// 记录总下注信息
        /// </summary>
        protected int AllMoney;

        /// <summary>
        /// 所有玩家总下注显示窗口
        /// </summary>
        [SerializeField]
        protected UILabel AllBetMoneyLabel = null;

       

        

        /// <summary>
        /// 用于存储、显示所有玩家总共的下注信息
        /// </summary>
        public virtual int AllBetMoney
        {
            set
            {
                AllMoney = value;
                AllBetMoneyLabel.gameObject.SetActive(AllMoney > 0);
                AllBetMoneyLabel.text = YxUtiles.ReduceNumber(AllMoney);//App.GetGameData<GlobalData>().GetShowGold(_allBetMoney);
            }

            get
            {
                return AllMoney;
            }
        }

        /// <summary>
        /// 减去喜分
        /// </summary>
        public void DeductHappys()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (!gdata.IsLanDi) return;
            int happy = gdata.Happys;
            if (happy <= 0) return;
            int min = Mathf.Min(AllMoney, happy);     //总下注与喜分取小
            AllBetMoney -= min;
        }


        public virtual void OnDealCard(int round)
        {
            
        }

        public virtual void OnPlayerBet(int gold)
        {
            AllBetMoney += gold;
        }


        /// <summary>
        /// 重置总下注额
        /// </summary>
        public virtual void Rest(bool isLandDi = false)
        {
            AllBetMoney = isLandDi ? AllMoney : 0;
        }
    }
}