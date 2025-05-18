using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelZhaniao), UIPanelhierarchy.Popup)]
    public class PanelZhaniao : UIPanelBase
    {
        public RectTransform Bg;
        public RectTransform Groups;
        public GameObject ObjZhongMaYes;
        public GameObject ObjZhongMaNO;

        public float ShowIntrval = 0.4f;
        public float EffectIntrval = 0.3f;

        private List<int> mZhamaList;
        private List<int> mZhongmaList;
        private List<ZhaniaoItem> mZhaniaoItems = new List<ZhaniaoItem>();

        public UIItemsManager ItemStore { get; private set; }

        public override void OnContinueGameUpdate() { OnReset(); }

        public override void OnEndGameUpdate() { OnReset(); }

        public override void OnStartGameUpdate() { OnReset(); }

        public override void OnReadyUpdate() { OnReset(); }

        private void OnReset()
        {
            Close();
            ItemStore.HideItems();
            mZhaniaoItems.Clear();
        }

        private void Awake()
        {
            ItemStore = GetComponent<UIItemsManager>();
            ItemStore.Parent = Groups;
        }

        public void Open(ZhaniaoArgs args)
        {
            base.Open();

            mZhaniaoItems.Clear();
            mZhamaList = args.ZhaMaList;
            mZhongmaList = args.ZhongMaAllList;

            SetBgLayout(mZhamaList.Count);
            for (int i = 0; i < mZhamaList.Count; i++)
            {
                var item = ItemStore.GetItem<ZhaniaoItem>(i);
                if (item != null)
                {
                    mZhaniaoItems.Add(item);
                    item.OnInit(mZhamaList[i]);
                }
            }
            // 动画
            ContinueTaskManager.NewTask().AppendFuncTask(ShowCardTask).Start();
            // 中码声音
            MahjongUtility.PlayEnvironmentSound("biyou");
        }

        private IEnumerator<float> ShowCardTask()
        {
            for (int i = 0; i < mZhaniaoItems.Count; i++)
            {
                mZhaniaoItems[i].PlayShowAnimation();
                yield return ShowIntrval;
            }
            yield return EffectIntrval;
            for (int i = 0; i < mZhaniaoItems.Count; i++)
            {
                var item = mZhaniaoItems[i];
                item.PlayZhongmaEffect(mZhongmaList.Contains(item.Card));
            }
        }

        private void SetBgLayout(int num)
        {
            int rowNum = Mathf.FloorToInt(num / 6.01f);
            Bg.sizeDelta = new Vector2(950, 290 + 100 * rowNum);
        }
    }
}