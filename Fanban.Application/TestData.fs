module Fanban.Application.TestData

open Fanban.Domain
open Fanban.Domain.Board
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

let TODO = Name.create "Todo" |> Result.valueOr failwith
let DOING = Name.create "Doing" |> Result.valueOr failwith
let BLOCKED = Name.create "Blocked" |> Result.valueOr failwith
let DONE = Name.create "Done" |> Result.valueOr failwith

let createDomainModel = Card.New "A minimal domain model" |> Result.valueOr failwith
let setupApi = Card.New "Setup epic api" |> Result.valueOr failwith
let setupDataAccess = Card.New "Setup DataAccess" |> Result.valueOr failwith
let celebrate = Card.New "Celebrate that it's perfect" |> Result.valueOr failwith
let createWebapp = Card.New "Create webapp" |> Result.valueOr failwith

let boardName = Name.create "Our epic board" |> Result.valueOr failwith

let states = NonEmptyList.create [ TODO; DOING; BLOCKED; DONE ] |> Result.valueOr failwith

let someBoard =
    BoardCreated.Create boardName states
    |> create
    |> withCard createDomainModel
    >>= moveCard createDomainModel DONE Beginning
    >>= withCard setupApi
    >>= moveCard setupApi DOING Beginning
    >>= withCard setupDataAccess
    >>= withCard celebrate
    >>= withCard createWebapp
    >>= moveCard celebrate BLOCKED Beginning
    |> Result.valueOr failwith

let someEmptyBoard =
    BoardCreated.Create
        (Name.create "Some other board" |> Result.valueOr failwith)
        (NonEmptyList.create [ TODO; DONE ] |> Result.valueOr failwith)
    |> create
