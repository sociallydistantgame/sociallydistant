using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.Demos.Main
{
    /// <summary>When this component or gameObject is disabled/enabled, the virtual children GameObjects will also be actived/de-activated</summary>
    public class ObjectsVirtualParent : MonoBehaviour
    {
        public GameObject realParent;
        public int startIndexIfNoLazyInit, count;
		public GameObject prefabIfLazyInit;
		public bool lazyInitSaveMemory;
        public GameObject[] virtualChildren;
		public bool delayInit = true;


        void OnEnable()
        {
			if (delayInit)
				StartCoroutine(InitCoroutine());
			else
				Init();
        }

		IEnumerator InitCoroutine()
		{
			yield return null;
			yield return null;
			Init();
		}

		void Init()
		{
			if (virtualChildren.Length == 0)
			{
				var list = new List<GameObject>();
				if (lazyInitSaveMemory)
				{
					for (int i = 0; i < count; ++i)
					{
						var instance = Instantiate(prefabIfLazyInit) as GameObject;
						instance.transform.SetParent(realParent.transform, false);
						list.Add(instance);
						instance.SetActive(true);
					}
				}
				else
				{
					for (int i = startIndexIfNoLazyInit; i < startIndexIfNoLazyInit + count; ++i)
					{
						var go = realParent.transform.GetChild(i).gameObject;
						list.Add(go);
						go.SetActive(true);
					}
				}
				virtualChildren = list.ToArray();
			}
			else
				foreach (var c in virtualChildren)
					c.SetActive(true);
		}

		void OnDisable()
        {
			StopAllCoroutines();
			if (lazyInitSaveMemory)
			{
				foreach (var c in virtualChildren)
				{
					c.SetActive(false);
					Destroy(c);
				}
				virtualChildren = new GameObject[0];
			}
			else
				foreach (var c in virtualChildren)
					c.SetActive(false);
		}
    }
}
