
namespace Com.TheFallenGames.OSA.CustomAdapters.TableView
{
	/// <summary>
	/// <para>
	/// If you want to support clearing values, i.e. setting cells or entire columns to null, 
	/// or the other functionalities exposed by <see cref="ITableViewOptionsPanel"/>, implement this, attach it to a game object and assign it to TableAdapter's params.
	/// </para>
	/// The <see cref="IsClearing"/> and <see cref="IsLoading"/> are not mutually-exclusive
	/// </summary>
	public interface ITableViewOptionsPanel
	{
		bool IsClearing { get; set; }
		bool IsLoading { get; set; }
	}
}