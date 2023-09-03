using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	public class OSATitle : MonoBehaviour
	{
		protected void Start() { GetComponent<Text>().text = "Optimized ScrollView Adapter v" + OSAConst.OSA_VERSION_STRING; }
	}
}