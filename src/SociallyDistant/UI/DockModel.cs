using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace SociallyDistant.UI;

public sealed class DockModel
{
    private readonly List<DockGroup>                 groups        = new();
    private readonly Subject<IEnumerable<DockGroup>> groupsSubject = new();

    public IObservable<IEnumerable<DockGroup>> DockGroupsObservable { get; }
    
    internal DockModel()
    {
        DockGroupsObservable = Observable.Create<IEnumerable<DockGroup>>(observer =>
        {
            observer.OnNext(groups);
            return groupsSubject.Subscribe(observer);
        });
    }
    
    public DockGroup DefineGroup()
    {
        var group = new DockGroup(this);
        groups.Add(group);
        return group;
    }

    public void RefreshDock()
    {
        groupsSubject?.OnNext(groups);
    }
}