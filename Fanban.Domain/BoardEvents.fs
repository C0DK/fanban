namespace Fanban.Domain

open System
open Fanban.Domain.ResultHelpers
open Fanban.Domain.Index
open Fanban.Domain.BoardError
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

type NewBoardEvent =
    { Id: BoardId
      Name: string
      ColumnNames: ColumnName list }

    static member Create name (columns: ColumnName list) =
        [ (String.IsNullOrWhiteSpace name) |> Result.requireFalse boardNameCannotBeEmpty
          columns.IsEmpty |> Result.requireFalse boardCannotHaveZeroColumns ]
        |> GivenValidThenReturn
            { Id = BoardId.New()
              Name = name
              ColumnNames = columns }

and SetBoardNameEvent =
    { BoardId: BoardId
      Name: string }

    static member New id name =
        [ (String.IsNullOrWhiteSpace name) |> Result.requireFalse boardNameCannotBeEmpty ]
        |> GivenValidThenReturn(SetBoardName { BoardId = id; Name = name })

and AddColumnEvent =
    { BoardId: BoardId
      ColumnName: ColumnName
      Index: Index }

and RemoveColumnEvent =
    { BoardId: BoardId
      ColumnName: ColumnName }

and AddCardEvent = { BoardId: BoardId; Card: Card }
and RemoveCardEvent = { BoardId: BoardId; CardId: CardId }

and MoveCardEvent =
    { BoardId: BoardId
      CardId: CardId
      NewColumn: ColumnName
      ColumnIndex: Index.Index }

and BoardEvent =
    | SetBoardName of SetBoardNameEvent
    | AddColumn of AddColumnEvent
    | RemoveColumn of RemoveColumnEvent
    | AddCard of AddCardEvent
    | RemoveCard of RemoveCardEvent
    | MoveCard of MoveCardEvent
    | NewBoard of NewBoardEvent
