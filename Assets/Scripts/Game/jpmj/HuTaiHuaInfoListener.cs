using System.Globalization;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jpmj
{
    public class HuTaiHuaInfoListener : MonoBehaviour
    {
        /// <summary>
        /// 用于显示台花信息
        /// </summary>
        [SerializeField]
        protected Text TaiInfoText;
        /// <summary>
        /// 用于显示台花信息
        /// </summary>
        [SerializeField]
        protected Text HuaInfoText;
        /// <summary>
        /// 用于显示台花信息
        /// </summary>
        [SerializeField]
        protected Text YuInfoText;


        /// <summary>
        /// tabledata
        /// </summary>
        [SerializeField] protected JpTableData JpTbdata;
        /// <summary>
        /// 台，花的
        /// </summary>
        [SerializeField] protected GameObject TaiHuaInfoGob;


        private GameObject _mjcdGob;

        // Use this for initialization
        void Start ()
        {
            JpTbdata.ShowTaiHuaAction = OnShowTaiHuaAction;
            JpTbdata.HideTaiHuaAction = HideTaihuaInfo;
        }

        /// <summary>
        /// 显示台花信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="opcdValue"></param>
        private void OnShowTaiHuaAction(ISFSObject data, int opcdValue = -1)
        {
            var tai = data.GetInt("tai");
            var tais = data.GetInt("tais");
            var hua = data.GetInt("hua");
            var huas = data.GetInt("huas");
            var yu = data.GetInt("cnt");


            if (_mjcdGob != null) DestroyImmediate(_mjcdGob);
            TaiInfoText.text = "台:" + tai + ", 分:" + YxUtiles.GetShowNumber(tais).ToString(CultureInfo.InvariantCulture);
            HuaInfoText.text = "花:" + hua+ ", 分:" + YxUtiles.GetShowNumber(huas).ToString(CultureInfo.InvariantCulture);
            YuInfoText.text = "余:" + yu + "张";

            if (opcdValue == -1)
            {
                if (JpTbdata.OutPutMahjong == UtilDef.NullMj)return;
                opcdValue = JpTbdata.OutPutMahjong;
            }
            var mj = D2MahjongMng.Instance.GetGroupForJp(new[] { opcdValue }, EnD2MjType.Me);
            mj.transform.SetParent(TaiHuaInfoGob.transform);
            mj.transform.localPosition = new Vector3(66, 0, 0);
            mj.transform.localScale = Vector3.one;
            _mjcdGob = mj.gameObject;

            TaiHuaInfoGob.SetActive(true);
        }



        /// <summary>
        /// 隐藏台花 ui
        /// </summary>
        private void HideTaihuaInfo()
        {
            if (_mjcdGob != null) DestroyImmediate(_mjcdGob);
            TaiHuaInfoGob.SetActive(false);
        }
    }
}
