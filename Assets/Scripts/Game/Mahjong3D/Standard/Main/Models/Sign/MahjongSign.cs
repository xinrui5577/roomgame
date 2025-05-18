using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum MahSignType
    {
        None,
        Laizi,
        Ting,
        Number,
        /// <summary>
        /// cpg 指向提示
        /// </summary>
        Vector,
        /// <summary>
        /// 任意标记
        /// </summary>
        Other,
    }

    [Serializable]
    public class SignAnchor
    {
        public Anchor Anchor;
        public Vector3 Vector3;
    }

    public class MahjongSign : MonoBehaviour
    {
        /// <summary>
        /// 锚点参数
        /// </summary>
        public SignAnchor[] Anchors;

        public List<AbsMahSignItem> Signs;

        public void SetNumberSign(int number)
        {
            var type = MahSignType.Number;
            var item = GetSign(type);

            if (number <= 1)
            {
                if (item != null) item.OnReset();
                return;
            }

            if (item == null)
            {
                item = CreateSign(type, Anchor.BottomLeft);
                item.SignType = type;
            }
            item.SetState(true);
            var sprite = GameCenter.Assets.GetSprite(type.ToString() + number);
            item.SetSprite(sprite);
        }

        public AbsMahSignItem LaiziSign(bool state)
        {
            return SetSign(MahSignType.Laizi, Anchor.TopRight, state);
        }

        public AbsMahSignItem TingSign(bool state)
        {
            return SetSign(MahSignType.Ting, Anchor.MarginTop, state);
        }

        public AbsMahSignItem VectorSign(bool state)
        {
            return SetSign(MahSignType.Vector, Anchor.MiddleCenter, state);
        }

        public AbsMahSignItem OtherSign(Anchor anchor, bool state)
        {
            return SetSign(MahSignType.Other, anchor, state);
        }

        public AbsMahSignItem SetSign(MahSignType type, Anchor acnhor, bool state)
        {
            var item = GetSign(type);
            if (item == null)
            {
                item = CreateSign(type, acnhor);
                var sprite = GameCenter.Assets.GetSprite(type.ToString());
                item.SetSprite(sprite);
            }
            item.SetState(state);
            return item;
        }

        public AbsMahSignItem GetSign(MahSignType type)
        {
            AbsMahSignItem item = null;
            for (int i = 0; i < Signs.Count; i++)
            {
                if (Signs[i].SignType == type)
                {
                    item = Signs[i];
                }
            }
            return item;
        }

        public void OnReset()
        {
            for (int i = 0; i < Signs.Count; i++)
            {
                Signs[i].SetState(false);
            }
        }

        private SignAnchor GetAnchor(Anchor anchor)
        {
            for (int i = 0; i < Anchors.Length; i++)
            {
                if (Anchors[i].Anchor == anchor)
                {
                    return Anchors[i];
                }
            }
            return new SignAnchor();
        }

        private AbsMahSignItem CreateSign(MahSignType type, Anchor anchor)
        {
            AbsMahSignItem item = Signs[0];
            if (item.SignType != MahSignType.None)
            {
                item = Instantiate(item);
                Signs.Add(item);
            }

            item.transform.SetParent(transform);
            item.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item.gameObject.layer = gameObject.layer;
            item.SetState(true);

            var pos = GetAnchor(anchor).Vector3;
            item.SetTranslate(pos);
            item.SignType = type;
            return item;
        }
    }
}