using UnityEngine;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商城中的记录
    /// </summary>
    public class TradeItem : MonoBehaviour
    {
        public UILabel SeriaNumber;
        public UILabel SpendMoney;
        public UILabel FinishState;
        public UILabel CreatTime;

        public void InitData(string seriaNumber,string money ,string state ,string time)
        {
            SeriaNumber.text = seriaNumber;
            SpendMoney.text = money;
            FinishState.text = int.Parse(state) == 0 ? "[ff5c5c]交易失败" : "[21a806]交易成功";
            CreatTime.text = time;
        }
    }
}
