using System.Globalization;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj
{
    public class OneRdExtraMessShow : MonoBehaviour
    {
        

        /// <summary>
        /// 花的信息详情
        /// </summary>
        private string _huaNameinfo="";
        [SerializeField]
        protected Text HuaNameText;


        /// <summary>
        /// 台数
        /// </summary>
        private int _taiCnt;
        [SerializeField]
        protected Text TaiCuntText;

        /// <summary>
        /// 花数
        /// </summary>
        private int _huaCnt;
        [SerializeField] protected Text HuaCutText;


        [SerializeField]
        protected Text LianZhuangInfoText;

        [SerializeField]
        protected GameObject BaoPaiGob;


        /// <summary>
        /// 补张的信息
        /// </summary>
        private readonly List<int> _extraCds = new List<int>();

        //标记赖子
        private int _laizi = UtilDef.NullMj;

        private int _gangTou = UtilDef.NullMj;

        public void SetExtraInfo(string huanameinfo,int taiCunt,int huaCnt,int[]laizis,int[] gangtouCds,int laizi,bool isbaopai,string lianzhaugnInfo,int gangtou)
        {
            _laizi = laizi;
            _gangTou = gangtou;

            _extraCds.Clear();
            if(laizis!=null)_extraCds.AddRange(laizis);
            if (gangtouCds != null) _extraCds.AddRange(gangtouCds);
            _huaNameinfo = huanameinfo;
            _taiCnt = taiCunt;
            _huaCnt = huaCnt;
            TaiCuntText.text = "台:" + _taiCnt.ToString(CultureInfo.InvariantCulture);
            HuaCutText.text = "花:" + _huaCnt.ToString(CultureInfo.InvariantCulture);
            HuaNameText.text = _huaNameinfo;

            if (LianZhuangInfoText != null) LianZhuangInfoText.text = lianzhaugnInfo;

            if (BaoPaiGob != null) BaoPaiGob.SetActive(isbaopai);
        }


        [SerializeField] protected HdCdsZone ExtraCdsZone;



        public void SetEstraCds()
        {
            ExtraCdsZone.transform.parent.gameObject.SetActive(true);
            ExtraCdsZone.ClearImmediate();
            UiCardGroup uiGroupCard = D2MahjongMng.Instance.GetGroupForJp(_extraCds.ToArray(), EnD2MjType.Up, false, _laizi,_gangTou);
            ExtraCdsZone.AddUiCdGroup(uiGroupCard);
            ExtraCdsZone.Sort(0);
            ExtraCdsZone.gameObject.transform.localPosition = new Vector3(-410f, -100, 0);
        }
    }
}
