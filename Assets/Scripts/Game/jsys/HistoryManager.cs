using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jsys
{
    public class HistoryManager : MonoBehaviour
    {
        /// <summary>
        /// 图片Image属性
        /// </summary>
        public Image[] ImgSprite;

        /// <summary>
        /// 图片sprite信息
        /// </summary>

        public Sprite[] Sprites;
        private readonly int[] _histroyNums = new int[10];
        protected void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                _histroyNums[i] = -1;
                if (_histroyNums[i] == -1)
                {
                    ImgSprite[i].gameObject.SetActive(false);
                }
            }
        }

        //初始化的时候显示历史纪录
        public void ShowHistory(int[] history)
        {
            for (var i = 0; i < _histroyNums.Length; i++)
            {
                if (history[i] == -1)
                {
                    history[i] = 9;
                    ImgSprite[i].gameObject.SetActive(false);
                }
                else
                {
                    ImgSprite[i].gameObject.SetActive(true);
                }
                ImgSprite[i].sprite = Sprites[history[i]];
            }
        }
        private int _index;
        //正常游戏时历史记录的变化
        public void ShowNewHistory(int pos)
        {
            App.GetGameData<JsysGameData>().History[_index] = pos;
            _index++;
            _index %= 10;
            for (int i = 0; i < _histroyNums.Length; i++)
            {
                ImgSprite[i].sprite = Sprites[App.GetGameData<JsysGameData>().History[(i + _index) % 10]];
            }
        }
    }
}

