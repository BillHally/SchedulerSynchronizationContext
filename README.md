# SchedulerSynchronizationContext
A synchronization context which schedules work to a given scheduler, and an implementation of ```Async.SwitchToScheduler``` which uses it.

For example, if you have some work which you want performed on a background scheduler to avoid blocking the UI thread, and the value propagated to the UI thread, this can be done as follows:

```f#
open SchedulerSynchronizationContext

let asyncWork uiScheduler backgroundScheduler =
    async {
        do! Async.SwitchToScheduler backgroundScheduler
        let backgroundResult = backgroundWork ()

        do! Async.SwitchToScheduler uiScheduler
        return uiWork backgroundResult
    }

```

This can then be used as follows (I've used ```EventLoopScheduler```s which name their threads to make it clarify which thread work is performed on):

```f#
#r @"..\packages\Rx-Interfaces.2.2.5\lib\net45\System.Reactive.Interfaces.dll"
#r @"..\packages\Rx-Core.2.2.5\lib\net45\System.Reactive.Core.dll"
#r @"..\packages\Rx-Linq.2.2.5\lib\net45\System.Reactive.Linq.dll"
#r @"..\packages\Rx-PlatformServices.2.2.5\lib\net45\System.Reactive.PlatformServices.dll"
#r @"bin\Debug\SchedulerSynchronizationContext.dll"

open System.Threading
open System.Reactive.Concurrency

open SchedulerSynchronizationContext

let backgroundWork () =
    sprintf "backgroundWork ran on %s" Thread.CurrentThread.Name

let uiWork x =
    sprintf "%s\nuiWork ran on %s" x Thread.CurrentThread.Name

let asyncWork uiScheduler backgroundScheduler =
    async {
        do! Async.SwitchToScheduler backgroundScheduler
        let backgroundResult = backgroundWork ()

        do! Async.SwitchToScheduler uiScheduler
        return uiWork backgroundResult
    }

let uiScheduler         = new EventLoopScheduler(fun x -> new Thread(x, Name = "UI scheduler"))
let backgroundScheduler = new EventLoopScheduler(fun x -> new Thread(x, Name = "Background scheduler"))

let t = asyncWork uiScheduler backgroundScheduler |> Async.StartAsTask
printfn "Result:\n\n%s" t.Result

(*
Prints:

Result:

backgroundWork ran on Background scheduler
uiWork ran on UI scheduler

*)
```
