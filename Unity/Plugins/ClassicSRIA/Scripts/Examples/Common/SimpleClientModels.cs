

namespace frame8.ScrollRectItemsAdapter.Classic.Examples.Common
{
	public class SimpleClientModel
	{
		public string clientName;
		public string location;
		public float availability01, contractChance01, longTermClient01;
		public bool isOnline;

		public float AverageScore01 { get { return (availability01 + contractChance01 + longTermClient01) / 3; } }

		public void SetRandom()
		{
			availability01 = CUtil.RandF();
			contractChance01 = CUtil.RandF();
			longTermClient01 = CUtil.RandF();
			isOnline = CUtil.Rand(2) == 0;
		}
	}


	public class ExpandableSimpleClientModel : SimpleClientModel
	{
		// View size related
		public bool expanded;
		public float nonExpandedSize;
	}
}
