module Fanban.Api.TestData

open Fanban.Domain
open Fanban.Domain.Board
open Fanban.Domain.Index
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

let TODO = ColumnName.New "Todo"
let DOING = ColumnName.New "Doing"
let BLOCKED = ColumnName.New "Blocked"
let DONE = ColumnName.New "Done"

let createDomainModel = Card.New "A minimal domain model" |> Result.valueOr failwith
let setupApi = Card.New "Setup epic api" |> Result.valueOr failwith
let setupDataAccess = Card.New "Setup DataAccess" |> Result.valueOr failwith
let celebrate = Card.New "Celebrate that it's perfect" |> Result.valueOr failwith
let createWebapp = Card.New "Create webapp" |> Result.valueOr failwith

let someBoard =
    NewBoardEvent.Create "Our epic board" [ TODO; DOING; BLOCKED; DONE ]
    |> Result.map create
    >>= withCard createDomainModel
    >>= moveCard createDomainModel DONE Beginning
    >>= withCard setupApi
    >>= moveCard setupApi DOING Beginning
    >>= withCard setupDataAccess
    >>= withCard celebrate
    >>= withCard createWebapp
    >>= moveCard celebrate BLOCKED Beginning
    |> Result.valueOr failwith

let someEmptyBoard =
    NewBoardEvent.Create "Some other board" [ TODO; DONE ]
    |> Result.map create
    >>= withCard createDomainModel
    >>= moveCard createDomainModel DONE Beginning
    >>= withCard setupApi
    >>= moveCard setupApi DOING Beginning
    >>= withCard setupDataAccess
    >>= withCard celebrate
    >>= withCard createWebapp
    >>= moveCard celebrate BLOCKED Beginning
    |> Result.valueOr failwith
