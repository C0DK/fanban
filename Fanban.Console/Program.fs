open Fanban.Domain
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

let board =
    NewBoardEvent.Create "My board" [ ColumnName.New "TODO"; ColumnName.New "Doing"; ColumnName.New "Done" ]
    |> Result.valueOr failwith
    |> Board.createFrom

for column in board.Columns do
    printfn $"{column.Name}"
