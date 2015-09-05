module SchedulerSynchronizationContext.Tests

open FsUnit
open NUnit.Framework
open SchedulerSynchronizationContext
open Microsoft.Reactive.Testing

[<Test>]
let ``Async.SwitchToScheduler when invoked runs the continuation using the scheduler``() =
    // Arrange
    let s = TestScheduler()

    let mutable state = 0

    async
        {
            state <- 1
            do! Async.SwitchToScheduler s
            state <- 2
        }
        |> Async.StartImmediate

    // Act
    let before = state
    s.Start()
    let after = state

    // Assert
    before |> should equal 1
    after  |> should equal 2

[<Test>]
let ``meh``() =
    let backgroundWork () = 5

    let state = ref false
    let uiWork x = state := true

    let asyncWork uiScheduler backgroundScheduler : Async<unit> =
        async {
            do! Async.SwitchToScheduler backgroundScheduler
            let backgroundResult = backgroundWork()

            do! Async.SwitchToScheduler uiScheduler
            uiWork backgroundResult
        }

    let uiScheduler = TestScheduler()
    let backgroundScheduler = TestScheduler()

    asyncWork uiScheduler backgroundScheduler |> Async.StartImmediate

    backgroundScheduler.AdvanceBy 1L
    printfn "%A" state.Value
    uiScheduler.AdvanceBy 1L
    printfn "%A" state.Value
