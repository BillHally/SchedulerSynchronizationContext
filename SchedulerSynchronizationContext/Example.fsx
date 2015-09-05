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

let asyncWork uiScheduler backgroundScheduler x : Async<unit> =
    async {
        do! Async.SwitchToScheduler backgroundScheduler
        let backgroundResult = backgroundWork x

        do! Async.SwitchToScheduler uiScheduler
        uiWork backgroundResult
    }

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
