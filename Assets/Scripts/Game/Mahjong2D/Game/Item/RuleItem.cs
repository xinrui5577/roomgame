/** 
 *文件名称:     RuleItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-02 
 *描述:         规则的perfab
 *历史记录: 
*/

using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class RuleItem : MonoBehaviour
    {
        [SerializeField]
        private UILabel _tabLabel;
        [SerializeField]
        private UILabel _contentLabel;
        [SerializeField]
        private UISprite _line;
        [SerializeField]
        private char _spliteFlag=':';

        private string _itemKey;

        private string _itemValue;

        private int _itemHeight;

        private int _maxPerLine;

        private int _baseHeight;

        private int _fontSize;

        public int  ItemHeight
        {
            get
            {
                return _itemHeight;
            }
        }

        public string ItemKey
        {
            get
            {
                return _itemKey;
            }
        }

        public string ItemValue
        {
            get
            {
                return _itemValue;
            }
        }
        void Init()
        {
            var contenWidth = _contentLabel.width;
            _fontSize=_contentLabel.font.size;
            _maxPerLine = contenWidth / _fontSize;
            _baseHeight = _tabLabel.height;
        }

        public int InitData(string itemInfo)
        {
            Init();
            string[] infos = itemInfo.Split(_spliteFlag);
            _itemKey = infos[0];
            _itemValue = infos[1];
            _tabLabel.text = _itemKey;
            _contentLabel.text = _itemValue;
            _itemHeight = (((_itemValue.Length - 1) / _maxPerLine) + 1)* _fontSize;
            if(_itemHeight< _baseHeight)
            {
                _itemHeight = _baseHeight;
            }
            return _itemHeight;
        }
    }
}
