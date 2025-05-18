using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.CommonCode.ChessCommon
{
    public class UIRollText : MonoBehaviour
    {


        public UILabel LableRoll;


        public static UILabel StaticLableRoll;
        private const int RollSpeed = 50;
        private static Vector3 _bPos;
        private static bool _isRoll = false;
        private static readonly List<string> TxtList = new List<string>();

        private static UIRollText _self;

        protected void Awake() {
            StaticLableRoll = LableRoll;
            _self = this;
            _self.gameObject.SetActive(false);
        }


        public static void AddText(string txt) {
            TxtList.Add(txt);
            if (!_isRoll) {
                _self.gameObject.SetActive(true);
                ShowText();
            }
        }


        private static void ShowText() {
            if (TxtList.Count > 0) {
                _isRoll = true;
                StaticLableRoll.text = TxtList[0];
                string str = TxtList[0];
                Bounds b = NGUIMath.CalculateRelativeWidgetBounds(StaticLableRoll.panel.transform,
                    StaticLableRoll.transform);
                _bPos = new Vector3(StaticLableRoll.panel.clipRange.z/2 + b.size.x/2,
                    StaticLableRoll.transform.localPosition.y, StaticLableRoll.transform.localPosition.z);
                Vector3 endPos = new Vector3(-(StaticLableRoll.panel.clipRange.z/2 + b.size.x/2), _bPos.y, _bPos.z);
                TxtList.Remove(str);
                _self.StartCoroutine(IeRollText(endPos));
            }
            else {
                _self.gameObject.SetActive(false);
            }
        }

        private static IEnumerator IeRollText(Vector3 endPos) {
            StaticLableRoll.transform.localPosition = _bPos;
            Transform tmp = StaticLableRoll.transform;

            while (tmp.localPosition.x > endPos.x) {
                tmp.localPosition = new Vector3(tmp.localPosition.x - RollSpeed*Time.deltaTime, _bPos.y, _bPos.z);
                yield return 1;
            }
            _isRoll = false;
            ShowText();
        }
    }
}