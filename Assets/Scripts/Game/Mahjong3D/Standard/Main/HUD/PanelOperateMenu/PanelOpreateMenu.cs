using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelOpreateMenu), UIPanelhierarchy.Popup)]
    public class PanelOpreateMenu : UIPanelBase, IUIPanelControl<OpreateMenuArgs>
    {
        public List<OperateButtonItem> Btns;

        public override void OnContinueGameUpdate()
        {
            HideButtons();
        }

        public override void OnReadyUpdate()
        {
            HideButtons();
        }

        public void HideButtons()
        {
            base.Close();
            for (int i = 0; i < Btns.Count; i++)
            {
                Btns[i].gameObject.SetActive(false);
            }
        }

        public void Open(OpreateMenuArgs args)
        {
            base.Open();
            if (null == args) return;
            List<KeyValuePair<int, bool>> opMenu = args.OpMenu;
            for (int i = 0; i < Btns.Count; i++)
            {
                Btns[i].SetActive(opMenu);
            }
        }

        public override void Close()
        {
            base.Close();
            GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public void OnChiClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpChi);
            Close();
        }

        public void OnPengClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpPeng);
            Close();
        }

        public void OnGangClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2OpGang);
            Close();
        }

        public void OnHuClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpHu);
            GameCenter.EventHandle.Dispatch<AiAgencyArgs>((int)EventKeys.AiAgency, (param) => param.State = false);
            Close();
        }

        public void OnGuoClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpGuo);
            Close();
        }

        public void OnTingClick()
        {
            ChooseTing(HandcardStateTyps.ChooseTingCard);
            GameCenter.EventHandle.Dispatch<AiAgencyArgs>((int)EventKeys.AiAgency, (param) => param.State = false);
            Close();
        }

        public void OnTingDaiguClick()
        {
            ChooseTing(HandcardStateTyps.Daigu);
            Close();
        }

        public void OnTingNiuClick()
        {
            ChooseTing(HandcardStateTyps.ChooseNiuTing);
            Close();
        }

        /// <summary>
        /// 潜江 游金
        /// </summary>
        public void OnYoujinClick()
        {
            ChooseTing(HandcardStateTyps.Youjin);
            Close();
        }

        public void OnLaiZiGangClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpLaiZiGang);
            Close();
        }

        public void OnXjfdClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpXJFD);
            Close();
        }

        public void OnJueGangClick()
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.C2SOpJueGnag);
            Close();
        } 

        protected void ChooseTing(HandcardStateTyps type)
        {
            var tingList = GameCenter.DataCenter.OneselfData.TingList;
            var groups = GameCenter.Scene.MahjongGroups;
            if (tingList.Count != 0)
            {
                ChooseCgArgs args = new ChooseCgArgs()
                {
                    Type = ChooseCgArgs.ChooseType.ChooseTing,
                    CancelTingAction = () => { groups.PlayerHand.SetHandCardState(HandcardStateTyps.Normal); }
                };
                //通知UI提示选择   
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowChooseOperate, args);
                groups.PlayerHand.SetHandCardState(type, tingList);
            }
        }
    }
}