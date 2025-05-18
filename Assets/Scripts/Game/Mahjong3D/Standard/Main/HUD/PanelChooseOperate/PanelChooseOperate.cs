using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelChooseOperate), UIPanelhierarchy.Popup)]

    public class PanelChooseOperate : UIPanelBase, IUIPanelControl<ChooseCgArgs>
    {
        public Button CancelBtn;
        public UiCardGroupCtrl CgGroup;

        private void Awake()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.OnTing, OnTing);
        }

        public override void OnContinueGameUpdate() { Close(); }

        public void OnTing(EvtHandlerArgs args)
        {
            base.Close();
        }

        public void Open(ChooseCgArgs args)
        {
            base.Open();
            CgGroup.Clear();
            CancelBtn.onClick.RemoveAllListeners();
            CancelBtn.onClick.AddListener(Close);
            List<int[]> cgList = args.FindList;
            if (cgList == null || cgList.Count == 0) return;
            for (int i = 0; i < cgList.Count; i++)
            {
                var valueList = new List<int>(cgList[i]);
                if (valueList.Count > 4)
                {
                    continue;
                }
                if (valueList.Count < 4 && args.OutPutCard > 0)
                {
                    valueList.Add(args.OutPutCard);
                }
                valueList.Sort((a, b) =>
                {
                    if (a > b) return 1;
                    if ((a < b)) return -1;
                    return 0;
                });
                UiCardGroup group = GameCenter.Assets.GetUIMahjongGroupBig(valueList.ToArray(), isHaveBg: true);
                CgGroup.AddUiCdGroup(group);
                int index = i;
                group.SetClickCallFunc(() =>
                {
                    args.ConfirmAction(index);
                    base.Close();
                });
            }
            CgGroup.Sort(1);
        }

        public void OnChooseTingCard(ChooseCgArgs args)
        {
            base.Open();
            CgGroup.Clear();
            CancelBtn.onClick.RemoveAllListeners();
            CancelBtn.onClick.AddListener(args.CancelTingAction);
        }

        public override void Close()
        {
            base.Close();
            CgGroup.Clear();
            GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open();
            GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        public void ClosePanel()
        {
            base.Close();
            CgGroup.Clear();
        }
    }
}