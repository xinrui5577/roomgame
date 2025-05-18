using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    [RequireComponent(typeof(UIGrid))]  
    public class GridEnableReposition : MonoBehaviour {

        protected void OnEnable()
        {
            var grid = GetComponent<UIGrid>();
            grid.repositionNow = true;
            grid.Reposition();
        }
    }
}
