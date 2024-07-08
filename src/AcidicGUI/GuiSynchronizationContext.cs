// https://github.com/vchelaru/FlatRedBall/blob/NetStandard/Engines/FlatRedBallXNA/FlatRedBall/Managers/SingleThreadSynchronizationContext.cs

using System.Collections.Concurrent;

namespace AcidicGUI;

public class GuiSynchronizationContext : SynchronizationContext
{
    private readonly ConcurrentQueue<Action> _messagesToProcess = new ConcurrentQueue<Action>();
    private readonly Queue<Action>           thisFrameQueue     = new Queue<Action>();
    
    public GuiSynchronizationContext()
    {
        SynchronizationContext.SetSynchronizationContext(this);
    }

    public override void Send(SendOrPostCallback codeToRun, object state)
    {
        throw new NotImplementedException();
    }

    public override void Post(SendOrPostCallback codeToRun, object state)
    {
        _messagesToProcess.Enqueue(() =>
        {
            try
            {
                codeToRun(state);
            }
            catch (TaskCanceledException)
            {
            }
        });
    }

    public void Update()
    {
        // Normally with async methods, it's all time-based. For example,
        // you do an await Task.Delay(100); and you expect that it waits 100 
        // milliseconds, then continues. FRB is a little different - it needs
        // to do time based, but the time we're interested in specifically is game
        // time. We use TimeManager.DelaySeconds to make sure that everything respects
        // game time and time factor (slow-mo, speed-up, and pausing). Therefore, we need
        // to check tasks every time Update is called because there could be any amount of 
        // time passed between frames. Frames could speed up considerably if the user drags
        // the window or performs some other kind of logic that freezes the window temporarily 
        // and then MonoGame calls Update a bunch of times to play "catch-up". Therefore, the
        // TimeManager.DelaySeconds will not internally wait anything, it calls Task.Yield which
        // immediately puts the task back on the message to process. Before running any code we want
        // to empty the _messagesToProcess into thisFrameQueue, then run all tasks on this frame. Next
        // frame everything will repeat.

        while (_messagesToProcess.TryDequeue(out Action? next))
        {
            thisFrameQueue.Enqueue(next);
        }

        while (thisFrameQueue.Count > 0)
        {
            var actionToRun = thisFrameQueue.Dequeue();
            try
            {
                actionToRun();
            }
            catch (TaskCanceledException)
            {
                // Most likely the task was cancelled due to moving screens.
                // Nothing is to be done for cancelled tasks.
            }
        }
    }

    public void Clear()
    {
        _messagesToProcess.Clear();
    }
}