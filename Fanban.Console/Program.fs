open Fanban.Domain
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

open Board

let card1 = Card.New "Finish domain model" |> Result.valueOr failwith

let TODO = ColumnName.New "TODO"
let Doing = ColumnName.New "Doing"
let Done = ColumnName.New "Done"

let board =
    Board.createFrom
    <!>
    NewBoardEvent.Create "My board" [ TODO ; Doing; Done ]
    >>= withCard card1
    >>= withCard (Card.New "Setup data access" |> Result.valueOr failwith)
    >>= withCard (Card.New "Setup api" |> Result.valueOr failwith)
    >>= moveCard card1  Doing Index.Beginning
    |> Result.valueOr failwith

let printCards cards =
    for card in cards do
        printfn $" - '{card.Name}' ({card.Id})"

let printColumn (column : Column) =
    if column.Cards |> Seq.isEmpty then
        printfn $"Column '{column.Name.Value}' is empty!"
    else
        printfn $"Column '{column.Name.Value}' contains: "
        printCards column.Cards

board.Columns |> Seq.iter printColumn

