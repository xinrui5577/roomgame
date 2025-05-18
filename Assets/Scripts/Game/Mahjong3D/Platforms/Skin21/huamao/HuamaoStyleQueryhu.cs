using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HuamaoStyleQueryhu : StyleQueryHu
    {
        protected override void SetGroupSize(int number)
        {
            if (number == 0) return;
            int num = number >= 4 ? 4 : number;
            float RowNum = Mathf.Ceil((float)number / 4);
            float width = Group.cellSize.x * num + Group.spacing.x * (num - 1) + 200;
            float high = Group.cellSize.y * RowNum + Group.spacing.y * (RowNum - 1) + 80f;
            Container.sizeDelta = new Vector2(width, high);
        }
    }
}
