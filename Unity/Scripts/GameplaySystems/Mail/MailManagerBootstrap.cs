#nullable enable
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Mail
{
	public sealed class MailManagerBootstrap : MonoBehaviour
	{
		[SerializeField]
		private MailManager mailManagerPrefab = null!;

		private MailManager mailManager;
		
		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MailManagerBootstrap));
			this.mailManager = Instantiate(mailManagerPrefab);
		}
	}
}