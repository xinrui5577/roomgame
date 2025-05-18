using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.mdx
{
    public class SetBanker : MonoBehaviour
    {

        public UIInput InputLabel;


        protected void Start()
        {
            var gdata = App.GetGameData<MdxGameData>();
            InputLabel.value = gdata.MinApplyBanker.ToString();
        }

        public void OnClickSureBtn()
        {
            long gold;
            if (!long.TryParse(InputLabel.value, out gold))
            {
                YxMessageBox.Show(App.GetGameManager<MdxGameManager>().TipStringFormat.BankErrorInput);
                return;
            }
          
            var selfCoin = App.GameData.GetPlayerInfo().CoinA;
            if (gold > selfCoin)
            {
                gameObject.SetActive(false);
                YxMessageBox.Show(App.GetGameManager<MdxGameManager>().TipStringFormat.NoEnoughGoldForBank);
                return;
            }

            gold = YxUtiles.RecoverShowNumber(gold);
            if (CouldApply(gold))
            {
                var sfsObject = new SFSObject();
                sfsObject.PutInt("type", RequestType.ApplyBanker);
                sfsObject.PutInt("gold", (int)gold);
                var server = App.GetRServer<MdxGameServer>();
                server.SendRequest(new ExtensionRequest(server.GameKey + RequestCmd.Request, sfsObject));
                server.SendGameRequest(sfsObject);
                gameObject.SetActive(false);
            }
            
        }

        bool CouldApply(long gold)
        {
            var gdata = App.GetGameData<MdxGameData>();

            if (gold < gdata.MinApplyBanker)
            {
                var limit = YxUtiles.GetShowNumberToString(gdata.MinApplyBanker);
                YxMessageBox.Show(string.Format("抢庄金额不能小于 {0}", limit));
                InputLabel.value = limit;
                return false;
            }

            if (gold > gdata.MaxApplyBanker)
            {
                var limit = YxUtiles.GetShowNumberToString(gdata.MaxApplyBanker);
                YxMessageBox.Show(string.Format("抢庄金额不能大于 {0}", limit));
                InputLabel.value = YxUtiles.GetShowNumberToString(gdata.MinApplyBanker);         //显示默认值
                return false;
            }
            return true;
        }


        public void OnValueChange()
        {
            long gold;
            var mgr = App.GetGameManager<MdxGameManager>();
            if (!long.TryParse(InputLabel.value, out gold))
            {
                return;
            }
            gold = YxUtiles.RecoverShowNumber(gold);
            var maxLimit = App.GetGameData<MdxGameData>().MaxApplyBanker;
            var limit = YxUtiles.GetShowNumberToString(maxLimit);
            if (gold > maxLimit)
            {
                YxMessageBox.Show(string.Format(mgr.TipStringFormat.InputTooLongForBank, limit));
                InputLabel.value = YxUtiles.GetShowNumberForm(maxLimit);         //显示默认值
            }
        }

    }
}
