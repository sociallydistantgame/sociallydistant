namespace AcidicGui.Mvc
{
	/// <summary>
	///		Interface for an MVC view that can be supplied with a data model.
	/// </summary>
	/// <typeparam name="TData">The type of data this view is capable of displaying.</typeparam>
	public interface IViewWithData<in TData> : IView
	{
		void SetData(TData data);
	}
}