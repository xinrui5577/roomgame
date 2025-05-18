using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn.brnn_skin01
{
    public class ChipItem : MonoBehaviour
    {
        readonly List<UISprite> _sprites = new List<UISprite>();

        void CleanSprites()
        {
            foreach (UISprite sp in _sprites)
            {
                Destroy(sp.gameObject);
            }
            _sprites.Clear();
        }

        /// <summary>
        /// 显示筹码面值
        /// </summary>
        /// <param name="betValue">筹码的面值</param>
        /// <param name="spriteName">筹码的贴图</param>
        /// <param name="depth">筹码的层级</param>
        public void ShowBetValue(int betValue, string spriteName, int depth)
        {

            var sprite = gameObject.GetComponent<UISprite>();
            sprite.spriteName = spriteName;
            sprite.MakePixelPerfect();
            sprite.depth = depth;

            var label = GetComponentInChildren<UILabel>();
            //使用图文混合
            if (label == null)
            {
                YxDebug.Log("No Label !!");
                return;
            }
            label.depth = depth + 1;
            label.text = YxUtiles.ReduceNumber(betValue,2, true);
        }
    }
}