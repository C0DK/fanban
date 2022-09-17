module Fanban.Api.Endpoints

open System
open Fanban.Domain
open Fanban.Domain.Board
open Fanban.Domain.Index
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open Giraffe

let TODO = ColumnName.New "Todo"
let DOING = ColumnName.New "Doing"
let BLOCKED = ColumnName.New "Blocked"
let DONE = ColumnName.New "Done"

let createDomainModel = Card.New "A minimal domain model" |> Result.valueOr failwith
let setupApi = Card.New "Setup epic api" |> Result.valueOr failwith
let setupDataAccess = Card.New "Setup DataAccess" |> Result.valueOr failwith
let celebrate = Card.New "Celebrate that it's perfect" |> Result.valueOr failwith
let createWebapp = Card.New "Create webapp" |> Result.valueOr failwith

let NewBoardEvent =
    NewBoardEvent.Create "Our epic board" [ TODO; DOING; BLOCKED; DONE ]
    |> Result.valueOr failwith

let someBoard =
    create NewBoardEvent
    |> withCard createDomainModel
    >>= moveCard createDomainModel DONE Beginning
    >>= withCard setupApi
    >>= moveCard setupApi DOING Beginning
    >>= withCard setupDataAccess
    >>= withCard celebrate
    >>= withCard createWebapp
    >>= moveCard celebrate BLOCKED Beginning
    |> Result.valueOr failwith

let boards : HttpHandler =
    json [ someBoard ]

let getBoard (id: Guid) : HttpHandler =
    json { someBoard with Id = BoardId.FromGuid(id) }
