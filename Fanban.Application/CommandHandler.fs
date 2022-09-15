module Fanban.Application.CommandHandler

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open Fanban.Application.Commands
open Fanban.Domain

let SaveEvent (saveEvent: BoardEvent -> Result<Unit,string>) (command : CreateBoard) =
    command.ToEvent()
    |> Result.map BoardEvent.BoardCreated
    |> Result.bind saveEvent
