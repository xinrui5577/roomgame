using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    public class OutCardIcon : MonoBehaviour
    {
        public GameObject JianTou;
        protected float _minY;
        protected float _maxY;
        protected float _disY = 0.1f;
        protected Vector3 JianTouPos;
        protected bool _isShow;

        void Awake()
        {
            JianTouPos = JianTou.transform.localPosition;
        }
        void Start()
        {
            JianTou.SetActive(false);
        }

        protected IEnumerator PosMove()
        {
            Transform jianTouTf = JianTou.transform;
            int sign = 1;
            while (true)
            {
                jianTouTf.localPosition += new Vector3(0, 0.01f * sign, 0);
                if (jianTouTf.localPosition.y > _maxY)
                {
                    jianTouTf.localPosition = JianTouPos + new Vector3(0, _disY, 0);
                    sign = -1;
                }
                if (jianTouTf.localPosition.y < _minY)
                {
                    jianTouTf.localPosition = JianTouPos;
                    sign = 1;
                }
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void Show(MahjongItem item)
        {
            Vector3 mjTfPos = item.transform.position;
            _minY = JianTouPos.y;
            _maxY = _minY + _disY;

            float posY = (float)(mjTfPos.y + MahjongManager.MagjongSize.z * 0.5 + 0.1);
            //移动坐标
            transform.position = new Vector3(mjTfPos.x, posY, mjTfPos.z);

            JianTou.SetActive(true);

            JianTou.transform.localPosition = JianTouPos;

            if (!_isShow)
            {
                _isShow = true;
                StartCoroutine(PosMove());
            }
        }

        public void Hide()
        {
            _isShow = false;
            JianTou.SetActive(false);
            StopAllCoroutines();
            JianTou.transform.localPosition = JianTouPos;
        }
    }
}
