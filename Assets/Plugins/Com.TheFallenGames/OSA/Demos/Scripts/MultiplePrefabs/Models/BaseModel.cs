using System;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models
{
    /// <summary>
    /// Base class for the 2 models used in the MultiplePrefabsExample scene. Contains a title as the only data field and some other fields for the view state
    /// </summary>
    public abstract class BaseModel
    {
		#region Data Fields
		public int id;
		///// <summary>Common data field for all derived models</summary>
		//public string title;
		#endregion

		#region View State
		/// <summary>Assigned in the constructor. It's related to the visual state and not a data field per-se, but the gains in performance are huge if it's declared here, compared to being managed in a separate array or class</summary>
		public Type CachedType { get; private set; }
        #endregion


        public BaseModel()
        { CachedType = GetType(); }
    }
}
