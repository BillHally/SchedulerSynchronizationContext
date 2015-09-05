# SchedulerSynchronizationContext
A synchronization context which schedules work to a given scheduler, and an implementation of ```Async.SwitchToScheduler``` which uses it.

For example, if you have some work which you want performed on a background scheduler to avoid blocking the UI thread, and the value propagated to the UI thread, this can be done as follows:

```f#
open SchedulerSynchronizationContext

let asyncWork uiScheduler backgroundScheduler x : Async<unit> =
    async {
        do! Async.SwitchToScheduler backgroundScheduler
        let backgroundResult = backgroundWork x

        do! Async.SwitchToScheduler uiScheduler
        uiWork backgroundResult
    }
```

This can then be used as follows (though obviously production code would not use ```TestScheduler```, and would perform real work): 

```f#

#r @"..\packages\Rx-Interfaces.2.2.5\lib\net45\System.Reactive.Interfaces.dll"
#r @"..\packages\Rx-Core.2.2.5\lib\net45\System.Reactive.Core.dll"
#r @"..\packages\Rx-Linq.2.2.5\lib\net45\System.Reactive.Linq.dll"
#r @"bin\Debug\SchedulerSynchronizationContext.dll"
#r @"..\packages\Rx-Testing.2.2.5\lib\net45\Microsoft.Reactive.Testing.dll"

open SchedulerSynchronizationContext
open Microsoft.Reactive.Testing

let backgroundWork x =
    printfn "backgroundWork passed %d" x
    x * 5

let uiWork x =
    printfn "uiWork passed %d" x

let uiScheduler = TestScheduler()
let backgroundScheduler = TestScheduler()

asyncWork uiScheduler backgroundScheduler 7 |> Async.StartImmediate

printfn "Advancing background scheduler..."
backgroundScheduler.AdvanceBy 1L
printfn "Advancing UI scheduler..."
uiScheduler.AdvanceBy 1L

(*
Prints:

Advancing background scheduler...
backgroundWork passed 7
Advancing UI scheduler...
uiWork passed 35

*)
```
