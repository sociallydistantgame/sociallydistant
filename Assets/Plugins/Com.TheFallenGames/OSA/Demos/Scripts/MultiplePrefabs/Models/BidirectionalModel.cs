

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models
{
    /// <summary>Plain model having only a <see cref="value"/> float data field. It's used to demonstrate that not only the views can be updated from the model, but also it can go the other way around.</summary>
    public class BidirectionalModel : BaseModel
    {
        #region Data Fields
        public float value;
        #endregion
    }
}
