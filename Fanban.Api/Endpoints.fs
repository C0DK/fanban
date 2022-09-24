module Fanban.Api.Endpoints

open System
open Fanban.Application
open Fanban.Application.Commands
open Fanban.Domain

open Giraffe

let jsonError msg =
    json {| msg= msg |}

let NotFound msg =
    RequestErrors.notFound (jsonError msg)

let BadRequest msg =
    RequestErrors.badRequest (jsonError msg)

let Success response =
    Successful.ok (json response)

let Created response =
    Successful.created (json response)


let boards (getBoards: unit -> Board seq) : HttpHandler =
    json (getBoards ())

let getBoard (getBoard: BoardId -> Board option) (id: Guid) : HttpHandler =
    match getBoard (BoardId.FromGuid id) with
    | Some board -> Success board
    | None -> NotFound $"Board '{id}' not found"

let createBoard (addBoard: Board -> BoardId) (command: CreateBoard) : HttpHandler =
    match CommandHandler.CreateBoard addBoard command with
    | Ok boardId -> Created boardId
    | Error msg -> BadRequest msg