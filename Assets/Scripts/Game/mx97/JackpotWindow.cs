using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using System.Collections;

namespace Assets.Scripts.Game.mx97
{
    public class JackpotWindow : YxNguiWindow
    {
        public UISprite[] uiSprites;
        public AudioSource bigWin;

        protected override void OnFreshView()
        {
            if (!(Data is int)) return;

            var lengths = uiSprites.Length;//数集个数
            var number = (int)Data;

            if (BigWin.getInstance().IsBigWin())
                bigWin.Play();
            else
                Facade.Instance<MusicManager>().Play("Winning");

            var str = YxUtiles.GetShowNumber(number).ToString("0.00").PadLeft(lengths, '0');

            SwapResultPic(str);

            //Debug.LogError("str---------：" + str);

            for (var i = lengths - 1; i >= 0; i--)
            {
                uiSprites[i].GetComponent<UISprite>().enabled = true;
                uiSprites[i].GetComponent<UISprite>().spriteName = "n_" + str.Substring(i, 1);
            }

            if (str.IndexOf('-') != -1)
            {
                for (int i = 0; i < str.IndexOf('-'); i++)
                {
                    uiSprites[i].GetComponent<UISprite>().enabled = false;
                }
            }
            else
                LightsCtrl.GetInstance().ChangeLightStatus(LightsCtrl.StatusL.win);  //win light
        }

        protected override void OnHide()
        {
            LightsCtrl.GetInstance().ChangeLightStatus(LightsCtrl.StatusL.idle);

            if (bigWin.isPlaying)
                bigWin.Stop();
        }

        public IEnumerator customHide(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            this.Hide();
        }

        //胜利或者失败的图片切换
        public GameObject bgPic;
        private void SwapResultPic(string str)
        {
            if (str.IndexOf("-") == -1)
                bgPic.GetComponent<UISprite>().spriteName = "47";  //win
            else
                bgPic.GetComponent<UISprite>().spriteName = "48";  //lose
        }

    }
}
