using UnityEngine;
using System.Collections.Generic;
#pragma warning disable 414
#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class CheckShowedPanel : MonoBehaviour
    {
        [SerializeField]

        private GameObject _paiJiuPrefab;


        [SerializeField]
        private UIGrid _grid;

        private readonly List<GameObject> _paijiuList = new List<GameObject>();

        private readonly List<int> _cardValList = new List<int>();

        private bool _haveInit;



        public void ShowCheckView()
        {
            //查看是否已经初始化过
            InitViewInfo();

            gameObject.SetActive(true);
            Reposition();
        }

        private void Reposition()
        {
            _grid.hideInactive = true;
            _grid.repositionNow = true;
            _grid.Reposition();
        }

        private void InitViewInfo()
        {
            if (_cardValList.Count <= 0)
                return;

            for (int i = 0; i < _cardValList.Count; i++)
            {
                GameObject go = GetOne(i);
                PaiJiuCard paijiuCard = go.GetComponent<PaiJiuCard>();
                paijiuCard.SetCardId(_cardValList[i]);
                paijiuCard.SetCardFront();
                go.SetActive(true);
            }
        }


        private GameObject GetOne(int index)
        {
            if (_paijiuList.Count <= index)
            {
                GameObject go = Instantiate(_paiJiuPrefab);
                go.transform.parent = _grid.transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                go.SetActive(false);
                _paijiuList.Add(go);
            }

            return _paijiuList[index];
        }


        public void ResetCards(bool shouldReset)
        {
            if (shouldReset)
            {
                Reset();
            }
        }


        /// <summary>
        /// 添加显示牌过的牌
        /// </summary>
        /// <param name="cardVal">牌的值</param>
        public void AddCard(int cardVal)
        {
            _cardValList.Add(cardVal);
        }

        /// <summary>
        /// 添加显示牌过的牌
        /// </summary>
        /// <param name="cardsArr">牌值数组</param>
        public void AddCard(int[] cardsArr)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < cardsArr.Length; i++)
            {
                AddCard(cardsArr[i]);
            }
        }


        private void Reset()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _paijiuList.Count; i++)
            {
                _paijiuList[i].SetActive(false);
            }
            _cardValList.Clear();
            _haveInit = false;
        }

        public void OnClickClose()
        {
            gameObject.SetActive(false);
        }

    }
}