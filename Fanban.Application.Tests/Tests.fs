module CommandHandler

open System
open Fanban.Application.Commands
open Xunit
open FsCheck
open FsCheck.Xunit

open Fanban.Application
open FsUnitTyped


module SaveEvent =
    [<Property>]
    let ``Given Valid command, Returns result from save function`` (command: CreateBoard, result : Result<Unit, string>) =
          command.ToEvent() |> Result.isOk ==> (CommandHandler.SaveEvent (fun _ -> result) command = result)

