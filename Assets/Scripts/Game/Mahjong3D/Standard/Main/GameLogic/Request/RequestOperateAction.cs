using System.Collections.Generic;
using YxFramwork.ConstDefine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public struct FindGangData
    {
        public int type;
        public int ttype;
        public int[] cards;
    }

    public class RequestOperateAction : AbsCommandAction

    {
        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpHu, OnHu);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpGuo, OnGuo);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpPeng, OnPeng);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpChi, OnChi);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2OpGang, OnGang);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpJueGnag, OnJueGnag);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpLaiZiGang, OnLaiZiGang);
            GameCenter.EventHandle.Subscriber((int)EventKeys.C2SOpXJFD, OnXJFD);
        }

        public void OnHu(EvtHandlerArgs args)
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                if (GameCenter.DataCenter.SelfCurrOp)
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.ZiMo);
                }
                else
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.CPG);
                    sfs.PutInt(AnalysisKeys.KeyTType, NetworkProls.Hu);
                }
                return sfs;
            });
        }

        public void OnGuo(EvtHandlerArgs args)
        {
            //清理听牌提示
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);

            //听牌时点过按钮
            if (DataCenter.CurrOpChair == 0 && DataCenter.OneselfData.IsAuto && GameUtils.BinaryCheck(OperateKey.OpreateHu, DataCenter.OperateMenu))
            {
                GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                {
                    param.Card = DataCenter.GetInMahjong;
                });
            }
            else
            {
                if (DataCenter.CurrOpChair == 0 && GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.AiAgency))
                {
                    GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                    {
                        param.Card = DataCenter.GetInMahjong;
                    });
                }
                else
                {
                    GameCenter.Network.OnRequestC2S((sfs) =>
                    {
                        sfs.PutInt(RequestKey.KeyType, NetworkProls.CPG);
                        sfs.PutInt(AnalysisKeys.KeyTType, OperateKey.OpreateNone);
                        return sfs;
                    });
                }
            }
        }

        public void OnPeng(EvtHandlerArgs args)
        {
            GameCenter.Network.OnRequestC2S((sfs) =>
            {
                int[] find = FindCanPeng();
                sfs.PutInt(RequestKey.KeyType, NetworkProls.CPG);
                sfs.PutInt(AnalysisKeys.KeyTType, OperateKey.OpreatePeng);
                sfs.PutIntArray(RequestKey.KeyCards, find);
                return sfs;
            });
        }

        private int[] FindCanPeng()
        {
            var dataCenter = GameCenter.DataCenter;
            int[] arr = null;
            int outcard = dataCenter.ThrowoutCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            if (dic.ContainsKey(outcard) && dic[outcard] >= 2)
            {
                arr = new[] { outcard, outcard };
            }
            return arr;
        }

        public void OnChi(EvtHandlerArgs args)
        {
            var dataCenter = GameCenter.DataCenter;
            List<int[]> findList = FindCanChi();
            Action<int> sendCall = (index) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.CPG);
                    sfs.PutInt(AnalysisKeys.KeyTType, OperateKey.OpreateChi);
                    sfs.PutIntArray(RequestKey.KeyCards, findList[index]);
                    return sfs;
                });
            };
            if (findList.Count > 1)
            {
                ChooseCgArgs cgArgs = new ChooseCgArgs()
                {
                    FindList = findList,
                    ConfirmAction = sendCall,
                    OutPutCard = dataCenter.ThrowoutCard,
                    Type = ChooseCgArgs.ChooseType.ChooseCg,
                };
                //通知UI提示选择   
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowChooseOperate, cgArgs);
            }
            else
            {
                sendCall(0);
            }
        }

        private List<int[]> FindCanChi()
        {
            var dataCenter = GameCenter.DataCenter;
            var findList = new List<int[]>();
            int outcard = dataCenter.ThrowoutCard;
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            if (dic.ContainsKey(outcard - 1) && outcard - 1 != laizi)
            {
                if (dic.ContainsKey(outcard - 2) && outcard - 2 != laizi)
                {
                    findList.Add(new[] { outcard - 2, outcard - 1 });
                }
                if (dic.ContainsKey(outcard + 1) && outcard + 1 != laizi)
                {
                    findList.Add(new[] { outcard - 1, outcard + 1 });
                }
            }
            if (dic.ContainsKey(outcard + 1) && dic.ContainsKey(outcard + 2) && outcard + 1 != laizi && outcard + 2 != laizi)
            {
                findList.Add(new[] { outcard + 1, outcard + 2 });
            }
            return findList;
        }

        public void OnGang(EvtHandlerArgs args)
        {
            var dataCenter = GameCenter.DataCenter;
            var findList = FindCanGang();
            Action<int> sendCall = (index) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    if (findList[index].type != DefaultUtils.DefInt)
                        sfs.PutInt(RequestKey.KeyType, findList[index].type);
                    if (findList[index].ttype != DefaultUtils.DefInt)
                        sfs.PutInt(AnalysisKeys.KeyTType, findList[index].ttype);
                    if (findList[index].cards != null && findList[index].ttype != NetworkProls.CPGZhuaGang)
                        sfs.PutIntArray(RequestKey.KeyCards, findList[index].cards);
                    else if (findList[index].cards != null && findList[index].ttype == NetworkProls.CPGZhuaGang)
                        sfs.PutInt(RequestKey.KeyOpCard, findList[index].cards[0]);
                    return sfs;
                });
            };
            //如果找到的杠 大于1
            if (findList.Count > 1)
            {
                var gangcard = dataCenter.GangCard;
                if (gangcard.Count > 0)
                {
                    //根据server 指定杠牌
                    var tempFindList = new List<FindGangData>();
                    for (int i = 0; i < gangcard.Count; i++)
                    {
                        var gang = findList.Find(d => d.cards[0] == gangcard[i]);
                        tempFindList.Add(gang);
                    }
                    findList = tempFindList;
                }
                gangcard.Clear();

                //如果手中又四张赖子牌， 过滤赖子牌
                int laizi = GameCenter.DataCenter.Game.LaiziCard;
                if (findList.Exists(d => d.cards[0] == laizi))
                {
                    findList.RemoveAll(d => d.cards[0] == laizi);
                }

                if (findList.Count == 1)
                {
                    sendCall(0);
                }
                else
                {
                    ChooseCgArgs cgArgs = new ChooseCgArgs()
                    {
                        ConfirmAction = sendCall,
                        OutPutCard = dataCenter.ThrowoutCard,
                        Type = ChooseCgArgs.ChooseType.ChooseCg,
                    };
                    cgArgs.FindList = GetGangList(findList);
                    //通知UI提示选择   
                    GameCenter.EventHandle.Dispatch((int)EventKeys.ShowChooseOperate, cgArgs);
                }
            }
            else
            {
                sendCall(0);
            }
        }

        protected virtual List<int[]> GetGangList(List<FindGangData> list)
        {
            var arr = new List<int[]>();
            for (int i = 0; i < list.Count; i++)
            {
                arr.Add(list[i].cards);
            }
            return arr;
        }

        protected virtual List<FindGangData> FindCanGang()
        {
            int checkValue;
            var dataCenter = GameCenter.DataCenter;
            var findList = new List<FindGangData>();
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            Func<int, bool> checkPengGang = (value) =>
            {
                if (dic.ContainsKey(value) && dic[value] >= 3)
                {
                    int[] temp = new int[dic[value]];
                    for (int i = 0; i < dic[value]; i++)
                    {
                        temp[i] = value;
                    }
                    var data = new FindGangData
                    {
                        type = NetworkProls.CPG,
                        ttype = NetworkProls.CPGPengGang,
                        cards = temp
                    };
                    findList.Add(data);
                    return true;
                }
                return false;
            };
            Func<bool> checkHandCardGang = () =>
            {
                bool ret = false;
                foreach (KeyValuePair<int, int> keyValuePair in dic)
                {
                    if (keyValuePair.Value > 3 && (laizi != keyValuePair.Value))
                    {
                        ret = true;
                        var data = new FindGangData
                        {
                            type = NetworkProls.SelfGang,
                            ttype = NetworkProls.CPGAnGang,
                            cards = new[] { keyValuePair.Key, keyValuePair.Key, keyValuePair.Key, keyValuePair.Key }
                        };
                        findList.Add(data);
                    }
                }
                return ret;
            };
            Func<int, bool> checkZhuaGang = (value) =>
            {
                CpgData temp;
                var findResult = false;
                List<CpgData> cpgs = dataCenter.Players[0].CpgDatas;
                for (int i = 0; i < cpgs.Count; i++)
                {
                    temp = cpgs[i];
                    if (temp.Type == EnGroupType.Peng && (value == temp.Card || dic.ContainsKey(temp.Card)))
                    {
                        var data = new FindGangData
                        {
                            type = NetworkProls.SelfGang,
                            ttype = NetworkProls.CPGZhuaGang,
                            cards = new[] { temp.Card, temp.Card, temp.Card, temp.Card }
                        };
                        findList.Add(data);
                        findResult = true;
                    }
                }
                return findResult;
            };
            Func<bool> checkXFGang = () =>
            {
                //旋风杠-只有第一轮生效
                if (dataCenter.Game.IsOutPutCard)
                    return false;
                bool zfbNoLaiZi = laizi != 81 && laizi != 84 && laizi != 87;
                //中发白-优先于-中发白
                if (dic.ContainsKey(81) && dic.ContainsKey(84) && dic.ContainsKey(87) && zfbNoLaiZi)
                {
                    var data = new FindGangData
                    {
                        type = NetworkProls.CPGXFG,
                        ttype = DefaultUtils.DefInt,
                        cards = new[] { 81, 84, 87 }
                    };
                    findList.Add(data);
                    return true;
                }
                //东南西北
                if (dic.ContainsKey(65) && dic.ContainsKey(68) && dic.ContainsKey(71) && dic.ContainsKey(74))
                {
                    var data = new FindGangData
                    {
                        type = NetworkProls.CPGXFG,
                        ttype = DefaultUtils.DefInt,
                        cards = new[] { 65, 68, 71, 74 }
                    };
                    findList.Add(data);
                    return true;
                }
                return false;
            };
            //如果是 当前用户
            if (0 == dataCenter.CurrOpChair)
            {
                //旋风杠 最优先 直接返回 不需要用户选择
                if (dataCenter.ConfigData.XFGang && checkXFGang())
                {
                    return findList;
                }
                checkValue = dataCenter.GetInMahjong;
                checkHandCardGang();
                checkZhuaGang(checkValue);
            }
            else
            {
                checkValue = dataCenter.ThrowoutCard;
                checkPengGang(checkValue);
            }
            return findList;
        }

        public void OnJueGnag(EvtHandlerArgs args)
        {
            var cards = DataCenter.OneselfData.HardCards;
            var fanCard = DataCenter.Game.FanCard;

            GameUtils.SortMahjong(cards);
            var dic = GameUtils.GetCardAmount(cards);

            Action<int[]> sendCall = (arr) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.JueGang);
                    sfs.PutInt(AnalysisKeys.KeyTType, 11);
                    sfs.PutIntArray(RequestKey.KeyCards, arr);
                    return sfs;
                });
            };

            Action<int[]> sendCallPeng = (arr) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.CPG);
                    sfs.PutInt(AnalysisKeys.KeyTType, 13);
                    sfs.PutIntArray(RequestKey.KeyCards, arr);
                    return sfs;
                });
            };

            if (DataCenter.OneselfData.Chair == DataCenter.CurrOpChair)
            {
                if (dic.ContainsKey(fanCard) && dic[fanCard] == 3) sendCall(new[] { fanCard, fanCard, fanCard });
            }
            else
            {
                if (dic.ContainsKey(fanCard) && dic[fanCard] == 2) sendCallPeng(new[] { fanCard, fanCard });
            }
        }

        public void OnLaiZiGang(EvtHandlerArgs args)
        {
            List<int> laiZiGangList = FindLaiZiGang();
            Action<int> sendCall = (index) =>
            {
                GameCenter.Network.OnRequestC2S((sfs) =>
                {
                    sfs.PutInt(RequestKey.KeyType, NetworkProls.LaiZiGang);
                    sfs.PutInt(AnalysisKeys.KeyTType, OperateKey.OperateLaiZiGang);
                    sfs.PutInt(RequestKey.KeyCard, laiZiGangList[index]);
                    return sfs;
                });
            };
            sendCall(0);
        }

        public List<int> FindLaiZiGang()
        {
            List<int> laiZiList = new List<int>();
            var dataCenter = GameCenter.DataCenter;
            int laizi = dataCenter.Game.LaiziCard;
            List<int> cards = dataCenter.OneselfData.HardCards;
            GameUtils.SortMahjong(cards);
            Dictionary<int, int> dic = GameUtils.GetCardAmount(cards);
            if (dic.ContainsKey(laizi))
            {
                laiZiList.Add(laizi);
            }
            return laiZiList;
        }

        public void OnXJFD(EvtHandlerArgs args)
        {
            var mahHand = GameCenter.Scene.MahjongGroups.PlayerHand;
            var ccMahHand = mahHand.GetComponent<CcmjMahjongPlayerHand>();
            ccMahHand.ChooseXjfdOnHand();
        }
    }
}
