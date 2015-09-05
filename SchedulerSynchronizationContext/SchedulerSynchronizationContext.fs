namespace SchedulerSynchronizationContext

open System
open System.Reactive
open System.Reactive.Concurrency
open System.Threading

type SchedulerSynchronizationContext(s : IScheduler) =
    inherit SynchronizationContext()

    override this.Send(d : SendOrPostCallback, state : obj) =
        use event = new AutoResetEvent(false)
        
        fun () ->
            d.Invoke state |> ignore
            event.Set()    |> ignore
        |> s.Schedule
        |> ignore

        event.WaitOne() |> ignore

    override this.Post(d : SendOrPostCallback, state : obj) =
        fun () ->
            d.Invoke state |> ignore
        |> s.Schedule
        |> ignore

[<AutoOpen>]
module AsyncExtensions =
    type Microsoft.FSharp.Control.Async with
        static member SwitchToScheduler (s : IScheduler) : Async<unit> = Async.SwitchToContext (SchedulerSynchronizationContext(s))
