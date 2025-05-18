using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Ttzkf
{
    public class AntePanel : MonoBehaviour
    {
        /// <summary>
        /// 下注筹码的Item
        /// </summary>
        public GameObject AnteItem;
        /// <summary>
        /// 按钮的Grid
        /// </summary>
        public UIGrid AnteGrid;
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 新模式下的五人模式设置按钮的下注值
        /// </summary>
        /// <param name="antes"></param>
        public void SetNewAnte(string[] antes)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                Destroy(transform.GetChild(j).gameObject);
            }

            gameObject.SetActive(true);
            foreach (var t in antes)
            {

                var obj = YxWindowUtils.CreateGameObject(AnteItem, AnteGrid.transform);
                var lable = obj.transform.FindChild("AnteLabel").GetComponent<UILabel>();
                lable.text = YxUtiles.ReduceNumber(long.Parse(t));
                obj.name = t;
                NguiAddOnClick(obj, Click, obj);
            }
            AnteGrid.repositionNow = true;
        }
        private void Click(GameObject obj)
        {
            App.GetGameManager<TtzGameManager>().Ante(obj);

        }
        public void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback, GameObject id)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }
       
    }
}
