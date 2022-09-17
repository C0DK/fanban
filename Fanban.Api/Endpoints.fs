module Fanban.Api.Endpoints

open System
open Fanban.Domain

open Giraffe

let boards (getBoards: unit -> Board seq) : HttpHandler =
    json (getBoards ())

let getBoard (getBoard: BoardId -> Board option) (id: Guid) : HttpHandler =
    match getBoard (BoardId.FromGuid id) with
    | Some board -> json board
    | None -> RequestErrors.notFound (text $"Board '{id}' not found")
