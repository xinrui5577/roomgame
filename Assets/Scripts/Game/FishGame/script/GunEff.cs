using UnityEngine;
using System.Collections;

public class GunEff : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(EFF());
	
	}
    IEnumerator EFF()
    {
        while (true)
        {
            gameObject.transform.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.5F);
            gameObject.transform.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.5F);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
