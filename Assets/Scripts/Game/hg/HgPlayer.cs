using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.hg
{
    public class HgPlayer : YxBaseGamePlayer
    {

        public UIGrid ResultGrid;
        public UISprite ResultItem;
        public GameObject Blink;
        public List<GameObject> StarList;

        private HgGameData _gdata
        {
            get { return App.GetGameData<HgGameData>(); }
        }

        public void ShowResult(long winGold,long currentCoin)
        {
            var isWin = false;
            var winCoin = "";
            var win = YxUtiles.GetShowNumberForm(winGold, 0, "0.#"); //YxUtiles.GetShowNumberToString(winGold);不显示万字

            bool isZero = winGold == 0;
            if (winGold > 0)
            {
                isWin = true;
                ResultGrid.GetComponent<UISprite>().spriteName = "winBg";
                winCoin = "+" + win;
                Coin = currentCoin;
            }
            else
            {
                ResultGrid.GetComponent<UISprite>().spriteName = "loseBg";
                winCoin =  win;
            }

            while (ResultGrid.transform.childCount>0)
            {
                DestroyImmediate(ResultGrid.transform.GetChild(0).gameObject);
            }
            if (isZero) return;

            for (int i = 0; i < winCoin.Length; i++)
            {
                var item = YxWindowUtils.CreateItem(ResultItem, ResultGrid.transform);
                item.spriteName= ShowValue(winCoin.Substring(i, 1), isWin);
                item.MakePixelPerfect();
            }

            ResultGrid.gameObject.SetActive(true);
            ResultGrid.repositionNow = true;

            if (winGold > 0)
            {
                Blink.SetActive(true);
            }

            ResultGrid.GetComponent<TweenPosition>().PlayForward();
            ResultGrid.GetComponent<TweenPosition>().AddOnFinished(() => { StartCoroutine(HideValue()); });
           
        }

        IEnumerator HideValue()
        {
            var time = _gdata.UnitTime * 10;
            yield return new WaitForSeconds(time);
            Blink.SetActive(false);
            ResultGrid.gameObject.SetActive(false);
            ResultGrid.GetComponent<TweenPosition>().ResetToBeginning();
        }

        private string ShowValue(string chars,bool isWin)
        {
            var value = isWin ? "win" : "lose";
            switch (chars)
            {
                case "+":
                case "-":
                    value += "";
                    break;
                case ".":
                    value +="Spot";
                    break;
                case "0":
                    value += 0;
                    break;
                case "1":
                    value += 1;
                    break;
                case "2":
                    value += 2;
                    break;
                case "3":
                    value += 3;
                    break;
                case "4":
                    value += 4;
                    break;
                case "5":
                    value += 5;
                    break;
                case "6":
                    value += 6;
                    break;
                case "7":
                    value += 7;
                    break;
                case "8":
                    value += 8;
                    break;
                case "9":
                    value += 9;
                    break;
                case "万":
                    value += "Wan";
                    break;

            }
            return value;

        }

        public void ShowStarMove(int index)
        {
            if (StarList[index] != null)
            {
                StarList[index].SetActive(true);
            }
        }

        public void Clear()
        {
            foreach (var star in StarList)
            {
                if (star != null)
                {
                    star.SetActive(false);
                }
            }
        }
    }
}
