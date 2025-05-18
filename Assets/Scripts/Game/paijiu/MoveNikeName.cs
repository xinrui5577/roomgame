using UnityEngine;
using System.Collections;
using com.yxixia.utile.YxDebug;


namespace Assets.Scripts.Game.paijiu
{
    public class MoveNikeName : MonoBehaviour
    {

        /// <summary>
        /// 显示名字的区域,一般设置在父层级
        /// </summary>
        [SerializeField]
        private UIPanel _panel;

        /// <summary>
        /// 显示名字的Label
        /// </summary>
        private UILabel _nameLabel;

        private Vector3 _startPos;

        public float MoveStep = 2;

        public float RepeatRate = 5;

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            _nameLabel = GetComponent<UILabel>();
            if (_nameLabel == null)
            {
                YxDebug.Log("No Label! No Name!");
                return;
            }
            if (_panel == null)
            {
                _panel = GetComponent<UIPanel>() ?? GetComponentInParent<UIPanel>();
            }
            if (_panel == null)
                return;

            _nameLabel.UpdateNGUIText();
            int labelWidth = _nameLabel.width;
            float viewWidth = _panel.GetViewSize().x;
            Vector3 pos = transform.localPosition;
            _startPos = new Vector3(viewWidth / -2, pos.y, pos.z);

            if (labelWidth > viewWidth + _nameLabel.fontSize && !_isMoving)
            {
                _isMoving = true;
                _nameLabel.pivot = UIWidget.Pivot.Left;
                StartCoroutine(LabelMove());
            }
            else
            {
                _isMoving = false;
                _nameLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
                _nameLabel.pivot = UIWidget.Pivot.Center;
                _nameLabel.width = 100;
                _nameLabel.transform.localPosition = Vector3.zero;
            }
        }


        bool _isMoving;

        IEnumerator LabelMove()
        {
            int labelWidth = _nameLabel.width;
            float leftLimit = -labelWidth;
            float newPosX = _startPos.x;
            bool goon = true;
            while (_isMoving)
            {
                while (goon)
                {
                    newPosX += MoveStep;

                    if (newPosX < leftLimit)
                    {
                        newPosX = leftLimit;
                        MoveStep = -1 * MoveStep;
                    }
                    else if (newPosX > _startPos.x)
                    {
                        newPosX = _startPos.x;
                        MoveStep = -1 * MoveStep;
                        goon = false;
                    }
                    transform.localPosition = new Vector3(newPosX, _startPos.y, _startPos.z);

                    yield return null;
                }

                yield return new WaitForSeconds(RepeatRate);
                goon = true;
            }
        }

        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            StopMoving();
            _nameLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
        }

        void StopMoving()
        {
            _isMoving = false;
            transform.localPosition = _startPos;
            StopCoroutine(LabelMove());
        }
    }
}