using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.Scene;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Item
{
    /// <summary>
    /// 晶体控制类，处理单个晶体闪烁
    /// </summary>
    public class LSCrystalItemControl : MonoBehaviour
    {
        private MeshRenderer render;

        private float _localTime = 20;

        public delegate void CryStalState();

        public CryStalState OnChangeFinished;

        public CryStalState OnChangeLocalCorlor;

        public string localType = LSConstant.Crystal_Normal;

        private LSCrystalControl manager;

        private Material _changeMaterial;

        private Material _normalMaterial;


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            manager = GetComponentInParent<LSCrystalControl>();

            if (manager == null)
            {
                Debug.Log("Manager 不存在");
            }
            manager.AddCrystal(this);

            render = this.GetComponentInChildren<MeshRenderer>();

            _normalMaterial = render.material;
        }

        public void ChangeCrystal(Type_Crystal changeType)
        {

            switch (changeType)
            {
                case Type_Crystal.NorMal:
                    localType = LSConstant.Crystal_Normal;
                    break;
                case Type_Crystal.Red:
                    localType = LSConstant.Crystal_Red;
                    break;
                case Type_Crystal.Green:
                    localType = LSConstant.Crystal_Green;
                    break;
                case Type_Crystal.Yellow:
                    localType = LSConstant.Crystal_Yellow;
                    break;
            }
        
            _changeMaterial = App.GetGameManager<LswcGamemanager>().ResourseManager.GetMaterial(localType);

            if (OnChangeLocalCorlor != null)
            {
                OnChangeLocalCorlor();
            }

            render.material = _changeMaterial;
        }
    }

}
