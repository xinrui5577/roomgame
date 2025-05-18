using System.Collections.Generic;
using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.bjl3d
{
    public class CoinTypeInfoUI : MonoBehaviour//G 11.15
    {
        /// <summary>
        /// 选择状态
        /// </summary>
        [Tooltip("选择状态")]
        public Transform Selected;
        [Tooltip("按钮预制体")]
        public Chip ChipBtnPrefab;
        public void InitChipBtns(IList<int> anteList)
        {
            var count = anteList.Count;
            var type = App.GetGameData<Bjl3DGameData>().GameConfig.CoinType;
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
            btnTs.parent = transform;
            btnTs.localScale = Vector3.one;
            btnTs.localRotation = Quaternion.identity;
            return chipGo;
        }

        public void CoinTypeBtnClick(GameObject btn)
        {
            var btnTs = btn.transform;
            AddSelected(btnTs);
            int index;
            if (!int.TryParse(btn.name, out index)) { return;}
            App.GetGameData<Bjl3DGameData>().GameConfig.CoinType = index;
        }

        public void AddSelected(Transform p)
        {
            Selected.parent = p;
            Selected.gameObject.SetActive(true);
            Selected.localPosition = Vector3.zero;
            Facade.Instance<MusicManager>().Play("movebutton");
        }

        public void DisplaySelected(bool isShow)
        {
            Selected.gameObject.SetActive(isShow);
        }
    }
}