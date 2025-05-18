using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public class SortRender : MonoBehaviour
    {
	    public int order;
	    public bool isUI = true;
	    void Start () 
	    {
		    if(isUI){
			    Canvas canvas = GetComponent<Canvas>();
			    if( canvas == null){
				    canvas = gameObject.AddComponent<Canvas>();
			    }
			    canvas.overrideSorting = true;
			    canvas.sortingOrder = order;
		    }
		    else
		    {
			    Renderer []renders  =  GetComponentsInChildren<Renderer>();
 
			    foreach(Renderer render in renders){
				    render.sortingOrder = order;
			    }
		    }
	    }
    }
}
