using DG.Tweening;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.jsys
{
    public class Pathfinding : MonoBehaviour
    {
        private NavMeshAgent _animal;
        public Transform Target;
        public Transform[] Points;
        public Camera Camera;
        private int _randomNum;
        public bool ChoosePath = false;
        public bool IsWiner = false;
        private Animation _animation;
        public int AnimalKind;

        // Use this for initialization
        protected void Start()
        {
            _animal = gameObject.GetComponent<NavMeshAgent>();
            _animation = gameObject.GetComponentInChildren<Animation>();
            StartCoroutine(GotoPath(0));
        }

        IEnumerator GotoPath(int time)
        {
            if (_animation == null) { yield break; }
            if (time > 0)
            {
                _animation.Play("rest");
            }
            yield return new WaitForSeconds(time);
            if (_animation == null) { yield break; }
            if (_animal.isOnNavMesh && _animal.isActiveAndEnabled)
            {
                _animation.Play("move");
                int rang = RandomNum();
                Target.position = Points[rang].position;
                _animal.SetDestination(Target.position);
                _animal.stoppingDistance = 1;
                ChoosePath = true;
            }
        }

        public void AnimalGotoPath()
        {
            StartCoroutine(GotoPath(2));
        }

        private int RandomNum()
        {
            var canChangebg = App.GetGameManager<JsysGameManager>().GoldSharkGameUIMgr;
            int rang = Random.Range(0, 8);
            if (rang == _randomNum || canChangebg.PathNum == rang)
            {
                RandomNum();
            }
            _randomNum = rang;
            canChangebg.PathNum = rang;
            return _randomNum;
        }

        public void ScaleToBig()
        {
            transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
        }
        public void ScaleToSmall()
        {
            transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 1);
        }
        public void ScaleToNomal()
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 1);
        }

        // Update is called once per frame
        protected void Update()
        {
            if (_animal == null) return;
            if (!_animal.enabled || !ChoosePath) return;
            if (!(_animal.remainingDistance <= 1)) return;
            ChoosePath = false;
            AnimalGotoPath();
        }
    }
}
