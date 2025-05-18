using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    /// <summary>
    /// 控制ui的size来显示,来显示不同的大小
    /// </summary>
    public class OnrdUiSizeCtrl : MonoBehaviour
    {
        [SerializeField]
        protected RectTransform[] UibgRectform;
        [SerializeField]
        protected GameObject[] ExtrInfoGobs;
        [SerializeField] 
        protected OneRdExtraMessShow[] OneRdExtraMessShows;

        [SerializeField]
        protected RectTransform PlayerPnlRecTranForm;

        struct PlayerInfoUiData
        {
            public float SizeDeltaX;
            public float SizeDeltaY;
            public Vector3 ParentGobLocalPosition;
        }

        //记录ui的原始数据
        private List<PlayerInfoUiData> _orgInfoData = new List<PlayerInfoUiData>();

        private Vector3 _orgLocalPos;

        void Start () {

            foreach (var rectTransform in UibgRectform)
            {
                var infodata = new PlayerInfoUiData();
                infodata.SizeDeltaX = rectTransform.sizeDelta.x;
                infodata.SizeDeltaY = rectTransform.sizeDelta.y;
                infodata.ParentGobLocalPosition = rectTransform.parent.gameObject.transform.localPosition;
                _orgInfoData.Add(infodata);
            }
            _orgLocalPos = PlayerPnlRecTranForm.transform.localPosition;
        }
	
        /// <summary>
        /// 重置所有
        /// </summary>
        public void ResetAll()
        {
            _isClick = false;
            ResetAllPlayerInfoUi();
            foreach (var gob in ExtrInfoGobs)
            {
                gob.SetActive(false);
            }
        }

        private void ResetAllPlayerInfoUi()
        {
            var len = UibgRectform.Length;
            for (int i = 0; i < len; i++)
            {
                UibgRectform[i].sizeDelta = new Vector2(_orgInfoData[i].SizeDeltaX, _orgInfoData[i].SizeDeltaY);
                UibgRectform[i].parent.transform.localPosition = _orgInfoData[i].ParentGobLocalPosition;
            }
            PlayerPnlRecTranForm.transform.localPosition = _orgLocalPos;

            PlayerPnlRecTranForm.localScale = new Vector3(1f, 1f, 1f);
        }


        private bool _isClick;

        /// <summary>
        /// 当点击背景板的时候
        /// </summary>
        public void OnClickBgImg(GameObject gob)
        {
            if (_isClick) return;
            _isClick = true;
            UnfoldingInfo();
        }

        /// <summary>
        /// 展开界面信息
        /// </summary>
        public void UnfoldingInfo()
        {
            var len = UtilData.CurrGamePalyerCnt;
            for (int i = 0; i < len; i++)
            {
                UibgRectform[i].sizeDelta = new Vector2(_orgInfoData[i].SizeDeltaX, _orgInfoData[i].SizeDeltaY * 1.5f);

                for (int j = i + 1; j < len; j++)
                {
                    UibgRectform[j].parent.gameObject.transform.localPosition -= new Vector3(0,_orgInfoData[j].SizeDeltaY * 0.4f, 0);
                }
                OneRdExtraMessShows[i].SetEstraCds();
            }

            if (UtilData.CurrGamePalyerCnt >= 4)
            {
                PlayerPnlRecTranForm.transform.localPosition = _orgLocalPos + new Vector3(0, 80f, 0);
                PlayerPnlRecTranForm.localScale = new Vector3(0.8f, 0.8f, 1f);
            }
        }
    }
}
