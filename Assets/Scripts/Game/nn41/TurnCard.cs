using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.nn41
{
    public class TurnCard : MonoBehaviour
    {
        public SkinnedMeshRenderer Plane;
        public GameObject BlackBg;
        public GameObject StartShowCard;
        public UISprite[] Pokers;
        public float MiddleValue = 0.47f;
        public float TimeCtrl = 4f;
        public float FinishTime = 3f;

        private Animator _animator;
        private Texture _cardName;
        private float _changeTime;
        private bool _isClick;
        private int _num;
        private bool _isHit;

        protected void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void InitFourCard()
        {
            for (var i = 0; i < Pokers.Length; i++)
            {
                Pokers[i].spriteName = "0x" + App.GetGameData<NnGameData>().SelfCards[i].ToString("X");
            }
        }
        public void CardValue(string cardName)
        {
            var bundleName = string.Format("pai/{0}", cardName);
            _cardName = Instantiate(ResourceManager.LoadAsset<Texture>(App.GameKey, bundleName, cardName)) ;
            Plane.materials[0].SetTexture("_MainTex_2", _cardName);
        }
        IEnumerator ShowPicture()
        {
            _num++;
            if (_num == 1 || !_isClick)
            {
                yield return new WaitForSeconds(1f);

                Plane.materials[0].SetColor("_Color_3", new Color(1, 1, 1, 0));

                yield return new WaitForSeconds(2f);

                InvokeRepeating("OnclickCard", 0, 0.1f);
                Invoke("FinishHide", FinishTime);
            }
        }

        private void FinishHide()
        {
            if (_isHit) return;
            App.GetGameData<NnGameData>().GetPlayer<NnPlayerSelf>().ShowCard41();
            if (BlackBg.activeSelf)
            {
                BlackBg.gameObject.SetActive(false);
            }
            _isHit = true;
        }
        public void OnclickCard()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (!Physics.Raycast(ray, out hitInfo)) return;
            YxDebug.LogError("是否被执行");
            CancelInvoke("OnclickCard");
            Invoke("FinishHide", 0);
        }
        protected void OnEnable()
        {
            Plane.gameObject.SetActive(true);
            StartCoroutine(DisplayObj());
            Plane.materials[0].SetColor("_Color_3", new Color(1, 1, 1, 1));
            _animator.SetFloat("speed", 1);
            _num = 0;
            _changeTime = 0;
            _isClick = false;

            StartCoroutine(DefaultShow());
            _isHit = false;
            _y = -0.86f;
            InitFourCard();
        }

        IEnumerator DisplayObj()
        {
            yield return new WaitForSeconds(0.25f);
            StartShowCard.SetActive(false);
        }
        IEnumerator DefaultShow()
        {
            yield return new WaitForSeconds(TimeCtrl);
            FinishHide();
        }

        private float _y = -0.86f;

        protected void Update()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) return;

            if (Input.GetMouseButton(0))
            {
                _isClick = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.collider.name == "closeBtn")
                    {
                        FinishHide();
                        return;
                    }
                    _changeTime = (hitInfo.point.y - _y) / 1.66f;

                    _animator.Play("Take 001", 0, _changeTime);

                    if (_changeTime >= MiddleValue)
                    {
                        StartCoroutine(ShowPicture());
                    }
                }
            }
            else
            {
                if (_changeTime <= MiddleValue)
                {
                    _animator.Play("Take 001");
                    _animator.SetFloat("speed", -1);
                }
                else
                {
                    _animator.SetFloat("speed", 1);
                    StartCoroutine(ShowPicture());
                }
            }

        }

        protected void OnDisable()
        {
            Plane.gameObject.SetActive(false);
            StartShowCard.SetActive(true);
        }
    }
}
