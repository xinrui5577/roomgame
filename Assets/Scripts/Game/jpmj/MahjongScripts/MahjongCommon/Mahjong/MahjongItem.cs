using System;
using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongItem : MonoBehaviour
    {
        public GameObject Model;
        public GameObject Mesh;

        /// <summary>
        /// 小鸡飞蛋选择标记
        /// </summary>
        public bool IsChosenXJFD;
        public bool HideMahjongOnDrag = false;

        protected BoxCollider _box;
        protected UserContorl _uc;
        protected MouseRoll _roll;

        protected MahjongIcon _icon;

        public MahjongIcon Icon
        {
            get { return _icon; }
        }

        public UserContorl UserCtl
        {
            get { return _uc; }
        }

        public BoxCollider Collider
        {
            get { return _box; }
        }

        public MouseRoll Roll
        {
            get { return _roll; }
        }

        public int MahjongIndex;                //排序用 先根据牌值 然后 根据index
        public int Value;

        public override string ToString()
        {
            return "值:" + Value + "；编号：" + MahjongIndex;
        }

        private Quaternion RotaToAcross = Quaternion.Euler(0, 0, -90);

        private Quaternion SignRota = Quaternion.Euler(0,0,0);
        private Vector3 SignPos = new Vector3(0.048f, 0.088f, -0.115f);
        private Vector3 SignScale = new Vector3(0.4f, 0.4f);

        protected GameObject _sign;

        /// <summary>
        /// 杠头角标
        /// </summary>
        private GameObject _gangTou;

        private GameObject _arrow;
        public bool IsArrowActive
        {
            get
            {
                return _arrow != null && _arrow.activeSelf;
            }
        }

        private bool _isAcross;

        public bool IsAcross
        {
            get { return _isAcross; }
            set
            {
                _isAcross = value;
                if(value)
                {
                    transform.localRotation = RotaToAcross;
                }
            }
        }

        public void SetMjRota(float x,float y,float z)
        {
            transform.localRotation = Quaternion.Euler(x,y,z);
        }

        public void HideArrow()
        {
            if (_arrow != null) _arrow.SetActive(false);
        }

        public void SetArrow(int acrossIndex)
        {
            if (!GameConfig.IsShowArrow || UtilData.CurrGamePalyerCnt<3) return;

            if (_arrow == null)
            {
                _arrow = MahjongManager.Instance.GetArrow();
            }
            _arrow.SetActive(true);
            _arrow.transform.parent = Mesh.transform;
            _arrow.transform.localPosition = new Vector3(0.12f, 0.2f, -0.115f);
            _arrow.transform.localScale = new Vector3(0.04f, 0.04f); 
            var rotation = Quaternion.Euler(0, 0, 0);

            switch (acrossIndex)
            {
                case 0:
                    rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case 1:

                    rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 2:
                    rotation = Quaternion.Euler(0, 0, -90);
                    break;

            }
            _arrow.transform.localRotation = rotation;
            _arrow.layer = Mesh.layer;
        }

        public bool IsSign
        {
            get { return _sign != null; }
            set
            {
                if (value)
                {                   
                    if (_sign == null)
                    {
                        _sign = MahjongManager.Instance.GetSign();
                        _sign.transform.parent = Mesh.transform;
                        _sign.transform.localRotation = SignRota;
                        _sign.transform.localPosition = SignPos;
                        _sign.transform.localScale = SignScale;
                        _sign.layer = Mesh.layer;
                    }
                }
                else
                {
                    if (_sign != null)
                    {
                        Destroy(_sign);
                        _sign = null;
                    }                       
                }
            }
        }

        public bool IsGangTou
        {
            get { return _gangTou != null; }
            set
            {
                if (value)
                {
                    if (_gangTou == null)
                    {
                        _gangTou = MahjongManager.Instance.GetGangTou();
                        _gangTou.transform.parent = Mesh.transform;
                        _gangTou.transform.localRotation = SignRota;
                        _gangTou.transform.localPosition = SignPos;
                        _gangTou.transform.localScale = SignScale;
                        _gangTou.layer = Mesh.layer;
                    }
                }
                else
                {
                    if (_gangTou != null)
                    {
                        Destroy(_gangTou);
                        _gangTou = null;
                    }
                }
            }
        }


        protected bool _lock;
        public bool Lock
        {
            get { return _lock; }
            set
            {
                _lock = value;
                if (_lock)
                {
                    ShowGray();
                }
                else
                {
                    ShowNormal();
                }
            }

        }

        public void ShowGray()
        {
            Mesh.GetComponent<MeshRenderer>().material = MahjongManager.Instance.MahjongMaterialGay;
        }

        public void ShowNormal()
        {
            Mesh.GetComponent<MeshRenderer>().material = MahjongManager.Instance.MahjongMaterialNomal;
        }

        public void ShowGreen()
        {
            Mesh.GetComponent<MeshRenderer>().material = MahjongManager.Instance.MahjongMaterialGreen;
        }

        public void ChooseXJFD()
        {
            if (_lock)
            {
                return;
            }
            if (IsChosenXJFD)
            {
                ShowNormal();
            }
            else
            {
                ShowGreen();
            }
            IsChosenXJFD = !IsChosenXJFD;
        }

        public void ChangeToHardLayer(bool isHard)
        {
            if (isHard)
            {
                UtilFunc.ChangeLayer(transform, 9);
            }
            else
            {
                UtilFunc.ChangeLayer(transform, 0);
            }
        }

        //互换模型与值
        public void Exchange(MahjongItem item)
        {
            GameObject objTemp = Model;
            Model = item.Model;
            item.Model = objTemp;

            ResetModel();
            item.ResetModel();

            int tempValue = item.Value;
            int tempIndex = item.MahjongIndex;

            item.Value = Value;
            Value = tempValue;

            item.MahjongIndex = MahjongIndex;
            MahjongIndex = tempIndex;

            if (item.IsSign) item.IsSign = false;
            if (IsSign) IsSign = false;
        }

        public virtual void SetMahjongScript()
        {
            if (_box == null)
            {
                if (gameObject.GetComponent<BoxCollider>())
                {
                    _box = gameObject.GetComponent<BoxCollider>();
                }
                else
                {
                    _box = gameObject.AddComponent<BoxCollider>();
                }
                _box.size = MahjongManager.MagjongSize;
            }

            if (_icon == null)
            {
                if (gameObject.GetComponent<MahjongIcon>())
                {
                    _icon = gameObject.GetComponent<MahjongIcon>();
                }
                else
                {
                    _icon = gameObject.AddComponent<MahjongIcon>();
                }
            }

            if (_uc == null)
            {
                if (gameObject.GetComponent<UserContorl>())
                {
                    _uc = gameObject.GetComponent<UserContorl>();
                }
                else
                {
                    _uc = gameObject.AddComponent<UserContorl>();
                }
            }

            if (_roll == null)
            {
                if (gameObject.GetComponent<MouseRoll>())
                {
                    _roll = gameObject.GetComponent<MouseRoll>();
                }
                else
                {
                    _roll = gameObject.AddComponent<MouseRoll>();
                }
                _roll.Target = Model.transform;
            }

        }

        public void RemoveMahjongScript()
        {
            if (_roll != null)
            {
                DestroyImmediate(_roll);
                _roll = null;
            }

            if (_uc != null)
            {
                DestroyImmediate(_uc);
                _uc = null;
            }

            if (_box != null)
            {
                DestroyImmediate(_box);
                _box = null;
            }
            
            if (_icon != null)
            {
                _icon.OnRemoveComponent();
                DestroyImmediate(_icon);
                _icon = null;
            }
            else
            {  
                //删除复制出来的麻将的 听或游金Icon
                for (int i = 0; i < transform.childCount; i++)
                {
                    var obj = transform.GetChild(i);
                    if (obj.name.Contains("Clone"))
                    {
                        DestroyImmediate(obj.gameObject);                  
                    }
                }  
            }
        }

        public void SetThowOutCall(DVoidTransform throwOutCall)
        {
            if (_uc != null)
            {
                _uc.OnThrowOut = throwOutCall;
            }
        }

        public void SetSelectCall(UserContorl.OnSelect selectCall)
        {
            if (_uc != null)
            {
                _uc.ItemSelect = selectCall;
            }
        }

        public void ChangeWaitForXjfd(bool status)
        {
            if (_uc!=null)
            {
                _uc.XjfdStatus = status;
            }
        }

        public void Reset()
        {
            Model.transform.localScale = Vector3.one;
            Model.transform.localPosition = Vector3.zero;
            Model.transform.localRotation = Quaternion.identity;
        
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Mesh.transform.localScale = Vector3.one;
            Mesh.transform.localPosition = Vector3.zero;
            Mesh.transform.localRotation = Quaternion.identity;

            ChangeToHardLayer(false);

            RemoveMahjongScript();

            ShowNormal();

            IsSign = false;
            
            IsGangTou = false;

            _isAcross = false;

            _lock = false;

            HideArrow();
        }

        public void ResetModel()
        {
            Model.transform.parent = transform;
            UtilFunc.ChangeLayer(transform);
            Model.transform.localScale = Vector3.one;
            Model.transform.localPosition = Vector3.zero;
            Model.transform.localRotation = Quaternion.identity;
            Mesh = Model.transform.GetChild(0).gameObject;

            if (_roll != null)
            {
                _roll.Target = Model.transform;
            }

        }

        private Coroutine _rotoToCor;
        public void RotaTo(Vector3 to, float time, float delayTime = 0, DVoidNoParam callBack = null)
        {
            if (gameObject.activeInHierarchy)
            {
                if (_rotoToCor!=null)
                {
                    StopCoroutine(_rotoToCor);
                }
                _rotoToCor= StartCoroutine(RotoTo(to, time, delayTime, callBack));
            }
            else
            {
                Quaternion tquat = Quaternion.Euler(to);
                transform.localRotation = tquat;
                if (callBack!=null)
                {
                    callBack();
                }
            }
        }

        public void RotaTo(Vector3 from, Vector3 to, float time, float delayTime = 0, DVoidNoParam callBack = null)
        {
            //delaytime足够短，默认为直接亮出，不要从下向上翻的动画了
            if (delayTime < 0.001)
            {
                transform.localRotation = Quaternion.Euler(to);
                return;
            }
            transform.localRotation = Quaternion.Euler(from);
            RotaTo(from,time,delayTime,callBack);
        }
        public IEnumerator RotoTo(Vector3 to, float time,float delayTime = 0, DVoidNoParam callBack = null)
        {
            if (Math.Abs(delayTime) > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }

            float val = 0;
            float bTime = Time.time;

            Quaternion fqua = transform.localRotation;
            Quaternion tquat = Quaternion.Euler(to);

            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;

                transform.localRotation = Quaternion.Lerp(fqua, tquat, smoothval);
                yield return 2;
            }

            if (callBack != null)
                callBack();
        }


        public void MoveToAction(Vector3 to, float time, DVoidNoParam callBack = null)
        {
            StartCoroutine(MoveTo(to, time, callBack));
        }

        private IEnumerator MoveTo(Vector3 moveTo, float time, DVoidNoParam callBack = null)
        {
            float val = 0;
            float bTime = Time.time;
            var fpos = transform.localPosition;
            var tpos = moveTo;

            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                yield return 2;
            }

            if (callBack != null) callBack();
        }

        public void GetInMjAction(Vector3 moveTo, DVoidString callBack)
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(PickUp(moveTo, callBack));
            } 
        }

        private Vector3 _pickUpRotation = new Vector3(0, 0, 10);
        private Vector3 _pickUpPos = new Vector3(0,MahjongManager.MagjongSize.y+GameConfig.PickUpMoreHeight, 0);
        private float PickUpTime = GameConfig.PickUpTime;
        private IEnumerator PickUp(Vector3 moveTo, DVoidString callBack)
        {
            var val = 0.0f;
            var bTime = Time.time;

            var fpos = transform.localPosition;
            var tpos = fpos + _pickUpPos;

            var fqua = Model.transform.localRotation;
            var tquat = Quaternion.Euler(_pickUpRotation);
            while (val < PickUpTime)
            {
                val = Time.time - bTime;
                var smoothval = val / PickUpTime;
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);

                Model.transform.localRotation = Quaternion.Slerp(fqua, tquat, smoothval);
                yield return 2;
            }
            yield return new WaitForSeconds(0.15f);
            yield return MoveTo(moveTo, callBack);
        }

        private IEnumerator MoveTo(Vector3 moveTo, DVoidString callBack)
        {
            var val = 0.0f;
            var bTime = Time.time;
            var fpos = transform.localPosition;
            var tpos = new Vector3(moveTo.x, fpos.y, fpos.z);

            var time = 0.3f;//Vector3.Distance(fpos, tpos) / MoveToSpeed;

            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                yield return 2;
            }
            if (callBack!=null) callBack("MoveFinish");
            yield return new WaitForSeconds(0.15f);
            yield return PutDown(moveTo,callBack);
        }

        private IEnumerator PutDown(Vector3 moveTo,DVoidString callBack)
        {
            float val = 0;
            float bTime = Time.time;

            var fpos = transform.localPosition;
            var tpos = new Vector3(fpos.x, moveTo.y, fpos.z);

            var fqua = Model.transform.localRotation;
            var tquat =  Quaternion.Euler(0, 0, 0);

            while (val < PickUpTime)
            {
                val = Time.time - bTime;
                var smoothval = val / PickUpTime;
                transform.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                Model.transform.localRotation = Quaternion.Slerp(fqua, tquat, smoothval);
                yield return 2;
            }

            callBack("PutDownFinish");
        }
    }
}
