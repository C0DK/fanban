// This script is created to basically test the most basic features of a kanban board
// The script itself showcases the status of the required features by creating cards and placing them in
// the correct column based on progress.
// So run it to see the status

open Fanban.Domain
open Fanban.Application
open Fanban.Application.Commands
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result
open Microsoft.FSharp.Core



let inMemoryStore = InMemoryEventStore()
let handle = CommandHandler.handle inMemoryStore.Push
let getEvents = inMemoryStore.GetEvents

let printEvents (events: BoardEvent seq) =
    printfn "Events:"

    for event in events do
        printfn $" - ({event.Id.ToShortString()}): {event.HumanReadableString}"

let printBoard (board: Board) =
    printfn $"Board: '{board.Name}'"

    for column in board.Columns.Value do
        printfn $" *{column.Name}*"

        for card in column.Cards do
            printfn $"  - {card.Id.ToShortString()}: {card.Name}"


let main =
    result {
        let! boardCreated =
            handle (
                CreateBoard
                    { Name = "The great Fanban project"
                      ColumnNames = [ "TODO"; "DOING"; "DONE" ] }
            )

        // TODO the actually card should probably be fetched from the event store, not returned here. (#CQRS)
        let addCard card =
            let payload =
                { BoardId = boardCreated.BoardId
                  Card = card }

            handle (AddCard payload) |> Result.map (fun _ -> card)

        let moveCardTo columnName (card: Card) =
            let payload =
                { BoardId = boardCreated.BoardId
                  CardId = card.Id
                  NewColumn = columnName
                  ColumnIndex = Index.Beginning }

            handle (MoveCard payload) |> Result.map (fun _ -> card)

        let! createCardsCard = Card.New "Create relevant cards" >>= addCard
        let! applyEventsCard = Card.New "apply events to create board" >>= addCard
        let! moveCardsCard = Card.New "move cards" >>= addCard
        let! _ = Card.New "save cards persistently" >>= addCard
        let! _ = Card.New "create snapshots" >>= addCard
        let! _ = Card.New "Correct CQRS" >>= addCard
        let! _ = Card.New "create API" >>= addCard

        let! _ = moveCardTo "DONE" createCardsCard
        let! _ = moveCardTo "DONE" applyEventsCard
        let! _ = moveCardTo "DOING" moveCardsCard


        printEvents (getEvents ())

        let boardEvents = inMemoryStore.GetEventsOf boardCreated.BoardId |> Seq.toList

        let! board = Board.ApplyEvents boardEvents

        printBoard board
    }

main |> Result.valueOr failwith
