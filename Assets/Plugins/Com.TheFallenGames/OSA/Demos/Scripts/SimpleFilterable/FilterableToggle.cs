using Com.TheFallenGames.OSA.Demos.Simple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.TheFallenGames.OSA.Demos.SimpleFilterable
{ 
public class FilterableToggle : MonoBehaviour {
	
		[SerializeField]
	int indexToSearchFor;
	[SerializeField]
	SimpleFilterExample target;

	// Use this for initialization
	void Start () {

		}
	
	// Update is called once per frame
	void Update () {

			//Debug.Log(indexToSearchFor);
		}

	public void addToFilter(bool isOn)
    {
            if (isOn)
            {
				target.addFilterIndex(indexToSearchFor);
            }
            else
            {
				target.removeFilterIndex(indexToSearchFor);
            }
    }
}
}