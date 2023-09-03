using System;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models
{
    /// <summary>A model representing an expandable item, including an <see cref="expanded"/> view-state-related property to keep track of whether it's expanded or not. It also includes an <see cref="imageURL"/> to be loaded into the view</summary>
    public class ExpandableModel : BaseModel
    {
        #region Data Fields
        public string imageURL;
		#endregion

		#region View State
		public float ExpandedAmount { get; set; }
		#endregion
	}
}
