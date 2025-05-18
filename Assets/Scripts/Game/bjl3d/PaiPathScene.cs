using System.Collections;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.bjl3d
{

    public class PaiPathScene : MonoBehaviour
    {
        public void SendCardFun()
        {

            StartCoroutine("SendCards");
        }

        IEnumerator SendCards()
        {
            yield return new WaitForSeconds(2f);

            Pai.GetInstance(App.GetGameData<Bjl3DGameData>().XianCards[0], "00", 0.0f, 0);
            Pai.GetInstance(App.GetGameData<Bjl3DGameData>().ZhuangCards[0], "10", 0.5f, 1);
            Pai.GetInstance(App.GetGameData<Bjl3DGameData>().XianCards[1], "01", 1f, 0);
            Pai.GetInstance(App.GetGameData<Bjl3DGameData>().ZhuangCards[1], "11", 1.5f, 1);

            if (App.GetGameData<Bjl3DGameData>().XianCards[2] != 0)
                Pai.GetInstance(App.GetGameData<Bjl3DGameData>().XianCards[2], "02", 2.0f, 0);

            if (App.GetGameData<Bjl3DGameData>().ZhuangCards[2] != 0)
                Pai.GetInstance(App.GetGameData<Bjl3DGameData>().ZhuangCards[2], "12", 2.5f, 1);
        }

    }
}