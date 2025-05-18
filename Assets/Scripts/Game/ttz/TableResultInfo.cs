using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.Adapters;
using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ttz
{
    public class TableResultInfo : MonoBehaviour
    {
        public GameObject BankerResult;
        public GameObject PlayersResult;
        public GameObject MyResult;
        public GameObject ResultPrefab;
        public float MoveUpDic = 25;
        public float IntervalTime = 0.05f;

        [SerializeField]
        private float _hideTime;

        private float _num = 0;

        private List<GameObject> _resultList = new List<GameObject>();

        public void ShowTableResultInfo(ISFSObject responseData)
        {
            ShowPanel();
            int myWin = 0;
            if (responseData.ContainsKey("win"))
            {
                myWin = responseData.GetInt("win");
                SetResultLabel(myWin, MyResult);
            }
            if (responseData.ContainsKey("bwin"))
            {
                var bankerWin = responseData.GetLong("bwin");
                SetResultLabel(bankerWin, BankerResult);

            }
            if (responseData.ContainsKey("bpg"))
            {
                var wins = responseData.GetIntArray("bpg");
                long usersWin = 0;
                foreach (var w in wins)
                {
                    usersWin += w;
                }
                SetResultLabel(-usersWin, PlayersResult);
            }

        }

        private void ShowPanel()
        {
            gameObject.SetActive(true);
            while (_resultList.Count != 0)
            {
                Destroy(_resultList[0]);
                _resultList.RemoveAt(0);
            }
            _resultList.Clear();
            StartCoroutine(TimerHide());
        }

        protected void SetResultLabel(long result, GameObject target)
        {
            _num = 0;
            var str = "";
            str = result >= 0 ? "+" : "-";
            if (result > 0) Facade.Instance<MusicManager>().Play("win");
            else if (result < 0) Facade.Instance<MusicManager>().Play("lose");
            var go = Instantiate(ResultPrefab);
            go.transform.parent = target.transform;
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            var sprite = go.GetComponent<UISprite>();
            sprite.spriteName = str;
            sprite.MakePixelPerfect();
            go.SetActive(true);
            go.name = str;
            _resultList.Add(go);
            var strResult = YxUtiles.ReduceNumber(result, 2, true);
            var grid = target.GetComponent<UIGrid>();
            for (int i = 0; i < strResult.Length; i++)
            {
                if (result < 0 && i == 0) continue;
                var temp = CreateResultPrefab(target, str + strResult[i]);
                _resultList.Add(temp);
                grid.Reposition();
                var tween = temp.GetComponent<TweenPosition>();
                tween.from = temp.transform.localPosition;
                tween.to = tween.transform.localPosition + Vector3.up * MoveUpDic;
                tween.delay = _num;
                _num += IntervalTime;
                tween.PlayForward();
            }

            grid.Reposition();
        }

        protected void MoveUpResultItem()
        {

        }

        private GameObject CreateResultPrefab(GameObject target, string str)
        {
            var temp = Instantiate(ResultPrefab);
            temp.transform.parent = target.transform;
            temp.transform.localScale = Vector3.one;
            temp.transform.localPosition = Vector3.zero;
            var tempS = temp.GetComponent<UISprite>();
            tempS.spriteName = str;
            tempS.MakePixelPerfect();
            temp.SetActive(true);
            return temp;
        }

        private IEnumerator TimerHide()
        {
            yield return new WaitForSeconds(_hideTime);
            HidePabel();
        }

        private void HidePabel()
        {
            while (_resultList.Count != 0)
            {
                Destroy(_resultList[0]);
                _resultList.RemoveAt(0);
            }
            _resultList.Clear();
            gameObject.SetActive(false);
        }

        //void HideLabel(NguiLabelAdapter adapter)
        //{
        //    adapter.Label.text = string.Empty;
        //    adapter.gameObject.SetActive(false);
        //}

        //void SetLabel(long gold, NguiLabelAdapter adapter)
        //{
        //    adapter.Text(gold);
        //   // var labelStyle = gold >= 0 ? WinLabelStyle : LoseLabelStyle;
        //   // SetLabelStyle(adapter, labelStyle);
        //    adapter.gameObject.SetActive(true);
        //}

        //protected void SetLabelStyle(NguiLabelAdapter labelAdapter, LabelStyle style)
        //{
        //    var label = labelAdapter.Label;
        //    if (style.ApplyGradient)
        //    {
        //        label.applyGradient = true;
        //        label.gradientBottom = style.GradientBottom;
        //        label.gradientTop = style.GradientTop;
        //    }
        //    if (style.EffectStyle != UILabel.Effect.None)
        //    {
        //        label.effectStyle = style.EffectStyle;
        //        label.effectColor = style.EffectColor;
        //        label.effectDistance = style.EffectDistance;
        //    }

        //}
    }
}