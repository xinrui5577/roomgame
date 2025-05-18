using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum TablePartsType
    {
        DnxbDirection,
        OutCardFlag,
        Saizi,
        Table,
        Timer,
        DisplayCard,
        TableSign,
        MahjongCounter,
    }

    public abstract class MahjongTablePart : MonoBehaviour
    {
        public TablePartsType PartType;

        public virtual void OnReset() { }

        public virtual void OnInitalization() { }
    }
}