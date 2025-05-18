using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.hg
{
    public class HgTrendWindow : YxNguiWindow
    {
        public EventObject EventObj;
        public List<int> LoadsShowLine;
        public TweenPosition Tween;
        public UIScrollBar DishRoadScrollBar;
        public UIGrid DishRoadGrid;
        public HgRoadItem DishHgRoadSpriteItem;

        public UIScrollBar BigRoadScrollBar;
        public Transform BigRoadParenTransform;
        public UISprite BigRoadSpriteItem;

        public UIScrollBar BigEyeRoadScrollBar;
        public Transform BigEyeRoadParenTransform;
        public UISprite BigEyeRoadSpriteItem;

        public UIScrollBar SmallRoadScrollBar;
        public Transform SmallRoadParenTransform;
        public UISprite SmallRoadSpriteItem;

        public UIScrollBar RoachRoadScrollBar;
        public Transform RoachRoadParentTransform;
        public UISprite RoachRoadSpriteItem;

        public UISprite NextBlackBigEye;
        public UISprite NextBlackSmall;
        public UISprite NextBlackRoach;

        public UISprite NextRedBigEye;
        public UISprite NextRedSmall;
        public UISprite NextRedRoach;


        public UILabel RedCountLabel;
        public UILabel BlackCountLabel;
        public UILabel TotaCountLabel;

        public UIGrid BottomCardTypeGrid;
        public UISprite BottomSpriteItem;
        public string CardTypeNormal;
        public string CardTypeSpecial;

        private HgGameData _gdata
        {
            get { return App.GetGameData<HgGameData>(); }
        }

        private bool _isAuto;
        private List<List<string>> _recordSpot = new List<List<string>>();

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Init":
                    InitWindow();
                    break;
                case "Close":
                    TweenPackUp();
                    break;
                case "AddOne":
                    AddOntItem((Dictionary<string, object>)data.Data);
                    TweenOpen();
                    break;
                case "Clear":
                    Clear();
                    break;
            }
        }

        private void InitWindow()
        {
            FreshWinCardType();
            _recordSpot.AddRange(_gdata.RecordSpot);
            DishRoadShow();//珠盘路的战绩
            GetOtherRoad(_recordSpot);//其余几路
            GetNext();

        }
        private void TweenPackUp()
        {
            if (Math.Abs(Tween.transform.localPosition.y - 600f) > 0)
            {
                Tween.Toggle();
            }
        }

        private void TweenOpen()
        {
            if (!_isAuto) return;
            if (Math.Abs(Tween.transform.localPosition.y - 600f) > 0)
            {

            }
            else
            {
                Tween.Toggle();
            }
        }


        private void AddOntItem(Dictionary<string, object> dic)
        {
            if (dic.ContainsKey("recordSpot"))
            {
                var recordSpot = (List<string>)dic["recordSpot"];
                AddOneDishRoadItem(recordSpot);
            }

            if (dic.ContainsKey("recordCardType"))
            {
                if (BottomCardTypeGrid)
                {
                    var recordCardType = dic["recordCardType"];
                    GetOneCardType(int.Parse(recordCardType.ToString()));
                }
            }

            GetNext();//预测下局
        }

        private void Clear()
        {
            _recordSpot.Clear();

            while (DishRoadGrid.transform.childCount > 0)
            {
                DestroyImmediate(DishRoadGrid.transform.GetChild(0).gameObject);
            }
            while (BigRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(BigRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (BigEyeRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(BigEyeRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (SmallRoadParenTransform.transform.childCount > 0)
            {
                DestroyImmediate(SmallRoadParenTransform.transform.GetChild(0).gameObject);
            }
            while (RoachRoadParentTransform.transform.childCount > 0)
            {
                DestroyImmediate(RoachRoadParentTransform.transform.GetChild(0).gameObject);
            }
        }

        public void OnAuto(UIToggle toggle)
        {
            _isAuto = toggle.value;
        }

        private void FreshWinCardType()
        {
            if (BottomCardTypeGrid == null) return;
            var recordCardType = _gdata.RecordCardType;

            for (int i = 0; i < recordCardType.Count; i++)
            {
                var item2 = YxWindowUtils.CreateItem(BottomSpriteItem, BottomCardTypeGrid.transform);
                int colorIndex;
                var str = GetCardType(recordCardType[i], out colorIndex);

                item2.spriteName = colorIndex == 0 ? CardTypeNormal : CardTypeSpecial;
                var label = item2.GetComponentInChildren<UILabel>();
                label.text = str;
                label.color = colorIndex == 0 ? new Color(186f / 255, 152f / 255, 104f / 255) : new Color(144f / 255, 82f / 255, 0);

                if (i == recordCardType.Count - 1)
                {
                    item2.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            BottomCardTypeGrid.repositionNow = true;
        }

        private void GetOneCardType(int type)
        {
            if (BottomCardTypeGrid == null) return;
            for (int i = 0; i < BottomCardTypeGrid.transform.childCount; i++)
            {
                var son = BottomCardTypeGrid.transform.GetChild(i).transform.GetChild(0).gameObject;
                if (son.activeSelf)
                {
                    son.SetActive(false);
                }
            }

            var item2 = YxWindowUtils.CreateItem(BottomSpriteItem, BottomCardTypeGrid.transform);
            int colorIndex;
            var str = GetCardType(type, out colorIndex);

            item2.spriteName = colorIndex == 0 ? CardTypeNormal : CardTypeSpecial;
            var label = item2.GetComponentInChildren<UILabel>();
            label.text = str;
            label.color = colorIndex == 0 ? new Color(186f / 255, 152f / 255, 104f / 255) : new Color(144f / 255, 82f / 255, 0);

            item2.transform.GetChild(0).gameObject.SetActive(true);
            BottomCardTypeGrid.repositionNow = true;
        }

        private string GetCardType(int type, out int colorIndex)
        {
            var cardType = "";
            colorIndex = 1;
            switch (type)
            {
                case (int)CardType.DanPai:
                    colorIndex = 0;
                    cardType = "单张";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "对子";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "顺子";
                    break;
                case (int)CardType.JinHua:
                    cardType = "金花";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "顺金";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "豹子";
                    break;
            }

            return cardType;
        }

        private void DishRoadShow()
        {
            var index = 0;
            if (_recordSpot.Count > _gdata.GameRecordNum)
            {
                index = _recordSpot.Count - _gdata.GameRecordNum;
            }

            for (int i = index; i < _recordSpot.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(DishHgRoadSpriteItem, DishRoadGrid.transform);
                item.CreatItem(_recordSpot[i]);
            }
           
            DishRoadGrid.repositionNow = true;

            if (_recordSpot.Count > LoadsShowLine[0])
            {
                DishRoadScrollBar.value = 1;
            }
        }
        public void GetOtherRoad(List<List<string>> data)
        {
            if(BigRoadParenTransform==null)return;
            var road = new RoadNodeTable(data, 6);
            CreatAllItem(road, BigRoadSpriteItem, BigRoadParenTransform, "bigRed", "bigBlack", BigRoadScrollBar, LoadsShowLine[1], true);

            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            CreatAllItem(bigEyeRoad, BigEyeRoadSpriteItem, BigEyeRoadParenTransform, "smallYQRed", "smallYQBlack", BigEyeRoadScrollBar, LoadsShowLine[2]);

            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            CreatAllItem(smallRoad, SmallRoadSpriteItem, SmallRoadParenTransform, "smallYSRed", "smallYSBlack", SmallRoadScrollBar, LoadsShowLine[2]);

            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            CreatAllItem(roachRoad, RoachRoadSpriteItem, RoachRoadParentTransform, "smallSRed", "smallSBalck", RoachRoadScrollBar, LoadsShowLine[2]);
        }

        private void CreatAllItem(RoadNodeTable road, UISprite itemSprite, Transform parent, string redShow, string blackShow, UIScrollBar bar, int sliderMaxLineCount, bool showHe = false)
        {
            var offestX = parent.GetComponent<UIGrid>().cellWidth;
            var offestY = parent.GetComponent<UIGrid>().cellHeight;
            for (int i = 0; i < road.Nodes.Count; i++)
            {
                var item = YxWindowUtils.CreateItem(itemSprite, parent);
                item.spriteName = road.Nodes[i].IsRed ? redShow : blackShow;
                var hCount = road.Nodes[i].DrawCount;
                if (showHe && hCount > 0)
                {
                    item.GetComponentInChildren<UILabel>().text = hCount.ToString();
                }
                item.transform.localPosition = new Vector3((road.Nodes[i].X - 1) * offestX, (road.Nodes[i].Y - 1) * -offestY, 0);

                if (i == road.Nodes.Count - 1)
                {
                    StartCoroutine(FlashSprite(item));
                }
            }
            if (road.LineCount.Count > sliderMaxLineCount)
            {
                bar.value = 1;
            }
        }


        public void AddOneDishRoadItem(List<string> value)
        {
            _recordSpot.Add(value);
            if (_recordSpot.Count > _gdata.GameRecordNum)
            {
                var index = _recordSpot.Count - _gdata.GameRecordNum;
                var index1 = 0;
                for (int i = index; i < _recordSpot.Count; i++)
                {
                    var item = DishRoadGrid.transform.GetChild(index1).GetComponent<HgRoadItem>();
                    item.CreatItem(_recordSpot[i]);
                    index1++;
                    if (i == _recordSpot.Count - 1)
                    {
                        StartCoroutine(FlashSprite(item));
                    }
                }
            }
            else
            {
                var item = YxWindowUtils.CreateItem(DishHgRoadSpriteItem, DishRoadGrid.transform);
                item.CreatItem(value);
                StartCoroutine(FlashSprite(item));
            }

            DishRoadGrid.repositionNow = true;

            if (_recordSpot.Count > LoadsShowLine[0])
            {
                DishRoadScrollBar.value = 1;
            }

           var list = _recordSpot;
            AddOneOtherRoadItem(list);
        }

        private void AddOneOtherRoadItem(List<List<string>> data)
        {
            if(BigRoadParenTransform==null)return;
            var road = new RoadNodeTable(data, 6);
            CreatOneItem(road, BigRoadSpriteItem, BigRoadParenTransform, "bigRed", "bigBlack", BigRoadScrollBar, LoadsShowLine[1], true);

            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            CreatOneItem(bigEyeRoad, BigEyeRoadSpriteItem, BigEyeRoadParenTransform, "smallYQRed", "smallYQBlack", BigEyeRoadScrollBar, LoadsShowLine[2]);

            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            CreatOneItem(smallRoad, SmallRoadSpriteItem, SmallRoadParenTransform, "smallYSRed", "smallYSBlack", SmallRoadScrollBar, LoadsShowLine[2]);

            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            CreatOneItem(roachRoad, RoachRoadSpriteItem, RoachRoadParentTransform, "smallSRed", "smallSBalck", RoachRoadScrollBar, LoadsShowLine[2]);
        }

        private void CreatOneItem(RoadNodeTable road, UISprite itemSprite, Transform parent, string redShow, string blackShow, UIScrollBar bar, int sliderMaxLineCount, bool showHe = false)
        {
            var creatRoadCount = road.Nodes.Count - 1;
            if (creatRoadCount < 0) return;
            var item = YxWindowUtils.CreateItem(itemSprite, parent);
            item.spriteName = road.Nodes[creatRoadCount].IsRed ? redShow : blackShow;
            var hCount = road.Nodes[creatRoadCount].DrawCount;
            if (showHe && hCount > 0)
            {
                item.GetComponentInChildren<UILabel>().text = hCount.ToString();
            }
            var offestX = parent.GetComponent<UIGrid>().cellWidth;
            var offestY = parent.GetComponent<UIGrid>().cellHeight;
            item.transform.localPosition = new Vector3((road.Nodes[creatRoadCount].X - 1) * offestX, (road.Nodes[creatRoadCount].Y - 1) * -offestY, 0);

            bar.value = road.LineCount.Count > sliderMaxLineCount ? 1 : 0;
            StartCoroutine(FlashSprite(item));
        }


        IEnumerator FlashSprite(UISprite obj)
        {
            var item = obj.GetComponent<TweenAlpha>();

            item.PlayForward();
            var needTime = item.duration;
            yield return new WaitForSeconds(needTime * 3);
            item.enabled = false;
        }
        IEnumerator FlashSprite(HgRoadItem obj)
        {
            var item = obj.GetComponent<TweenAlpha>();

            item.PlayForward();
            var needTime = item.duration;
            yield return new WaitForSeconds(needTime * 3);
            item.enabled = false;
        }

        private void GetNext()
        {
            var list = new List<string> {"1"};
            _recordSpot.Add(list);
            var newInts = _recordSpot;
            var road = new RoadNodeTable(newInts, 6);
            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            GetNextItem(bigEyeRoad, NextBlackBigEye, NextRedBigEye, "smallYQRed", "smallYQBlack");

            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            GetNextItem(smallRoad, NextBlackSmall, NextRedSmall, "smallYSRed", "smallYSBlack");

            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            GetNextItem(roachRoad, NextBlackRoach, NextRedRoach, "smallSRed", "smallSBalck");

            _recordSpot.RemoveAt(_recordSpot.Count - 1);
        }

        private void GetNextItem(RoadNodeTable road, UISprite nextBlack, UISprite nextRed, string redStr, string blackStr)
        {
            if (nextBlack==null)return;
            if (road.Nodes.Count == 0)
            {
                nextBlack.gameObject.SetActive(false);
                nextRed.gameObject.SetActive(false);
                return;
            }
            nextBlack.gameObject.SetActive(true);
            nextBlack.spriteName = road.Nodes[road.Nodes.Count - 1].IsRed ? redStr : blackStr;
            nextRed.gameObject.SetActive(true);
            nextRed.spriteName = !road.Nodes[road.Nodes.Count - 1].IsRed ? redStr : blackStr;
        }
    }
}
