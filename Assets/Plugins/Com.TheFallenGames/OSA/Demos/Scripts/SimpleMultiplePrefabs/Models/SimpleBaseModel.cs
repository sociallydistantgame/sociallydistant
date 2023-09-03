using System;

namespace Com.TheFallenGames.OSA.Demos.SimpleMultiplePrefabs.Models
{
    /// <summary>
    /// Base class for the 3 models used in the SimpleMultiplePrefabsExample scene.
    /// </summary>
    public abstract class SimpleBaseModel
    {
		/// <summary>Assigned in the constructor. It's related to the visual state and not a data field per-se, but the gains in performance are huge if it's declared here, compared to being managed in a separate array or class</summary>
		public Type CachedType { get; private set; }


        public SimpleBaseModel()
        { CachedType = GetType(); }
    }
}
