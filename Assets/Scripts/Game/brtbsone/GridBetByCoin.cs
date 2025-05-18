using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.components;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brtbsone
{
    public class GridBetByCoin : MonoBehaviour
    {
        public Chip ChipBtnPrefab;
        public UIGrid[] Grid;
        public int ChipNum = 3;
        public Color[] LabelColors;

        public Vector3 BetChipSceal = Vector3.one;

        public void SetShowBet(long coin)
        {

        }
        public Chip InitChips(int minNum, List<int> anteList)
        {
            var anteLen = anteList.Count;
            int num = anteLen % ChipNum == 0 ? anteLen / ChipNum : anteLen / ChipNum + 1;
            num = num > 2 ? 2 : num;//定死只有两个grid
            if (anteLen < 1) return null;
            var gridTs = Grid[0].transform;
            var firstChip = CreateChipBtn(anteList[0], 0, gridTs);
            //for (int i = 1; i < ChipNum; i++)
            //{
            //    CreateChipBtn(anteList[i], i, gridTs);
            //}
            //for (int i = ChipNum; i < anteLen; i++)
            //{
            //    CreateChipBtn(anteList[i], i, Grid[1].transform);
            //}
            int temp = 0;
            for (int i = num - 1; i >= 0; i--)
            {
                for (int j = temp * ChipNum; j < anteLen - i * ChipNum; j++)
                {
                    if (i == num - 1 && j == 0) continue;
                    if (Grid[temp] != null) CreateChipBtn(anteList[j], j, Grid[temp].transform);
                }
                temp++;
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
            chipTs.localScale = BetChipSceal;
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
                btn.normalSprite = chip.GetChipName();
                var list = new List<EventDelegate>();
                var betCtrl = App.GetGameManager<BrttzGameManager>().BetCtrl;
                var ed = new EventDelegate(betCtrl, "OnBetClick");
                list.Add(ed);
                ed.parameters[0] = new EventDelegate.Parameter { obj = chip };
                EventDelegate.Add(btn.onClick, ed);
                //btn.onClick = list;
            }
            var label = chip.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                label.color = LabelColors[bgIndex];
            }
            return chip;
        }

        public virtual void SetPos()
        {
            if (Grid == null) return;
            foreach (var g in Grid)
            {
                g.repositionNow = true;
                g.Reposition();
            }
        }

        /// <summary>
        /// 设置下注按钮UIButton的状态
        /// </summary>
        /// <param name="couldClick">是否可以点击</param>
        public virtual void SetChipBtnsState(bool couldClick)
        {
            foreach (var g in Grid)
            {
                Transform chipsParent = g.transform;
                var buttons = chipsParent.GetComponentsInChildren<UIButton>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    var btn = buttons[i];
                    btn.GetComponent<BoxCollider>().enabled = couldClick;
                    btn.state = couldClick ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
                }
            }
        }
    }
}