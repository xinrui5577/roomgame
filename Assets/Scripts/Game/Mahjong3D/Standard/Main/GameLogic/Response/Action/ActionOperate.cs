using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionOperate : AbsCommandAction
    {
        protected int[] mOpMenu;
        protected ContinueTaskContainer mAutoThrow;
        protected List<KeyValuePair<int, bool>> mOpMenuCache = new List<KeyValuePair<int, bool>>();

        public override void OnInit()
        {
            mOpMenu = (int[])System.Enum.GetValues(typeof(OperateMenuType));
        }

        public virtual void OperateAction(ISFSObject data)
        {
            if (ParseOperate(data))
            {
                Dispatch();
            }
        }

        protected virtual void Dispatch()
        {
            if (mOpMenuCache.Count > 0)
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.OperateMenuCtrl, new OpreateMenuArgs() { OpMenu = mOpMenuCache });
            }
            //lisi--没有op操作时关闭菜单--start--
            else
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.OperateMenuCtrl);
            }
            //lisi--end--
        }

        protected virtual bool ParseOperate(ISFSObject data)
        {
            mOpMenuCache.Clear();
            if (data.ContainsKey(RequestKey.KeySeat))
            {
                var seat = data.GetInt(RequestKey.KeySeat);
                //不是自己的op
                if (DataCenter.OneselfData.Seat != seat) return false;
            }
            if (data.ContainsKey(AnalysisKeys.KeyOp))
            {
                var opMenu = data.GetInt(AnalysisKeys.KeyOp);
                DataCenter.OperateMenu = opMenu;
                if (opMenu != 0)
                {
                    //解析按钮列表菜單
                    for (int i = 0; i < mOpMenu.Length; i++)
                    {
                        if (GameUtils.BinaryCheck(mOpMenu[i], opMenu))
                        {
                            mOpMenuCache.Add(new KeyValuePair<int, bool>(mOpMenu[i], true));
                        }
                    }
                }
            }
            else
            {
                DataCenter.OperateMenu = 0;
            }
            //上听自动打牌
            AutoThrowout();
            //听牌
            OnTingCard(data);
            return true;
        }

        protected void OnTingCard(ISFSObject data)
        {
            var list = DataCenter.OneselfData.TingList;
            list.Clear();
            //有听菜单的 tinglist
            if (data.ContainsKey("tingout"))
            {
                DataCenter.OneselfData.SetTinglist(data.TryGetIntArray("tingout"));
            }
            //不带听玩法时候 tinglist
            if (data.ContainsKey("tingoutlist"))
            {
                DataCenter.OneselfData.SetTinglist(data.TryGetIntArray("tingoutlist"));
            }
            if (DataCenter.ConfigData.MahjongQuery && MahjongUtility.TingTipCtrl == 0)
            {
                GameCenter.Shortcuts.MahjongQuery.ShowQueryTipOnOperate(list);
            }
        }

        protected void AutoThrowout()
        {
            if (null == mAutoThrow)
            {
                mAutoThrow = ContinueTaskManager.NewTask().AppendFuncTask(() => AutoThrowoutTask());
            }
            if (DataCenter.Config.AiAgency && GameCenter.Shortcuts.CheckState(GameSwitchType.AiAgency))
            {
                if (GameCenter.Shortcuts.AiAgency.Holdup(DataCenter.OperateMenu))
                {
                    if (DataCenter.CurrOpChair == 0)
                    {
                        mAutoThrow.Start();
                    }
                    else
                    {
                        GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpGuo);
                    }
                }
            }
            else if (DataCenter.CurrOpChair == 0 && DataCenter.OneselfData.IsAuto && DataCenter.OperateMenu == 0)
            {
                mAutoThrow.Start();
            }
        }

        protected IEnumerator<float> AutoThrowoutTask()
        {
            yield return Config.TimeTingPutCardWait;
            //如果没有补张，可以自动出牌
            if (!GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.HasBuzhang))
            {
                GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                {
                    param.Card = DataCenter.GetInMahjong;
                });
            }
        }
    }
}
