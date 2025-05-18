using System.Collections;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jpmj
{
    public class JpMahjongPlayerHard : MahjongPlayerHard
    {
        /// <summary>
        /// 标记财神值
        /// </summary>
        public static int CaishenValue = UtilDef.NullMj;

        /// <summary>
        /// 牌组信息
        /// </summary>
        [SerializeField] private TableData _tableDataInfo;
        [Tooltip("手牌显示控制")]
        public HandCdScreenCtrl ScaleCtrl;

        

        protected override void OnOutPutCard(Transform transf)
        {           
            if (HasToken)
            {
                var choseMj = transf.GetComponent<MahjongItem>();

                var cpgData = _tableDataInfo.UserCpg[_tableDataInfo.PlayerSeat];
                var flag = (from data in cpgData
                            where data.Type == EnGroupType.Peng
                            select data.AllCards())
                           .Any(allCds => allCds[0] == choseMj.Value);
                if (flag)
                {
                    return;
                }

                //检查是不是财神，如果是财神值则不能打出
                if (choseMj.Value == CaishenValue)
                {
                    CheckChaiShenItem();
                    return;
                }

                base.OnOutPutCard(transf);
            }
        }


        /// <summary>
        /// 检查是否手牌里所有牌都不能出
        /// </summary>
        public bool IsAllhdCdCantOut()
        {
            var childCount = gameObject.transform.childCount;
            var cpgData = _tableDataInfo.UserCpg[_tableDataInfo.PlayerSeat];
            for (int i = 0; i < childCount; i++)
            {
                var nahjongItem = transform.GetChild(i).gameObject.GetComponent<MahjongItem>();
                if (nahjongItem != null)
                {
                    var cdValue = nahjongItem.Value;
                    var isInPengGroup = (from data in cpgData where data.Type == EnGroupType.Peng select data.AllCards()).Any(allCds => allCds[0] == cdValue);
                    if (isInPengGroup == false && cdValue != CaishenValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 捋一下手牌有么有没有赋值的财神
        /// </summary>
        public void CheckChaiShenItem()
        {
            if (CaishenValue == UtilDef.NullMj || MahjongList==null)
            {
                return;
            }

            //标记有没有赋值的麻将牌
            bool hasbug = false;
            foreach (var mjItem in MahjongList)
            {
                if (mjItem.Value == CaishenValue)
                {
                    if (mjItem.IsSign == false)
                    {
                        mjItem.IsSign = true;
                        hasbug = true;
                    }
                }
                else
                {
                    if (mjItem.IsSign)
                    {
                        mjItem.IsSign = false;
                        hasbug = true;
                    }
                }
            }

            if (hasbug)
            {

                SortMahjong();

                SetMahjongPos();
            }
        }

        /// <summary>
        /// 排序自己手牌
        /// </summary>
        public void SortHandMjPos()
        {
            SortMahjong();
            SetMahjongPos();
        }

        protected override void OutCard(MahjongItem chooseMj)
        {
            int value = chooseMj.Value;
            //通知网络 发送出牌消息
            EventDispatch.Dispatch((int)NetEventId.OnOutMahjong, new EventData(value));
            //如果打出的牌是抬起状态放下
            if (chooseMj.Roll != null && chooseMj.Roll.IsUp)
            {
                chooseMj.Roll.ResetPos();
            }
        }

        public override MahjongItem ThrowOut(int value)
        {
            CheckCdNum();
            return base.ThrowOut(value);
        }

        /// <summary>
        /// 检查自己牌数是否正确
        /// </summary>
        public void CheckCdNum()
        {
            if (!IsHdCds17)
            {
                App.GetRServer<NetWorkManager>().DoSendRejoinGame();
            }
        }

        public void CheckCdNum16()
        {
            if (!IsHdCds16)
            {
                App.GetRServer<NetWorkManager>().DoSendRejoinGame();
            }
        }

        /// <summary>
        /// 检查是否17张
        /// </summary>
        public bool IsHdCds17
        {
            get
            {
                if (_tableDataInfo != null)
                {
                    var jptabeData = _tableDataInfo as JpTableData;
                    if (jptabeData != null)
                    {
                        var cpgCdsNum = jptabeData.GetSelfCpgCdNum();
                        var totalCdsNum = GetCurHdCdsNum() + cpgCdsNum;
                        return totalCdsNum == 17;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 检查是否16张
        /// </summary>
        public bool IsHdCds16
        {
            get
            {
                if (_tableDataInfo != null)
                {
                    var jptabeData = _tableDataInfo as JpTableData;
                    if (jptabeData != null)
                    {
                        var cpgCdsNum = jptabeData.GetSelfCpgCdNum();
                        var totalCdsNum = GetCurHdCdsNum() + cpgCdsNum;
                        return totalCdsNum == 16;
                    }
                }
                return false;
            }
        }

        private IEnumerator SortHdCdLater()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            while (HasToken)
            {
                yield return new WaitForEndOfFrame();

            }
            SortMahjong();
            SetMahjongPos();
        }

        /// <summary>
        /// 获得在手里的手牌数量
        /// </summary>
        /// <returns></returns>
        private int GetCurHdCdsNum()
        {
            var len = transform.childCount;
            var k = 0;
            for (var i = 0; i < len; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    k++;
                }
            }

            return k;
        }

        #region//测试用自当打牌
        /*        public override MahjongItem GetInMahjong(int value)
        {
            StartCoroutine(AutoOutPutCd());
            return base.GetInMahjong(value);
        }

        private IEnumerator AutoOutPutCd()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            while (HasToken)
            {
                yield return new WaitForEndOfFrame();
                OnOutPutCard(LastGetIn.transform);
            }
        }*/
        #endregion
    }
}
