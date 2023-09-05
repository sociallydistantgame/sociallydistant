namespace AcidicGui.Mvc
{
	public abstract class ViewWithData<TData> :
		View,
		IViewWithData<TData>
	{
		/// <inheritdoc />
		public abstract void SetData(TData data);
	}
}