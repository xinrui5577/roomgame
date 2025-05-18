using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelExhibition), UIPanelhierarchy.Popup)]
    public class PanelExhibition : UIPanelBase
    {
        public UIBigCardItem Prefab;
        public RectTransform Group;
        public RectTransform Bg;
        private List<UIBigCardItem> mItemList = new List<UIBigCardItem>();

        public override void OnInit()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.ReplayRestart, ReplayRestart);
        }

        public void ReplayRestart(EvtHandlerArgs args) { OnReset(); }

        public override void OnContinueGameUpdate() { OnReset(); }

        public override void OnStartGameUpdate() { OnReset(); }

        public override void OnReadyUpdate() { OnReset(); }

        private void OnReset()
        {
            Close();
            for (int i = 0; i < mItemList.Count; i++)
            {
                mItemList[i].gameObject.SetActive(false);
            }
        }

        public void Open(List<int> list)
        {
            base.Open();
            //显示item
            UIBigCardItem item = null;
            for (int i = 0; i < list.Count; i++)
            {
                if (mItemList.Count - 1 >= i)
                {
                    item = mItemList[i];
                }
                else
                {
                    item = Instantiate(Prefab);
                    item.transform.ExSetParent(Group);
                    item.ExLocalRotation(new Vector3(0, 0, 180));
                    mItemList.Add(item);
                }
                item.SetCard(list[i]);
            }
            //调整布局
            Vector2 v2 = Bg.sizeDelta;
            int rowNum = (int)System.Math.Ceiling((list.Count / 6f));
            float y = 111 * rowNum + 160;
            Bg.sizeDelta = new Vector2(v2.x, y);
            if (DataCenter.Game.BaoCard == 0)
            {
                //中码声音
                MahjongUtility.PlayEnvironmentSound("biyou");
            }
        }
    }
}