module Fanban.Application.CommandHandler

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open Fanban.Application.Commands
open Fanban.Domain

let CreateBoard (saveBoard: Board -> BoardId) (command : CreateBoard) =
    command.ToEvent()
    |> Result.map Board.create
    |> Result.map saveBoard
