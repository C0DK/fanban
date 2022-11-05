namespace Fanban.Application

open System.Collections.Generic
open Fanban.Domain
open Fanban.Utils

type InMemoryBoardRepository() =
    let boards = Dictionary<BoardId, Board>()
    member this.GetBoards() =
        boards.Values |> seq

    member this.GetBoard (boardId : BoardId) =
        boards.TryGetValue boardId |> Option.FromTuple

    member this.AddBoard (board : Board) =
        boards.Add(board.Id, board)
        Ok ()


type InMemoryEventStore() =
    let events = List<BoardEvent>()
    member this.GetEvents() =
        events |> seq

    member this.GetEventsOf (boardId : BoardId) =
        this.GetEvents() |> Seq.filter (fun event -> event.BoardId = boardId)


    member this.Push (event: BoardEvent) : Result<BoardEvent, string>=
        events.Add event
        Ok event

