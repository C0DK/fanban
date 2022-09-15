namespace Fanban.Application.Commands

open Fanban.Domain

type CreateBoard =
    { Name: string
      ColumnNames: ColumnName list }

    member this.ToEvent () =
        BoardCreated.create this.Name this.ColumnNames