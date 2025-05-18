using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjCardItem : MonoBehaviour
    {
        public PoKerCard PoKerCard;
        public UISprite King;
        public UISprite Card;
        public bool MoveFlag;
        public bool IsTop;
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ShowCardFace();
            }
        }
        public void SetCardDepth(int depth)
        {
            depth = depth * 4;
            Card.depth = depth;
            if (King)
            {
                King.depth = depth + 1;
            }
        }

        private void ShowCardFace()
        {
            if (PoKerCard == null) PoKerCard = new PoKerCard();
            PoKerCard.SetCardId(_value);
            var cardName = string.Format("0x{0:X}", _value);
            Card.spriteName = cardName;
        }

        public void KingName(int value)
        {
            if (value == 0x51)
            {
                King.spriteName = "0x52";
            }
            else if (value == 0x61)
            {
                King.spriteName = "0x62";
            }
            else
            {
                King.gameObject.SetActive(false);
                return;
            }

            King.gameObject.SetActive(true);
        }

        public void ShowCardBack()
        {
            Card.spriteName = "CardBack";
            King.gameObject.SetActive(false);
        }

        public void MoveUp()
        {
            if (!IsTop)
            {
                MoveFlag = !MoveFlag;
                var pos = MoveFlag ? new Vector3(Card.transform.localPosition.x, 35, 0) : new Vector3(Card.transform.localPosition.x, 0, 0);
                TweenPosition.Begin(Card.gameObject, 0.1f, pos);
            }
        }

        public void Bounce()
        {
            StartCoroutine(DoBounce());
        }

        IEnumerator DoBounce()
        {
            Vector3 selfPos = transform.localPosition;
            float posY = selfPos.y;
            float target = selfPos.y + 30f;
            while (posY < target)
            {
                posY += Time.deltaTime * 256;
                if (posY >= target)
                    posY = target;
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }

            while (posY > selfPos.y)
            {
                posY -= Time.deltaTime * 256;
                if (posY <= selfPos.y)
                {
                    posY = selfPos.y;
                }
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }
        }
    }

    [Serializable]
    public class PoKerCard
    {
        /// <summary>
        /// 花色
        /// </summary>
        public int Colour;

        /// <summary>
        /// 面值
        /// </summary>
        public int Value;

        /// <summary>
        /// 传输值
        /// </summary>
        public int Id;

        public void SetCardId(int id)
        {
            Id = id;
            Colour = GetColor(id);
            Value = GetValue(id);
        }


        private int GetColor(int id)
        {
            return Colour = id >> 4;
        }

        private int GetValue(int id)
        {
            id = id & 0x0f;
            return Value = id;
        }
    }
}
