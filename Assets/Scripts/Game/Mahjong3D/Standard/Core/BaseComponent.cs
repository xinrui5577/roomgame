using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class BaseComponent : MonoBehaviour
    {
        public virtual void OnLoad() { }

        public virtual void OnInitalization() { }

        public virtual void OnLoadAssembly(Type type) { }

        protected virtual void Awake()
        {
            GameCenter.RegisterComponent(this);
        }
    }
}