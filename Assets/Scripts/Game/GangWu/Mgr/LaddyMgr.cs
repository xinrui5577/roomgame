using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.GangWu.Mgr
{
    public class LaddyMgr : MonoBehaviour
    {

        /// <summary>
        /// 记录总下注信息
        /// </summary>
        private int _allBetMoney;

        /// <summary>
        /// 所有玩家总下注显示窗口
        /// </summary>
        [SerializeField]
        private UILabel _allBetMoneyLabel = null;


        /// <summary>
        /// 用于存储、显示所有玩家总共的下注信息
        /// </summary>
        public int AllBetMoney
        {
            set
            {
                _allBetMoney = value;
                _allBetMoneyLabel.gameObject.SetActive(_allBetMoney > 0);
                _allBetMoneyLabel.text = YxUtiles.ReduceNumber(_allBetMoney);// App.GetGameData<GlobalData>().GetShowGold(_allBetMoney);
            }

            get
            {
                return _allBetMoney;
            }
        }


        /// <summary>
        /// 重置总下注额
        /// </summary>
        public void Rest(bool isLandDi = false)
        {
            AllBetMoney = isLandDi ? _allBetMoney : 0;
        }
    }
}