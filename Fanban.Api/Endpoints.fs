module Fanban.Api.Endpoints

open Fanban.Domain
open Fanban.Domain.Board
open Fanban.Domain.Index
open Board
open FsToolkit.ErrorHandling

open Giraffe

let TODO =ColumnName.New "TODO"
let DOING =ColumnName.New "DOING"
let card =(Card.New "A minimal domain model" |> Result.valueOr failwith)

let NewBoardEvent =
    NewBoardEvent.Create "Our epic board" [ TODO; DOING; ColumnName.New "Done" ]
    |> Result.valueOr failwith

let someBoard = 
    create NewBoardEvent
    |> withCard card
    |> Result.valueOr failwith
    |> moveCard card DOING Beginning
    |> Result.valueOr failwith

let boards : HttpHandler =
    //text "These are the boards"
    json ([ someBoard ]) //"These are the boards"


let board : HttpHandler =
    //text "These are the boards"
    json someBoard //"These are the boards"

