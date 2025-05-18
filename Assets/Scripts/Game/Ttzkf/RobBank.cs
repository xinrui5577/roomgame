using System.Globalization;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Ttzkf
{
    public class RobBank : MonoBehaviour
    {
        public RobItem RobItem;
        public UIGrid RobGrid;
        public GameObject RobParent;
        private readonly int[] _num = new[] { 1, 2, 3 };
        /// <summary>
        /// 初始化RobItem
        /// </summary>
        public void SetRobValue(int[] robAnte)
        {
            var gmanager = App.GetGameManager<TtzGameManager>();

            for (int i = 0; i < RobGrid.transform.childCount; i++)
            {
                if (RobGrid.transform.GetChild(i).gameObject.name != "0")
                {
                    Destroy(RobGrid.transform.GetChild(i).gameObject);
                }

            }
            for (int i = 0; i < robAnte.Length; i++)
            {
                var obj = YxWindowUtils.CreateItem(RobItem, RobGrid.transform);
                obj.Multiple.text = robAnte[i] + "倍";
                obj.Chip.spriteName = "icon_00" + _num[i % 3];
                obj.name = robAnte[i].ToString(CultureInfo.InvariantCulture);
                NguiAddOnClick(obj.gameObject, gmanager.RobBankSelect, obj.gameObject);
            }
            RobGrid.repositionNow = true;
            RobGrid.Reposition();
            RobParentCtrl();
        }

        public void RobParentCtrl(bool isShow = false)
        {
            RobParent.SetActive(isShow);
        }

        /// <summary>
        /// 为NGUI对象添加点击事件
        /// </summary>
        /// <param name="gob">点击对象</param>
        /// <param name="callback">监听事件</param>
        /// <param name="id">ID</param>
        public void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback, GameObject id)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }

        public void CtrlRobBtnShow(bool isCtrl)
        {
            if (isCtrl)
            {
                for (int i = 0; i < RobGrid.transform.childCount; i++)
                {
                    if (i <= 1) continue;
                    RobGrid.transform.GetChild(i).GetComponent<RobItem>().Bg.color = new Color(161f / 255f, 161f / 255f, 161f / 255f);
                    RobGrid.transform.GetChild(i).GetComponent<BoxCollider>().enabled = false;
                }
            }
            else
            {
                for (int i = 0; i < RobGrid.transform.childCount; i++)
                {
                    if (i <= 1) continue;
                    RobGrid.transform.GetChild(i).GetComponent<BoxCollider>().enabled = true;
                    RobGrid.transform.GetChild(i).GetComponent<RobItem>().Bg.color = Color.white;
                }
            }

        }
    }
}
