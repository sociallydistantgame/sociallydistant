#nullable enable
using UnityEngine;
using UnityExtensions;

namespace GameplaySystems.Mail
{
	public sealed class MailManagerBootstrap : MonoBehaviour
	{
		[SerializeField]
		private MailManagerHolder mailHolder = null!;

		[SerializeField]
		private MailManager mailManagerPrefab = null!;

		private void Awake()
		{
			this.AssertAllFieldsAreSerialized(typeof(MailManagerBootstrap));
			this.mailHolder.Value = Instantiate(mailManagerPrefab);
		}

		private void OnDestroy()
		{
			if (mailHolder.Value == null)
				return;
			
			Destroy(mailHolder.Value.gameObject);
		}
	}
}