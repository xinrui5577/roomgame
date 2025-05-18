using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.Ttzkf
{
    public class CardCtrl : MonoBehaviour
    {
        public UISprite TargetSprite;
        public UISprite MahjongSprite;
        public float WaitTime = 0.5f;
        public string Mahjong = "Mahjong";
        public string MahjongBg = "MahjongBg";
        public string TurnName = "turn";

        [HideInInspector]
        public int CardValue;

        private bool _isFristMove = false;

        public void MoveOnCuoPai()
        {
            _isFristMove = true;
            var temp = transform.GetComponent<TweenPosition>();
            temp.from = transform.localPosition;
            temp.to = Vector3.zero;
            temp.PlayForward();
        }

        public void RotateOnFanPai()
        {
            if (TargetSprite == null) return;
            StartCoroutine("Turn");
        }

        private IEnumerator Turn()
        {
            var wait = new WaitForSeconds(0.05f);
            for (int i = 0; i < 7; i++)
            {
                MahjongSprite.spriteName = TurnName + i;
                MahjongSprite.MakePixelPerfect();
                if (i == 6) ShowMahjongFont();
                yield return wait;
            }
        }

        private void ShowMahjongFont()
        {
            MahjongSprite.spriteName = Mahjong;
            MahjongSprite.MakePixelPerfect();
            TargetSprite.spriteName = "A_" + CardValue;
            TargetSprite.gameObject.SetActive(true);
        }

        public void MoveAgain()
        {
            if (!_isFristMove) return;
            var temp = transform.GetComponent<TweenPosition>();
            temp.from = transform.localPosition;
            temp.to = new Vector3(50, -300, 0);
            temp.PlayForward();
            _isFristMove = false;
        }
    }
}