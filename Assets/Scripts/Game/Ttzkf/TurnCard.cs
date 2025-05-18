using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TurnCard : MonoBehaviour
    {
        public UISprite[] Cards;
        public UIScrollView Drag;

        private float _closeTime;
        private bool _isHit;
        private UIDragScrollView _dragScrollView;

        protected void Awake()
        {
            _dragScrollView = Drag.transform.GetComponentInChildren<UIDragScrollView>();
        }

        public void InitFourCard()
        {
            for (var i = 0; i < Cards.Length; i++)
            {
                Cards[i].spriteName = "Ab_" + App.GetGameData<TtzGameData>().SelfCards[i];
            }
        }

        public void FinishHide()
        {
            if (_isHit) return;
            var self = App.GetGameData<TtzGameData>().GetPlayer<TtzPlayerSelf>();
            var count = self.Card.Count;
            for (int fpIndex = 0; fpIndex < count; fpIndex++)
            {
                var selfSprite = self.Card[fpIndex].GetComponent<UISprite>();
                selfSprite.spriteName = self.Card[fpIndex].Mahjong;
                selfSprite.MakePixelPerfect();
                self.Card[fpIndex].TargetSprite.spriteName = "A_" + self.Cards[fpIndex];
                self.Card[fpIndex].TargetSprite.gameObject.SetActive(true);
            }
            var target = self.Target;
            target.GetComponent<UIGrid>().Reposition();
            gameObject.SetActive(false);
            _isHit = true;
        }
        
        public void Init()
        {
            gameObject.SetActive(true);
            Drag.transform.localPosition = Vector3.zero;
            Drag.GetComponent<UIPanel>().clipOffset = Vector2.zero;
            _closeTime = 4.5f;
            StartCoroutine("DefaultShow");
            _isHit = false;
            Drag.enabled = true;
            if (_dragScrollView != null) _dragScrollView.enabled = true;
            InitFourCard();
        }

        IEnumerator DefaultShow()
        {
            yield return new WaitForSeconds(_closeTime);
            FinishHide();
        }

        IEnumerator ShowCard()
        {
            yield return new WaitForSeconds(1.5f);
            Drag.transform.localPosition = Vector3.zero;
            Drag.GetComponent<UIPanel>().clipOffset = Vector2.zero;
            Drag.ResetPosition();
            StopCoroutine("DefaultShow");
            FinishHide();
        }

        protected void Update()
        {
            if (Drag.isDragging)
            {
                var dis = Cards[0].transform.position.x - Cards[1].transform.position.x;
                if (Mathf.Abs(dis) >= 0.6f)
                {
                    if (_dragScrollView != null) _dragScrollView.enabled = false;
                    StartCoroutine(ShowCard());
                }
            }
        }
    }
}
