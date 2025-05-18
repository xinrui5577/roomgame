using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Assets.Scripts.Game.paijiu
{
    /// <summary>
    /// 显示分数的效果
    /// </summary>
    public class ShowScoreModel : MonoBehaviour
    {


        public GameObject Prefab;

        public ShowScoreType ShowType = ShowScoreType.None;

        /// <summary>
        /// 图片间的间距
        /// </summary>
        public int Space = -6;

        /// <summary>
        /// 下一个数字延迟时间
        /// </summary>
        public float Duration = 0.1f;

        public int Depth = 500;

        private readonly List<GameObject> _itemsList = new List<GameObject>();

        private int _numCount;


        bool _isPlaying;
        public void ShowScore(string score)
        {
            
            switch (ShowType)
            {
                case ShowScoreType.None:
                    break;
                case ShowScoreType.WaveJumpRightToLeft:
                    //添加跳跃组件
                    if (Prefab.GetComponent<WaveJump>() == null)
                    {
                        Prefab.AddComponent<WaveJump>();
                    }
                    InitWaveJump(score);    //图片初始化

                    if (!_isPlaying)
                    {
                        _isPlaying = true;
                        StartCoroutine(WaveJumpRtoL());
                    }
                    break;
                case ShowScoreType.WaveJumpLeftToRight:
                    //添加跳跃组件
                    if (Prefab.GetComponent<WaveJump>() == null)
                    {
                        Prefab.AddComponent<WaveJump>();
                    }

                    InitWaveJump(score);    //图片初始化
                    if(!_isPlaying)
                    {
                        _isPlaying = true;
                        StartCoroutine(WaveJumpLtoR());
                    }

                    break;
            }
        }

        /// <summary>
        /// 从右至作
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator WaveJumpRtoL()
        {
            for (int i = _numCount - 1; i >= 0 && _isPlaying; i--)
            {
                GetUIsprite(i).gameObject.SetActive(true);
                yield return new WaitForSeconds(Duration);
            }
            StopCoroutine(WaveJumpRtoL());
            _isPlaying = false;
        }

        /// <summary>
        /// 从左至右
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator WaveJumpLtoR()
        {
            for (int i = 0; i < _numCount && _isPlaying; i++)
            {
                GetUIsprite(i).gameObject.SetActive(true);
                yield return new WaitForSeconds(Duration);
            }
            StopCoroutine(WaveJumpLtoR());
            _isPlaying = false;
        }

        private void InitWaveJump(string score)
        {
            //查找一个父层级
            if(transform.childCount <= 0)
            {
                GameObject gob = new GameObject();
                gob.transform.name = ShowType.ToString();
                gob.transform.parent = transform;
                gob.transform.localScale = Vector3.one;
                gob.transform.localPosition = Vector3.zero;
            }
            Transform parent = transform.GetChild(0);

            int ttWidth = 0;
            
            //生成符号位
            string symbol = score.Substring(0, 1);    //确定符号
            //生成数字位
            char[] scoreStrArr = score.Substring(1, score.Length-1).ToCharArray();   
            UISprite sprite = GetUIsprite(0);
            sprite.gameObject.SetActive(false);
            sprite.transform.parent = parent;
            sprite.transform.localPosition = Vector3.zero;
            sprite.transform.localScale = Vector3.one;
            setSpriteName(sprite, symbol);
            ttWidth += sprite.width + Space;

            _numCount = scoreStrArr.Length + 1;      //记录一共多少个图片对象,由于有符号位,所以是Length + 1
            for (int i = 1; i < scoreStrArr.Length + 1; i++)
            {
                sprite = GetUIsprite(i);
                sprite.gameObject.SetActive(false);
                sprite.transform.parent = parent;
                sprite.transform.localPosition = Vector3.one * 0.6f;
                setSpriteName(sprite, symbol + scoreStrArr[i - 1]);
                ttWidth += sprite.width + Space;
                SetPosition(sprite, i);
            }

            //设置父层级位置
            Vector3 localPos = parent.localPosition;
            // ReSharper disable once PossibleLossOfFraction
            parent.localPosition = new Vector3((GetUIsprite(0).width - ttWidth)/2, localPos.y, localPos.z);
        }

        private void SetPosition(UISprite sprite,int index)
        {
            var preSpr = GetUIsprite(index - 1);
            var sprTran = sprite.transform;
            var prePos = preSpr.transform.localPosition;
            // ReSharper disable once PossibleLossOfFraction
            sprTran.transform.localPosition = new Vector3(prePos.x +(preSpr.width + sprite.width) / 2 + Space, prePos.y, prePos.z);
        }


        /// <summary>
        /// 设置UIsprite的图片
        /// </summary>
        /// <param name="sprite">sprite对象</param>
        /// <param name="srpiteName">图片名称</param>
        private void setSpriteName(UISprite sprite, string srpiteName)
        {
            sprite.spriteName = srpiteName;
            sprite.MakePixelPerfect();
        }

        /// <summary>
        /// 从_itemsList中获得一个UIsprite
        /// </summary>
        /// <param name="index">对象索引</param>
        /// <returns></returns>
        private UISprite GetUIsprite(int index)
        {
            var go = GetOne(index);
            UISprite sprite = go.GetComponent<UISprite>() ?? go.AddComponent<UISprite>();
            sprite.depth = Depth;
            return sprite;
        }

        /// <summary>
        /// 获取预制
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetOne(int index)
        {
            if (index >= _itemsList.Count)
            {
                var go = Instantiate(Prefab);
                go.name = "sprite " + index;
                _itemsList.Add(go);

            }
            return _itemsList[index];
        }


        public void StopPlay()
        {
            _isPlaying = false;
            StopAllCoroutines();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _itemsList.Count; i++)
            {
                GameObject go = _itemsList[i];
                go.gameObject.SetActive(false);
            }
        }

    }

        public enum ShowScoreType
        {
            None = 0,
            /// <summary>
            /// 数字逐位跳跃,从左到右
            /// </summary>
            WaveJumpLeftToRight = 1,
            /// <summary>
            /// 数字逐位跳跃,从右到左
            /// </summary>
            WaveJumpRightToLeft = 2,
        }

}