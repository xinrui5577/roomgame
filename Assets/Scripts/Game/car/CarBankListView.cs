using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Game.car
{
    public class CarBankListView : YxView
    {
        public EventObject EventObj;
        public UILabel TotalBank;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "FreshBankList":
                    FreshView((BankListData) data.Data);
                    break;
            }
        }

        private void FreshView(BankListData bankListData)
        {
            TotalBank.text = string.Format("{0}人在排队", bankListData.BankList.Count);
        }

        public void OnBankClick(UIButton applyBtn)
        {
            if (applyBtn.normalSprite.Equals("applyBank"))
            {
                var isApplyBank = App.GetGameData<CarGameData>().ApplyBank;
                if (isApplyBank)
                {
                    applyBtn.normalSprite = "giveUpBank";
                    EventObj.SendEvent("GameServerEvent", "ApplyBank", null);
                }
                else
                {
                    YxMessageTip.Show("您当前不具备 申请庄家的资格");
                }
            }
            else
            {
                applyBtn.normalSprite = "applyBank";
                EventObj.SendEvent("GameServerEvent", "GiveUpBank", null);
            }
        }
    }
}
