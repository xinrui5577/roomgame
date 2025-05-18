using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Common.components;
using YxFramwork.Common;

namespace Assets.Scripts.Game.toubao
{
    public class ResetChip : MonoBehaviour
    {
        public Chip ChipBtnPrefab;
        public UIGrid Grid;

        public Chip InitChips(int minNum, List<int> anteList)
        {
            var anteLen = anteList.Count;
            if (anteLen < 1) return null;
            var gridTs = Grid.transform;
            var firstChip = CreateChipBtn(anteList[0], 0, gridTs);
            for (var i = 1; i < anteLen; i++)
            {
                CreateChipBtn(anteList[i], i, gridTs);
            }

            SetPos();
            return firstChip;
        }

        public Chip CreateChipBtn(int value, int bgIndex, Transform parentTs)
        {
            var chip = Instantiate(ChipBtnPrefab);
            chip.name = value.ToString();
            var chipTs = chip.transform;
            chipTs.parent = parentTs;
            chipTs.localPosition = Vector3.zero;
            chipTs.localScale = Vector3.one;
            chipTs.localRotation = Quaternion.identity;
            chip.gameObject.SetActive(true);
            chip.UpdateView(new ChipData
            {
                Value = value,
                BgId = bgIndex
            });
            var btn = chip.GetComponentInChildren<UIButton>();
            if (btn != null)
            {
                var list = new List<EventDelegate>();
                var betCtrl = App.GetGameManager<TouBaoGameManager>().BetArea;
                var ed = new EventDelegate(betCtrl, "OnBetClick");
                list.Add(ed);
                ed.parameters[0] = new EventDelegate.Parameter { obj = chip };
                EventDelegate.Add(btn.onClick, ed);
                btn.onClick = list;
            }
            return chip;
        }

        public virtual void SetPos()
        {
            if (Grid == null) return;
            Grid.repositionNow = true;
            Grid.Reposition();
        }
    }
}
