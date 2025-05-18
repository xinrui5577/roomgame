using System.Collections;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetModeMgr : MonoBehaviour
    {
        public BetMode TheBetMode;

        private int _coinType;

        //设置下注筹码的数据
        public void SetBetModeChouMaData(int serverSeat)
        {
            if (serverSeat == App.GameData.GetPlayerInfo().Seat)
            {
                Debug.LogError(App.GetGameData<Brnn3dGameData>().BetMoney);
            }
            _coinType = GetCoinTypeByChouMaMoney(App.GetGameData<Brnn3dGameData>().BetMoney);

            SetBetModeChouMaDataEx(serverSeat);
            SetBetMoneyUIShowDataEx(serverSeat);//下注钱数界面的变化显示
        }
        int GetCoinTypeByChouMaMoney(int money)
        {
            var list = App.GameData.AnteRate;
            var count = list.Count;
            var index = 0;
            for (var i = 0; i < count; i++)
            {
                var value = list[i];
                if (value != money) continue;
                index = i;
                break;
            }
            return index;
        }

        //设置下注筹码的效果
        public void SetBetModeChouMaDataEx(int serverSeat)
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            TheBetMode.InstanceCoinDemo(_coinType, gdata.BetPos, serverSeat);
        }
        public bool IsSelf;

        public void DisPlay()
        {
            StartCoroutine("Wait");
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
        }

        //下注钱数界面的变化显示
        public void SetBetMoneyUIShowDataEx(int serverSeat)
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            if (gdata.SelfSeat == serverSeat)
            {
                IsSelf = true;
            }
            App.GetGameManager<Brnn3DGameManager>().TheMidUICtrl.TheBetMoneyUI.SetBetMoneyUI(IsSelf);
            IsSelf = false;
        }

    }
}

