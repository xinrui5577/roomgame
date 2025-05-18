using UnityEngine;

namespace Assets.Scripts.Common.UI
{
    [ExecuteInEditMode]
    public class NguiLine : MonoBehaviour
    {
        /// <summary>
        /// 开始端点
        /// </summary>
        public GameObject StartPoint;
        /// <summary>
        /// 结束端点
        /// </summary>
        public GameObject EndPoint;
        /// <summary>
        /// 距离
        /// </summary>
        public UILabel DistanceLabel;
        /// <summary>
        /// 距离背景
        /// </summary>
        public UISprite DistanceBackground;
        /// <summary>
        /// 起点
        /// </summary>
        public Transform From;
        /// <summary>
        /// 结点
        /// </summary>
        public Transform To;
        /// <summary>
        /// 起点
        /// </summary>
        [SerializeField]
        private Vector2 _from;
        /// <summary>
        /// 结点
        /// </summary>
        [SerializeField]
        private Vector2 _to;
        /// <summary>
        /// 宽度
        /// </summary>
        public int _width = 10;
        /// <summary>
        /// 线得素材
        /// </summary>
        [SerializeField]
        private UIBasicSprite _line;
        /// <summary>
        /// 深度
        /// </summary>
        [SerializeField]
        private int _depth;

        public Vector2 OffLocalPostion = new Vector2(175,0);
        /// <summary>
        /// 样式线默认方向
        /// </summary>
        public YxEArrangement LineDefaultDirecton = YxEArrangement.Horizontal;
        private void Awake()
        {
            InitLine(Line);
        }

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            if (From != null)
            {
                _from = From.localPosition;
                _isLocal = true;
            }
            if (To != null)
            {
                _to = To.localPosition;
                _isLocal = true;
            }
            Set(_from, _to, _isLocal);
        }

        public UIBasicSprite Line
        {
            get
            {
                if (_line == null)
                {
                    _line = gameObject.GetComponent<UIBasicSprite>() ?? gameObject.AddComponent<UITexture>();
                    var texture = new Texture2D(1, 1);
                    texture.SetPixel(1, 1, Color.black);
                    texture.Apply();
                    _line.mainTexture = texture;
                    InitLine(_line);
                }
                return _line;
            }
            set { _line = value; }
        }

        [ContextMenu("Init Line")]
        private void InitLine(UIBasicSprite line)
        {
            line.pivot = LineDefaultDirecton == YxEArrangement.Horizontal ? UIWidget.Pivot.Left : UIWidget.Pivot.Top;
            line.type = UIBasicSprite.Type.Sliced;
            line.transform.localPosition = Vector3.zero;
            line.depth = _depth;
            SetLineWidth(Width);
        }
        private bool _isLocal;
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="isLocal"></param>
        public void Set(Vector2 from, Vector2 to, bool isLocal = false)
        {
            _isLocal = isLocal;
            _from = from;
            _to = to;
            DrawLine(_from, _to, isLocal);
        }

        public void SetDistanceLabelColor(Color color)
        {
            if (DistanceLabel == null) return;
            DistanceLabel.color = color;
        }

        public void SetDistanceLabel(string distance = "")
        {
            if (DistanceLabel == null) return;
            if (string.IsNullOrEmpty(distance)) return;
            DistanceLabel.text = distance;
        }

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                SetLineWidth(Longness);
            }
        }

        private void SetLineWidth(int longness)
        {
            if (Line == null) return;
            if (LineDefaultDirecton == YxEArrangement.Horizontal)
            {
                Line.width = longness;
                Line.height = _width;
            }
            else
            {
                Line.width = _width;
                Line.height = longness;
            }
        }

        public void SetDistanceLabelPos(bool needOff)
        {
            if (DistanceLabel == null) return;
            var ts = DistanceLabel.transform;
            var lpos = ts.localPosition;
            if (needOff)
            {
                lpos.x = OffLocalPostion.x;
                lpos.y = OffLocalPostion.y;
            }
            else
            {
                lpos.x = 0;
                lpos.y = 0;
            }
            ts.localPosition = lpos;
        }

        public int Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
                Line.depth = _depth;
            }
        }

        public int Longness { get; private set; }

        private void DrawLine(Vector2 from, Vector2 to, bool isLocal)
        {
            var parentTs = transform.parent;
            if (!isLocal)
            {
                from = parentTs.InverseTransformPoint(from);
                to = parentTs.InverseTransformPoint(to);
                transform.position = from;
            }
            else
            {
                transform.localPosition = from;
            }
            var distance = Vector2.Distance(from, to);
            Longness = (int)distance;
            SetLineWidth(Longness);
            var targetDir = to - from;
            var ts = Line.transform;
            if (LineDefaultDirecton == YxEArrangement.Horizontal)
            {
                var angle = Vector2.Angle(parentTs.right, targetDir);
                ts.localEulerAngles = new Vector3(0, 0, targetDir.y > 0 ? angle : -angle);
            }
            else
            {
                var angle = Vector2.Angle(-parentTs.up, targetDir);
                ts.localEulerAngles = new Vector3(0, 0, targetDir.x < 0 ? -angle : angle);
            }
        }


#if UNITY_EDITOR
        private Vector2 _oldfrom;
        private Vector2 _oldto;

        private void Update()
        {
            Init();
            if (_line != null && _oldfrom == _from && _oldto == _to) return;
            _oldfrom = _from;
            _oldto = _to;
            DrawLine(_from, _to, _isLocal);
        }
#endif
        public void SetLineSkin(string state)
        {
            var sp = Line as UISprite;
            if (sp!=null)
            {
                sp.spriteName = state;
            }
        }

        public void SetLineSkin(Texture texture)
        {
            var tx = Line as UITexture;
            if (tx != null)
            {
                tx.mainTexture = texture;
            }
        }

        public void SetTitleSkin(string state)
        {
            if (DistanceBackground)
            {
                DistanceBackground.spriteName = state;
            }
        }
    }

    public enum YxEArrangement
    {
        Horizontal,
        Vertical
    }
}
