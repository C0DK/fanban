namespace Fanban.Application.Commands

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result
open Fanban.Domain

type CreateBoardPayload =
    { Name: string
      ColumnNames: string list }

and AddCardPayload =
    { BoardId: BoardId; Card: Card }

and MoveCardPayload =
    { BoardId: BoardId
      CardId: CardId
      NewColumn: string
      ColumnIndex: Index }

and BoardCommand  =
    | CreateBoard of CreateBoardPayload
    | AddCard of AddCardPayload
    | MoveCard of MoveCardPayload
