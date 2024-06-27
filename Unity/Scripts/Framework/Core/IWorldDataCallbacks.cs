#nullable enable
namespace Core
{
	public interface IWorldDataCallbacks
	{
		void AddCreateCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct;
		
		void AddModifyCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct;
		
		void AddDeleteCallback<TDataElement>(DeleteCallback<TDataElement> callback)
			where TDataElement : struct;
		
		void RemoveCreateCallback<TDataElement>(CreateCallback<TDataElement> callback)
			where TDataElement : struct;
		
		void RemoveModifyCallback<TDataElement>(ModifyCallback<TDataElement> callback)
			where TDataElement : struct;
		
		void RemoveDeleteCallback<TDataElement>(DeleteCallback<TDataElement> callback)
			where TDataElement : struct;
	}
	
	public delegate void CreateCallback<in TDataElement>(TDataElement subject)
		where TDataElement : struct;
	
	public delegate void ModifyCallback<in TDataElement>(TDataElement subjectPrevious, TDataElement subjectNew)
		where TDataElement : struct;
	
	public delegate void DeleteCallback<in TDataElement>(TDataElement subject)
		where TDataElement : struct;
}