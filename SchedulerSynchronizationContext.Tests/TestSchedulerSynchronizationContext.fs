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
