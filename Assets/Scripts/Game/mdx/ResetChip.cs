using System.Collections.Generic;
using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.mdx
{
    public class ResetChip : MonoBehaviour
    {
        public Chip ChipBtnPrefab;
        public Chip FastBetBtnPrefab;
        public UIGrid PrincipalGrid;
        public UIGrid[] LineGrids;
        public UIButton FastBetBtn;

        public int LineLimit = 5;

        public List<GameObject> ChipBtnList;

        public Chip InitChips(int minNum, List<int> anteList)
        {
            CleanBtnList();
            var anteLen = anteList.Count;
            if (anteLen < 1) return null;
            int gridIndex = 0;
            var gridTs = LineGrids[gridIndex].transform;
            var firstChip = CreateChipBtn(ChipBtnPrefab,anteList[0],0, gridTs);
            ChipBtnList.Add(firstChip.gameObject);
            for (var i = 1; i < anteLen; i++)
            {
                gridIndex = i/LineLimit;
                gridTs = LineGrids[gridIndex].transform;
                var btn = CreateChipBtn(ChipBtnPrefab,anteList[i],i, gridTs);
                ChipBtnList.Add(btn.gameObject);
                gridTs.gameObject.SetActive(true);
            }
            //快速下注按钮
            var fast = CreateFastBetBtn(FastBetBtnPrefab, 0, LineGrids[(anteLen + 1)/ LineLimit].transform);
            FastBetBtn = fast.GetComponent<UIButton>();
            SetPos();
            return firstChip;
        }

        void CleanBtnList()
        {
            foreach (var btn in ChipBtnList)
            {
                Destroy(btn.gameObject);
            }
            ChipBtnList.Clear();
        }

        public Chip CreateChipBtn(Chip prefab, int value, int bgIndex,Transform parentTs)
        {

            var chipData = new ChipData
            {
                Value = value,
                BgId = bgIndex
            };
           
            var chip = GetOne(prefab, parentTs, chipData);

            var btn = chip.GetComponentInChildren<UIButton>();
            if (btn == null) return chip;

            btn.pressed = Color.white * .7f;
            btn.hover = Color.white;

            btn.normalSprite = chip.GetChipName();

            var list = new List<EventDelegate>();
            var betCtrl = App.GetGameManager<MdxGameManager>().BetCtrl;
            var ed = new EventDelegate(betCtrl, "OnBetClick");
            list.Add(ed);
            ed.parameters[0] = new EventDelegate.Parameter {obj = chip};
            EventDelegate.Add(btn.onClick, ed);
            btn.onClick = list;
            return chip;
        }

        public Chip CreateFastBetBtn(Chip prefab, int value, Transform parentTs)
        {

            var chip = GetOne(prefab, parentTs, null);

            var btn = chip.GetComponentInChildren<UIButton>();
            if (btn == null) return chip;

            btn.pressed = Color.white * .7f;
            btn.hover = Color.white;

            var list = new List<EventDelegate>();
            var betCtrl = App.GetGameManager<MdxGameManager>().BetCtrl;
            var ed = new EventDelegate(betCtrl, "OnClickFastBetBtn");
            list.Add(ed);
            ed.parameters[0] = new EventDelegate.Parameter { obj = chip };
            EventDelegate.Add(btn.onClick, ed);
            btn.onClick = list;
            return chip;
        }



        Chip GetOne(Chip prefab, Transform parent,ChipData chipData)
        {
            var chip = Instantiate(prefab);
            chip.name = chipData == null? prefab.name : chipData.Value.ToString();

            var chipTs = chip.transform;
            chipTs.parent = parent;
            chipTs.localPosition = Vector3.zero;
            chipTs.localScale = Vector3.one;
            chipTs.localRotation = Quaternion.identity;
            chip.gameObject.SetActive(true);

            chip.UpdateView(chipData);
            return chip;
        }

        /// <summary>
        /// 设置下注按钮UIButton的状态
        /// </summary>
        /// <param name="couldClick">是否可以点击</param>
        public void SetChipBtnsState(bool couldClick)
        {
            int len = ChipBtnList.Count;
            for (int i = 0; i < len; i++)
            {
                var btn = ChipBtnList[i].GetComponent<UIButton>();
                SetBtnState(btn, couldClick);
            }

            if (FastBetBtn != null)
                SetBtnState(FastBetBtn, couldClick);
        }

        void SetBtnState(UIButton btn, bool couldClick)
        {
            btn.GetComponent<BoxCollider>().enabled = couldClick;
            btn.state = couldClick ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }

        public virtual void SetPos()
        {
            int len = LineGrids.Length;

            for (int i = 0; i < len; i++)
            {
                var grid = LineGrids[i];
                grid.repositionNow = true;
                grid.Reposition();
            }

            if (PrincipalGrid == null) return;
            PrincipalGrid.repositionNow = true;
            PrincipalGrid.Reposition();
        }
    }
}
