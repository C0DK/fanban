module Fanban.Api.Configuration
open System.Collections.Generic
open Fanban.Domain

let pairToOption ((parsed, value) : bool * 'a) =
    if parsed then Some value else None

type InMemoryBoardRepository() =
    let boards = Dictionary<BoardId, Board>()
    member this.GetBoards() =
        boards.Values |> seq

    member this.GetBoard (boardId : BoardId) =
        boards.TryGetValue boardId |> pairToOption

    member this.AddBoard (board : Board) =
        boards.Add(board.Id, board)


let repository = InMemoryBoardRepository();

// Add some test data

repository.AddBoard TestData.someBoard