using System;
using System.Linq;
using UnityEngine;
using YxFramwork.ConstDefine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CcmjMahjongPlayerHand : MahjongPlayerHand
    {
        public bool IsChooseBySelf;
        private bool[] _hasDan;
        private List<int> _canChoose;
        private float _mYOffset = 0.2f;//选牌时向上偏移
        private List<MahjongContainer> _chooseXjfdList = new List<MahjongContainer>();

        /// <summary>
        /// 小鸡飞蛋点击事件
        /// </summary>
        public void ChooseXjfdOnHand()
        {
            ChooseXjfd();
        }

        /// <summary>
        /// 自己选择小鸡飞蛋点击确定的点击事件
        /// </summary>
        public void SendChoosXjfd()
        {
            var dataCenter = GameCenter.DataCenter;
            Dictionary<int, int> canDic;
            var args = new EvtHandlerArgs();
            Action<int[]> sendCall = (index) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.Dan);
                    sfs.PutIntArray(RequestKey.KeyCards, index);
                    return sfs;
                });
            };
            List<int> xjfdList = new List<int>();
            for (int i = 0; i < _chooseXjfdList.Count; i++)
            {
                xjfdList.Add(_chooseXjfdList[i].Value);
            }
            xjfdList.Sort();
            canDic = GameUtils.GetCardAmount(xjfdList);
            int yaoJiNum = canDic.ContainsKey(33) ? canDic[33] : 0;
            if (dataCenter.ConfigData.FeiDan)
            {
                if (xjfdList.Count == 1)
                {
                    if (yaoJiNum == 3)
                    {
                        sendCall(xjfdList.ToArray()); //发送小鸡飞蛋数据
                        ResetPlayerHandMahjong();
                        return;
                    }
                    if (yaoJiNum == 1)
                    {
                        bool isHasDan = false;
                        for (int i = 0; i < _hasDan.Length; i++)
                        {
                            if (_hasDan[i]) isHasDan = true;
                        }
                        if (isHasDan)
                        {
                            sendCall(xjfdList.ToArray()); //发送小鸡飞蛋数据
                            ResetPlayerHandMahjong();
                            return;
                        }
                    }
                }
                if (canDic.ContainsKey(33)) canDic.Remove(33);
            }
            if (ChackCardsIsHasDan(canDic, yaoJiNum) && IsOnlyOneInHand(xjfdList, canDic))
            {
                sendCall(xjfdList.ToArray());//发送小鸡飞蛋数据
                ResetPlayerHandMahjong();
            }
            else
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowChooseXjfd, args);
                ResetChooseXjfd();
                //发送数据错误重新选择
            }
        }

        /// <summary>
        /// 重新排序手牌麻将
        /// </summary>
        /// <param name="IsCancel"></param>
        public void ResetPlayerHandMahjong(bool IsCancel = false)
        {
            var dataCenter = GameCenter.DataCenter;
            for (int i = 0; i < _chooseXjfdList.Count; i++)
            {
                _chooseXjfdList[i].transform.localPosition -= new Vector3(0, _mYOffset, 0);
            }
            _chooseXjfdList.Clear();
            if (dataCenter.Players[dataCenter.CurrOpChair].IsAuto)
            {
                if (IsCancel)
                {
                    var lastMj = GameCenter.Scene.MahjongGroups.MahjongHandWall[dataCenter.CurrOpChair].GetLastMj();
                    lastMj.ShowNormal();
                    lastMj.SetMahjongScript();
                    lastMj.SetAllowOffsetStatus(true);
                    lastMj.SetThowOutCall(ThrowCardClickEvent);
                }
                return;
            }
            var hand = MahjongList;
            for (int i = 0; i < hand.Count; i++)
            {
                var item = hand[i];
                item.Lock = false;
                item.ShowNormal();
                item.SetMahjongScript();
                item.SetAllowOffsetStatus(true);
                item.SetThowOutCall(ThrowCardClickEvent);
            }
        }

        /// <summary>
        /// 选择小鸡飞蛋的牌
        /// </summary>
        protected void ChooseXjfd()
        {
            Action<int[]> sendCall = (index) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.Dan);
                    sfs.PutIntArray(RequestKey.KeyCards, index);
                    return sfs;
                });
            };
            var dataCenter = GameCenter.DataCenter;
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            _canChoose = FindCanXjfd();
            if (_canChoose == null || _canChoose.Count == 0)
            {
                YxDebug.LogError("本地遍历可蛋的数量为空");
                return;
            }
            var args = new XjfdListArgs();
            _canChoose.Sort();
            var canDic = GameUtils.GetCardAmount(_canChoose);
            if (_canChoose.Count == 1)//&& dic[_canChoose[0]] == 1
            {
                int can = _canChoose[0];
                if (!ChackCardsIsHasDan(can))
                {
                    sendCall(_canChoose.ToArray());
                    ResetPlayerHandMahjong();//重新设置麻将状态
                    return;
                }
            }
            else if (_canChoose.Count == 3 || _canChoose.Count == 4)
            {
                int yaoNum = 0;
                if (canDic.ContainsKey(33))
                {
                    yaoNum = canDic[33];
                    canDic.Remove(33);
                }               
                if (ChackCardsIsHasDan(canDic, yaoNum))
                {
                    sendCall(_canChoose.ToArray());
                    ResetPlayerHandMahjong();//重新设置麻将状态
                    return;
                }
            }
            if (IsChooseBySelf)
            {
                //显示选择小鸡飞蛋界面
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowChooseXjfd, args);
                var list = MahjongList;
                MahjongContainer item;
                for (int i = 0; i < list.Count; i++)
                {
                    item = list[i];
                    item.ResetPos();
                    if (!_canChoose.Contains(item.Value))
                    {
                        item.Lock = true;
                        item.RemoveMahjongScript();
                    }
                }
                for (int i = 0; i < _canChoose.Count; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        item = list[j];
                        if (item.Value == _canChoose[i])
                        {
                            item.SetMahjongScript();
                            item.SetAllowOffsetStatus(false);
                            item.SetThowOutCall(XjfdClickEvent);
                        }
                    }
                }
                return;
            }
            int yao = dic.ContainsKey(33) && dataCenter.ConfigData.FeiDan ? dic[33] : 0;
            //弹出列表
            var target = ShowChooseXjfdPanel(yao);
            if (target.Count > 0)
            {
                args.XjfdList = target;
                LockHandCard();
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowXjfdList, args);
            }
        }
        /// <summary>
        /// 检查列表中的值在手牌中是否只有一个
        /// </summary>
        /// <param name="targetList">需要检查的列表</param>
        /// <param name="targetDic">手牌字典</param>
        /// <returns></returns>
        private bool IsOnlyOneInHand(List<int> targetList, Dictionary<int, int> handDic)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == 33) continue;
                if (handDic.ContainsKey(targetList[i]) && handDic[targetList[i]] > 1)
                    return false;
            }
            return true;
        }

        private void LockHandCard()
        {
            var hand = MahjongList;
            for (int i = 0; i < hand.Count; i++)
            {
                var item = hand[i];
                item.Lock = true;
                item.RemoveMahjongScript();
            }
        }

        /// <summary>
        /// 检查查找出来的牌可否杠出,找出多张牌
        /// </summary>
        /// <param name="dic">查找出来的牌值和数量的字典</param>
        /// <param name="yaoNum">找出来的牌中幺鸡的数量</param>
        /// <returns></returns>
        private bool ChackCardsIsHasDan(Dictionary<int, int> canDic, int yaoNum = 0)
        {
            var dataCenter = GameCenter.DataCenter;
            List<int>[] tempLists = new List<int>[4];
            for (int i = 0; i < tempLists.Length; i++)
            {
                tempLists[i] = new List<int>();
                var temp = tempLists[i];
                CheckIsHaveNeed(canDic, i, temp, out temp);
            }
            for (int i = 0; i < tempLists.Length; i++)
            {
                var temp = tempLists[i];
                if (temp.Count + yaoNum != _canChoose.Count) continue;
                if (temp.Count == 0 && yaoNum != 3) continue;
                if (temp.Count == 1 && yaoNum == 0)
                    return !ChackCardsIsHasDan(temp[0]);
                if (dataCenter.ConfigData.SanFeng && temp.Count + yaoNum != 3) continue;
                int cnt = 3;
                if (CheckCardType(temp[0]) == (int)EnGroupType.XFDan && !dataCenter.ConfigData.SanFeng) cnt = 4;
                if (temp.Count + yaoNum != cnt) continue;
                int type = 0;
                if (temp.Count == 0 && yaoNum == 3)
                    type = (int)EnGroupType.YaoDan;
                else
                    type = CheckCardType(temp[0]);
                switch (type)
                {
                    case (int)EnGroupType.YaoDan:
                        if (_hasDan[0])
                            continue;
                        break;
                    case (int)EnGroupType.JiuDan:
                        if (_hasDan[1])
                            continue;
                        break;
                    case (int)EnGroupType.ZFBDan:
                        if (_hasDan[2])
                            continue;
                        break;
                    case (int)EnGroupType.XFDan:
                        if (_hasDan[3])
                            continue;
                        break;
                }
                for (int j = 0; j < temp.Count - 1; j++)
                {
                    if (temp[j] == temp[j + 1])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查找出来的牌可否杠出,找出只有一张牌
        /// </summary>
        /// <param name="value">这张牌的牌值</param>
        /// <returns></returns>
        private bool ChackCardsIsHasDan(int value)
        {
            switch (value)
            {
                case 33:
                    if (GameCenter.DataCenter.ConfigData.FeiDan)
                    {
                        if (_hasDan[0] || _hasDan[1] || _hasDan[2] || _hasDan[3])
                            return false;
                    }
                    else
                    {
                        if (_hasDan[0])
                            return false;
                    }
                    break;
                case 17:
                case 49:
                    if (_hasDan[0])
                        return false;
                    break;
                case 25:
                case 57:
                case 41:
                    if (_hasDan[1])
                        return false;
                    break;
                case 81:
                case 84:
                case 87:
                    if (_hasDan[2])
                        return false;
                    break;
                case 65:
                case 71:
                case 68:
                case 74:
                    if (_hasDan[3])
                        return false;
                    break;
            }
            return true;
        }

        /// <summary>
        /// 查找手牌中可以杠出的牌
        /// </summary>
        /// <returns></returns>
        private List<int> FindCanXjfd()
        {
            _hasDan = new bool[4];
            int yaoJiNum = 0;
            var dataCenter = GameCenter.DataCenter;
            List<int> canXjfd = new List<int>();
            List<int>[] tempXjfds = new List<int>[4];
            for (int i = 0; i < tempXjfds.Length; i++)
            {
                tempXjfds[i] = new List<int>();
            }
            var cpgData = dataCenter.OneselfData.CpgDatas;
            for (int i = 0; i < cpgData.Count; i++)
            {
                EnGroupType type = cpgData[i].Type;
                switch (type)
                {
                    case EnGroupType.YaoDan:
                        _hasDan[0] = true;
                        break;
                    case EnGroupType.JiuDan:
                        _hasDan[1] = true;
                        break;
                    case EnGroupType.ZFBDan:
                        _hasDan[2] = true;
                        break;
                    case EnGroupType.XFDan:
                        _hasDan[3] = true;
                        break;
                }
            }
            if (dataCenter.Players[dataCenter.CurrOpChair].IsAuto)
            {
                canXjfd.Add(dataCenter.GetInMahjong);
                return canXjfd;
            }
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            for (int i = 0; i < _hasDan.Length; i++)
            {
                var tempXjfd = tempXjfds[i];
                if (!dataCenter.ConfigData.YaoJiuDan && i <= 1 && !dataCenter.ConfigData.FeiDan)
                {
                    continue;
                }
                if (i == 0 && dataCenter.ConfigData.FeiDan && dic.ContainsKey(33))
                {
                    canXjfd.Add(33);
                    yaoJiNum = dic.ContainsKey(33) ? dic[33] : 0;
                    dic.Remove(33);
                }
                if (_hasDan[i])
                {
                    CheckIsHaveNeed(dic, i, canXjfd, out canXjfd);
                }
                else
                {
                    CheckIsHaveNeed(dic, i, tempXjfd, out tempXjfd);
                }
            }
            if (!dataCenter.OneselfData.IsTuiDan && !dataCenter.ConfigData.ZanDan)
                return canXjfd;
            for (int i = 0; i < tempXjfds.Length; i++)
            {
                var temp = tempXjfds[i];
                if (temp != null && temp.Count > 0)
                {
                    temp.Sort();
                    int diffcultNum = 0;
                    if (temp.Count > 0)
                    {
                        diffcultNum++;
                    }
                    for (int j = 0; j < temp.Count - 1; j++)
                    {
                        if (temp[j] != temp[j + 1])
                        {
                            diffcultNum++;
                        }
                    }

                    if (dataCenter.ConfigData.FeiDan)
                    {
                        diffcultNum += yaoJiNum;
                    }
                    int cnt = 3;
                    if (!dataCenter.ConfigData.SanFeng && temp[0] >= 65 && temp[0] <= 74)
                    {
                        cnt = 4;
                    }
                    if (diffcultNum >= cnt)
                    {
                        for (int j = 0; j < temp.Count; j++)
                        {
                            canXjfd.Add(temp[j]);
                        }
                    }
                }
            }
            return canXjfd;
        }

        /// <summary>
        /// 查找字典中可以杠出的值
        /// </summary>
        /// <param name="dic">需要查找的字典</param>
        /// <param name="num">杠出去的类型</param>
        /// <param name="target">需要查找的列表</param>
        /// <param name="need">返回的需要的列表</param>
        /// <param name="feiDan"></param>
        private void CheckIsHaveNeed(Dictionary<int, int> dic, int num, List<int> target, out List<int> need)
        {
            need = target;
            switch (num)
            {
                case 0://幺
                    if (dic.ContainsKey(17))
                        need.Add(17);
                    if (dic.ContainsKey(33))
                        need.Add(33);
                    if (dic.ContainsKey(49))
                        need.Add(49);
                    break;
                case 1://九
                    if (dic.ContainsKey(25))
                        need.Add(25);
                    if (dic.ContainsKey(41))
                        need.Add(41);
                    if (dic.ContainsKey(57))
                        need.Add(57);
                    break;
                case 2://中发白
                    if (dic.ContainsKey(81))
                        need.Add(81);
                    if (dic.ContainsKey(84))
                        need.Add(84);
                    if (dic.ContainsKey(87))
                        need.Add(87);
                    break;
                case 3://东南西北
                    if (dic.ContainsKey(65))
                        need.Add(65);
                    if (dic.ContainsKey(68))
                        need.Add(68);
                    if (dic.ContainsKey(71))
                        need.Add(71);
                    if (dic.ContainsKey(74))
                        need.Add(74);
                    break;
            }
        }

        /// <summary>
        /// 检查杠出的类型
        /// </summary>
        /// <param name="card">牌值</param>
        /// <returns></returns>
        private int CheckCardType(int card)
        {
            int cardType = 0;
            switch (card)
            {
                case 33:
                    if (GameCenter.DataCenter.ConfigData.FeiDan)
                    {
                        cardType = (int)EnGroupType.XiaoJi;
                    }
                    else
                    {
                        cardType = (int)EnGroupType.YaoDan;
                    }
                    break;
                case 17:
                case 49:
                    cardType = (int)EnGroupType.YaoDan;
                    break;
                case 25:
                case 57:
                case 41:
                    cardType = (int)EnGroupType.JiuDan;
                    break;
                case 81:
                case 84:
                case 87:
                    cardType = (int)EnGroupType.ZFBDan;
                    break;
                case 65:
                case 68:
                case 71:
                case 74:
                    cardType = (int)EnGroupType.XFDan;
                    break;
                default:
                    YxDebug.LogError("给的牌非幺牌，无法杠出");
                    break;
            }
            return cardType;
        }

        /// <summary>
        /// 需要选择小鸡飞蛋时手牌的点击事件
        /// </summary>
        /// <param name="transf">点击手牌</param>
        private void XjfdClickEvent(Transform transf)
        {
            var item = transf.GetComponent<MahjongContainer>();
            if (item.Lock) return;
            bool isFound = false;
            for (int i = 0; i < _chooseXjfdList.Count; i++)
            {
                if (item.Value == _chooseXjfdList[i].Value && item.MahjongIndex == _chooseXjfdList[i].MahjongIndex)
                {
                    isFound = true;
                    _chooseXjfdList.RemoveAt(i);
                    item.transform.localPosition -= new Vector3(0, _mYOffset, 0);
                    break;
                }
            }
            if (!isFound)
            {
                _chooseXjfdList.Add(item);
                item.transform.localPosition += new Vector3(0, _mYOffset, 0);
            }
        }

        private List<int[]> ShowChooseXjfdPanel(int yaoNum)
        {
            List<int[]> target = new List<int[]>();
            List<int>[] cardsList = new List<int>[4];
            for (int i = 0; i < cardsList.Length; i++)
            {
                cardsList[i] = new List<int>();
            }
            List<int> tempCan = new List<int>();
            for (int i = 0; i < _canChoose.Count - 1; i++)
            {
                if (_canChoose[i] == _canChoose[i + 1])
                    continue;
                if (i == _canChoose.Count - 2 && _canChoose[i] != _canChoose[i + 1])
                    tempCan.Add(_canChoose[i + 1]);
                tempCan.Add(_canChoose[i]);
            }
            if (_canChoose.Count == 1) tempCan.Add(_canChoose[0]);
            var db = GameCenter.DataCenter;
            for (int i = 0; i < tempCan.Count; i++)
            {
                var num = tempCan[i];
                if (!ChackCardsIsHasDan(num))
                {
                    target.Add(new[] { num });
                }
                switch (num)
                {
                    case 17:
                    case 49:
                        if (db.ConfigData.YaoJiuDan)
                        {
                            cardsList[0].Add(num);
                        }
                        break;
                    case 33:
                        if (db.ConfigData.YaoJiuDan && !db.ConfigData.FeiDan)
                        {
                            cardsList[0].Add(num);
                        }
                        break;
                    case 25:
                    case 41:
                    case 57:
                        if (db.ConfigData.YaoJiuDan)
                        {
                            cardsList[1].Add(num);
                        }
                        break;
                    case 81:
                    case 84:
                    case 87:
                        cardsList[2].Add(num);
                        break;
                    case 65:
                    case 68:
                    case 71:
                    case 74:
                        cardsList[3].Add(num);
                        break;
                }
            }

            int needNum;
            for (int i = 0; i < cardsList.Length; i++)
            {
                if (!db.ConfigData.YaoJiuDan && i <= 1) continue;
                if (cardsList[i].Count + yaoNum < 3) continue;
                needNum = (i == cardsList.Length - 1 && !db.ConfigData.SanFeng) ? 4 : 3;
                if (_hasDan[i]) continue;
                int targetNum;
                var tempList = cardsList[i];
                if (i == 1 && tempList.Count == 3)
                {
                    target.Add(tempList.ToArray());
                    continue;
                }
                for (int j = 0; j <= yaoNum; j++)
                {
                    var list = new List<int[]>();
                    targetNum = needNum - j;
                    for (int k = 0; k < tempList.Count; k++)
                    {
                        var temp = list.Where(p => p.Length < targetNum).ToList();
                        int[] nArr = { tempList[k] };
                        list.Add(nArr);
                        for (int l = 0; l < temp.Count; l++)
                        {
                            list.Add(temp[l].Concat(nArr).ToArray());
                        }
                    }
                    var outList = list.OrderByDescending(p => p.Length).ToList();
                    for (int k = 0; k < outList.Count; k++)
                    {
                        if (outList[k].Length == targetNum)
                        {
                            if (outList[k].Length == needNum)
                            {
                                target.Add(outList[k]);
                            }
                            else
                            {
                                var lis = outList[k].ToList();
                                for (int l = 0; l < needNum - targetNum; l++)
                                {
                                    lis.Add(33);
                                }
                                target.Add(lis.ToArray());

                            }
                            j = yaoNum + 1;
                            break;
                        }
                    }
                }
            }
            return target;
        }

        private void ResetChooseXjfd()
        {
            for (int i = 0; i < _chooseXjfdList.Count; i++)
            {
                _chooseXjfdList[i].transform.localPosition -= new Vector3(0, _mYOffset, 0);
            }
            _chooseXjfdList.Clear();
        }
    }
}