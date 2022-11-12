module Fanban.Application.CommandHandler

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open Fanban.Application.Commands
open Fanban.Domain

let handle (saveEvent: BoardEvent -> Result<BoardEvent, string>) (command: BoardCommand) =
    match command with
    | CreateBoard payload ->
        result {
            let! name = Name.create payload.Name

            let! columns =
                payload.ColumnNames
                |> List.map Name.create
                |> List.sequenceResultM
                >>= NonEmptyList.create

            return BoardCreated(BoardCreated.Create name columns)
        }
        >>= saveEvent
    | AddCard payload ->
        saveEvent (
            CardAdded(
                DomainEvent.newWithPayload
                    { BoardId = payload.BoardId
                      Card = payload.Card }
            )
        )

    | MoveCard payload ->
        result {
            let! column = Name.create payload.NewColumn

            let eventPayload =
                { BoardId = payload.BoardId
                  CardId = payload.CardId
                  ColumnIndex = payload.ColumnIndex
                  NewColumn = column }

            return CardMoved(DomainEvent.newWithPayload eventPayload)
        }
        >>= saveEvent
