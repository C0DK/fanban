module Fanban.Application.Configuration


open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result
open Fanban.Application

let repository = InMemoryBoardRepository();

repository.AddBoard TestData.someBoard |> Result.valueOr failwithf

module CommandHandler =
    let ExecuteCreateBoard command = CommandHandler.CreateBoard repository.AddBoard command