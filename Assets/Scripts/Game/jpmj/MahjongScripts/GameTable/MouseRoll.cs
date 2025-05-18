using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    //麻将有旋转，z轴控制了上下移动，并且z变小的时候，麻将上升
    public class MouseRoll : MonoBehaviour
    {
        public float Situ;
        public float Top;
        private DVoidNoParam _dv;// = Nothing;
        private bool _isUp;
        public Transform Target;
        public float Speed = 4;

        private MahjongIcon iconCmp;

        public bool IsUp
        {
            get { return _isUp; }
        }

        public bool IsHoverEnable = true;
        public void RollUp()
        {
            if (iconCmp != null)
                iconCmp.OnQuery(transform);

            _dv = Up;
            _isUp = true;
            Top = Situ + 0.1f;
            int value = GetComponent<MahjongItem>().Value;
            EventDispatch.Dispatch((int)GameEventId.FlagMahjong, new EventData(value));
        }

        public void RollDown()
        {
            if (_isUp)
            {
                _isUp = false;
            }
            else
            {
                _dv = Down;
            }

            EventDispatch.Dispatch((int)GameEventId.CleareFlagMahjong, new EventData());

            MahjongIcon icon = GetComponent<MahjongIcon>();
            if (null != icon)
            {
                icon.IsQuerying = false;          
                EventDispatch.Dispatch((int)UIEventId.HideQueryHulistPnl);
            }                
            
        }
#if UNITY_STANDALONE
        protected void OnHover(bool isOver)
        {
            if (!UtilData.HandMjTouchEnable)
            {
                return;
            }

            if (!IsHoverEnable) return;
          
            if (isOver)
            {
                RollUp();
            }
            else
            {
                RollDown();
            }
        }
#endif
        public void Update()
        {
            _dv();
        }
        private void Nothing()
        {

        }
        private void Up()
        {           
            Vector3 v3 = Target.localPosition;
            v3.y -= Speed;
            if (v3.y <= Top)
            {
                _dv = Nothing;
                v3.y = Top;
                if (_isUp)
                {
                    _isUp = false;
                }
                else
                {
                    _dv = Down;
                }
            }
            Target.localPosition = v3;
        }
        private void Down()
        {           
            Vector3 v3 = Target.localPosition;
            v3.y += Speed;
            if (v3.y >= Situ)
            {
                _dv = Nothing;
                v3.y = Situ;
            }
            Target.localPosition = v3;
        }
        public void Start()
        {
            iconCmp = GetComponent<MahjongIcon>();

            if (Target == null)
            {
                Target = transform.GetComponent<MahjongItem>().Model.transform;
            }
            Reset();
        }

        public void ResetPos()
        {
            _dv = Nothing;

            Vector3 v3 = Target.localPosition;
            v3.y = Situ;
            Target.localPosition = v3;

            EventDispatch.Dispatch((int)GameEventId.CleareFlagMahjong, new EventData());
        }
        public void Reset()
        {
            _dv = Nothing;
            Situ = Target.localPosition.y;
        }
    }
}
