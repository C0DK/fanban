namespace Fanban.Domain

open System
open Fanban.Domain.ResultHelpers
open Fanban.Domain.Index
open Fanban.Domain.BoardError
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

type BoardCreatedPayload =
    { BoardId: BoardId
      Name: string
      ColumnNames: ColumnName list }

    static member Create (name: string) (columns: ColumnName list) =
        [ (String.IsNullOrWhiteSpace name) |> Result.requireFalse boardNameCannotBeEmpty
          columns.IsEmpty |> Result.requireFalse boardCannotHaveZeroColumns ]
        |> GivenValidThenReturn(
            DomainEvent.newWithPayload
                { BoardId = BoardId.New()
                  Name = name
                  ColumnNames = columns }
        )

and BoardNameSetPayload =
    { BoardId: BoardId
      Name: string }

    static member New id name =
        [ (String.IsNullOrWhiteSpace name) |> Result.requireFalse boardNameCannotBeEmpty ]
        |> GivenValidThenReturn(DomainEvent.newWithPayload (BoardNameSet { BoardId = id; Name = name }))

and ColumnAddedPayload =
    { BoardId: BoardId
      ColumnName: ColumnName
      Index: Index }

and ColumnRemovedPayload =
    { BoardId: BoardId
      ColumnName: ColumnName }

and CardAddedPayload = { BoardId: BoardId; Card: Card }

and CardMovedPayload =
    { BoardId: BoardId
      CardId: CardId
      NewColumn: ColumnName
      ColumnIndex: Index.Index }

and BoardEvent =
    | BoardNameSet of BoardNameSetPayload
    | ColumnAdded of ColumnAddedPayload
    | ColumnRemoved of ColumnRemovedPayload
    | CardAdded of CardAddedPayload
    | CardMoved of CardMovedPayload
    | BoardCreated of BoardCreatedPayload
