using System.Collections.Generic;
using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class ResetChip : MonoBehaviour
    {
        public int MaxCout = 6;
        public Chip ChipBtnPrefab;
        [Tooltip("选择状态")]
        public Transform Selected;

        public void InitChipBtns(IList<int> anteList)
        {
            var count = anteList.Count;
            var type = App.GetGameData<Brnn3dGameData>().CoinType;
            for (var i = 0; i < count; i++)
            {
                var chip = CreateChipBtn(i);
                var chipData = new ChipData
                {
                    BgId = i,
                    Value = anteList[i]
                };
                chip.UpdateView(chipData);
                if (type == i)
                {
                    CoinTypeBtnClick(chip.gameObject);
                }
            }
        }

        public Chip CreateChipBtn(int type)
        {
            var chipGo = Instantiate(ChipBtnPrefab);
            chipGo.name = type.ToString();
            chipGo.gameObject.SetActive(true);
            var btnTs = chipGo.transform;
            btnTs.parent = ChipBtnPrefab.transform.parent;
            btnTs.localScale = Vector3.one;
            btnTs.localRotation = Quaternion.identity;
            return chipGo;
        }

        /// <summary>
        /// 选择筹码事件
        /// </summary>
        /// <param name="btn"></param>
        public void CoinTypeBtnClick(GameObject btn)
        {
            var btnTs = btn.transform;
            AddSelected(btnTs);
            int index;
            if (!int.TryParse(btn.name, out index)) { return; }
            App.GetGameData<Brnn3dGameData>().CoinType = index; 
        }

        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="p"></param>
        public void AddSelected(Transform p)
        {
            Selected.parent = p;
            Selected.gameObject.SetActive(true);
            Selected.localPosition = Vector3.zero;
        }
    }
}