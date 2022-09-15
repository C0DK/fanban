namespace Fanban.Domain.Tests

open Fanban.Domain
open Board
open FsToolkit.ErrorHandling

module Fixture =
    let BoardName = Name.create "Fanban" |> Result.okOrFail
    let OtherBoardName = Name.create "Jira" |> Result.okOrFail
    let Card = Card.New "A minimal domain model" |> Result.okOrFail

    module Columns =
        let Todo = Name.create "TODO"  |> Result.okOrFail
        let Doing = Name.create "Doing" |> Result.okOrFail
        let Done = Name.create "Done" |> Result.okOrFail

    module ExtraColumns =
        let Backlog = Name.create "Backlog" |> Result.okOrFail

    let NewBoardEvent =
        BoardCreated.Create BoardName (NonEmptyList.create [ Columns.Todo; Columns.Doing; Columns.Done ] |> Result.okOrFail)

    let board = create NewBoardEvent
