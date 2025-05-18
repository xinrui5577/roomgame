using System;
using Assets.Scripts.Common.UI;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    /// <summary>
    /// 连线版gps
    /// </summary>
    public class GpsLineWindow : YxNguiWindow
    {
        /// <summary>
        /// 线的样式
        /// </summary>
        public NguiLine LinePrefab;

        public Transform LineContainerPrefab;

        public string DisFormat = "相距约{0}{1}";
        public string LtDisFormat = "<={0}{1}";
        public string WarnDisFormat = "距离过近";

        public string Metre = "米";
        public string Kmetre = "千米";

        public bool NeedMetre = true;
        [Tooltip("线的前缀")]
        public string LinePrefix = "line_";
        [Tooltip("线距离背景的前缀")]
        public string LineTitlePrefix = "linetitle_";
        [Tooltip("正常样式名称")]
        public string LineNormalState = "normal";
        [Tooltip("无数据样式名称")]
        public string LineNoneState = "none";
        [Tooltip("警告样式名称")]
        public string LineWarringState = "warring";
        /// <summary>
        /// </summary>
        public GpsItemView[] ItemViews;

        /// <summary>
        /// </summary>
        private NguiLine[] _lines;
        private Transform _lineContainer;

        protected override void OnAwake()
        {
            base.OnAwake();
            var n = ItemViews.Length -1;
            var count = n * (n + 1) / 2;
            _lines = new NguiLine[count];
            CheckIsStart = true;
        }

        protected override void OnFreshView()
        {
            if (_lineContainer == null)
            {
                YxWindowUtils.CreateItemParent(LineContainerPrefab, ref _lineContainer);
            }
            var gdata = App.GameData;
            if (gdata == null) return;
            var dataCount = ItemViews.Length;//3
            var lineIndex = 0;

            var lineNormal = string.Format("{0}{1}", LinePrefix, LineNormalState);
            var lineTitleNormal = string.Format("{0}{1}", LineTitlePrefix, LineNormalState);
            var lineNone = string.Format("{0}{1}", LinePrefix, LineNoneState);
            var lineTitleNone = string.Format("{0}{1}", LineTitlePrefix, LineNoneState);
            var lineWarring = string.Format("{0}{1}", LinePrefix, LineWarringState);
            var lineTitleWarring = string.Format("{0}{1}", LineTitlePrefix, LineWarringState);
            var gpsMgr = Facade.GetInterimManager<YxGPSManager>();
              
            if (gpsMgr != null)
            {
                var warnDistance = gpsMgr.WarnDistance;
                var ltWarnDistance = gpsMgr.LtWarnDistance;
                var startIndex = gpsMgr.HasSelf ? 0 : 1;
                var p1Max = dataCount; 
                for (var i = 0; i < p1Max; i++)
                {
                    var p1Item = ItemViews[i];
                    if (p1Item == null) continue;
                    var p1Seat = i + startIndex;
                    p1Item.gameObject.SetActive(true);
                    var p1 = gdata.GetPlayer(p1Seat);
                    var info = p1 == null ? null : p1.Info;
                    p1Item.UpdateView(info);
                    var p1ItemOff = p1Item.DisOffId;
                    var p1NeedOff = p1ItemOff > 0;
                    for (var p2Seat = p1Seat + 1; p2Seat < dataCount; p2Seat++)//画线
                    { 
                        var distance = gpsMgr.GetDistance(p1Seat, p2Seat);
                        var p2Item = ItemViews[p2Seat];
                        if (p2Item == null)
                        {
                            continue;
                        }
                        //Debug.LogError("i:" + i + "    p1:" + p1Seat + "   p2" + p2Seat + "   lineIndex" + lineIndex); 
                        var line = GetLine(lineIndex++, p1NeedOff && p1ItemOff == p2Item.DisOffId);//YxWindowUtils.CreateItem(LinePrefab, _lineContainer);
                        
                        var p1Pos = p1Item.transform.localPosition;
                        var p2Pos = p2Item.transform.localPosition;
                        if (p1Pos.x < p2Pos.x) line.Set(p1Pos, p2Pos, true);
                        else line.Set(p2Pos, p1Pos, true);
                        if (distance<0)
                        {
                            line.SetDistanceLabel("无法获取");
                            line.SetLineSkin(lineNone);
                            line.SetTitleSkin(lineTitleNone);
                            continue;//该座位暂时没人
                        }
                        // Vector2.Distance(p1.Gps, p2.Gps);
                        string disInfo;
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (distance <= warnDistance)//警告距离
                        {
                            disInfo = GetDistanceFormat(warnDistance, WarnDisFormat);
                                              //                        line.SetDistanceLabelColor(Color.red);
                            line.SetLineSkin(lineWarring);
                            line.SetTitleSkin(lineTitleWarring);
                        }
                        else if (distance <= ltWarnDistance)//小于某个距离
                        {
                            disInfo = GetDistanceFormat(ltWarnDistance, LtDisFormat);
                            //                        line.SetDistanceLabelColor(Color.white);
                            line.SetLineSkin(lineNormal);
                            line.SetTitleSkin(lineTitleNormal);
                        }
                        else//正常距离
                        {
                            disInfo = GetDistanceFormat(distance, DisFormat);
                            //                        line.SetDistanceLabelColor(Color.white);
                            line.SetLineSkin(lineNormal);
                            line.SetTitleSkin(lineTitleNormal);
                        }
                        line.SetDistanceLabel(disInfo);
                    }
                }
            }
        }

        protected string GetDistanceFormat(double distance, string format)
        {
            return distance < 1000 && NeedMetre
//                ? string.Format(metreFormat, distance.ToString("0.##"))
//                : string.Format(kmFormat, (distance / 1000d).ToString("0.##"));
                ? string.Format(format, distance.ToString("0.##"),Metre)
                : string.Format(format, (distance / 1000d).ToString("0.##"),Kmetre);
        }


        /// <summary>
        /// 获取线
        /// </summary>
        /// <returns></returns>
        public NguiLine GetLine(int index,bool needOff)
        {
            var count = _lines.Length;
            if (index >= count) return null;
            var line = _lines[index];
            if (line != null) return line;
            line = YxWindowUtils.CreateItem(LinePrefab, _lineContainer);
            _lines[index] = line;
            line.SetDistanceLabelPos(needOff);
            return line;
        }
    }
}
