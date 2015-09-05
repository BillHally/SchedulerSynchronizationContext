# SchedulerSynchronizationContext
A synchronization context which schedules work to a given scheduler, and an implementation of Async.SwitchToScheduler which uses it.

For example, if you have some work which you want performed on a background scheduler to avoid blocking the UI thread, and the value propagated on the UI thread, this can be done as follows:

```f#

  let backgroundWork () = 5
  let foregroundWork x = ()

  let asyncWork uiScheduler backgroundScheduler : Async<unit> =
    async {
      do! Async.SwitchToScheduler backgroundScheduler
      let backgroundResult = backgroundWork()

      do! Async.SwitchToScheduler uiScheduler
      foregroundWork backgroundResult
    }
```
