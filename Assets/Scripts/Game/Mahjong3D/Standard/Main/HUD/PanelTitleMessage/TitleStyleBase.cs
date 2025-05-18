using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class TitleStyleBase : MonoBehaviour
    {
        public TitleMessageType Type;
        public Dictionary<int, Variable> Params = new Dictionary<int, Variable>();

        public virtual void OnStartGameUpdate() { }

        public virtual void OnReconnectUpdate() { }

        public abstract void Show();
    }
}