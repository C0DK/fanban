namespace Fanban.Domain.Tests

open Fanban.Domain
open Board
open FsToolkit.ErrorHandling

module Fixture =
    let BoardName = "Fanban"
    let OtherBoardName = "Jira"
    let Card = Card.New "A minimal domain model" |> Result.valueOr failwith

    module Columns =
        let Todo = ColumnName.New "TODO"
        let Doing = ColumnName.New "Doing"
        let Done = ColumnName.New "Done"

    module ExtraColumns =
        let Backlog = ColumnName.New "Backlog"

    let NewBoardEvent =
        BoardCreatedPayload.Create BoardName [ Columns.Todo; Columns.Doing; Columns.Done ]
        |> Result.valueOr failwith

    let board = create NewBoardEvent
