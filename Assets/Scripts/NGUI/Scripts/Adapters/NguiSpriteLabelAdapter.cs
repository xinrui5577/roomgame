using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiSpriteLabelAdapter : YxBaseNumberAdapter
    {
        public UISprite NumberPerfab;
        public string NumberPrefix;
        public UIGrid Gird;
        private readonly List<UISprite> _numberList = new List<UISprite>();
        public Color NumberColor = Color.white;
        public Vector2 NumberScale = Vector2.one;
        private string _value;
          
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        protected override void OnText(string content)
        {
            _value = content;
            var len = _value.Length;
            var oldCount = _numberList.Count;
            var count = Mathf.Min(oldCount, len);
            var index = 0;
            for (; index < count; index++)//重用
            {
                var sp = _numberList[index];
                InitNumberSp(sp, _value[index]);
            }
            if (oldCount < len)
            {
                CreateNews(index, _value, len);
            }
            else
            {
                RemoveRemainingNumber(index);
            }
            enabled = true;
            Gird.enabled = true;
            Gird.repositionNow = true;
            Gird.Reposition();
        }

        private void CreateNews(int startIndex, string nums, int maxLen)
        {
            for (var i = startIndex; i < maxLen; i++)
            {
                var sp = Instantiate(NumberPerfab);
                _numberList.Add(sp);
                sp.color = NumberColor;
                InitNumberSp(sp, nums[i]);
            }
        }

        private void RemoveRemainingNumber(int startIndex)
        {
            var oldCount = _numberList.Count;
            for (var i = oldCount - 1; i >= startIndex; i--)
            {
                var go = _numberList[i];
                go.gameObject.SetActive(false);
                //Destroy(go.gameObject);
                //_numberList.RemoveAt(i);
            }
        }

        private void InitNumberSp(UISprite sp, char num)
        {
            var ts = sp.transform;
            ts.parent = Gird.transform;
            ts.localPosition = Vector3.zero;
            sp.spriteName = NumberPrefix + num;
            sp.MakePixelPerfect();
            ts.localScale = NumberScale;
            ts.gameObject.SetActive(true);
        }


        public override void Font(Font font)
        {
        }

        public override void SetStyle(YxLabelStyle style)
        {
            throw new NotImplementedException();
        }

        public override void SetAlignment(YxEAlignment alignment)
        {
        }

        public override int GetTextWidth(string content)
        {
            return 0;
        }

        public override void FreshStyle(YxBaseLabelAdapter labelGo)
        {
        }

        public override string Value
        {
            get { return _value; }
        }

        public override void SetAnchor(GameObject go, int left, int bottom, int right, int top)
        {
        }

        public override int Width { get; set; }
        public override int Height { get; set; }
        public override int Depth { get; set; }
        public override YxEPivot Pivot { get; set; }
        public override Color Color { get; set; }
    }
}
