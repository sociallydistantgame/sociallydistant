#nullable enable
using UnityEngine;

namespace Architecture
{
	public sealed class Comment : MonoBehaviour
	{
		[SerializeField]
		[TextArea]
		private string comment = string.Empty;
	}
}