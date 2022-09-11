module Fanban.Domain.Tests.Board

open FsCheck
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result
open Fanban.Domain
open Fanban.Domain.Index
open Board
open FsUnitTyped
open Xunit

module NewBoardWithName =
    [<Fact>]
    let ``New board has correct name`` () =
        Board.createFrom Fixture.NewBoardEvent
        |> fun board -> board.Name
        |> shouldEqual Fixture.BoardName

    [<Fact>]
    let ``New board has history of one`` () =
        Board.createFrom Fixture.NewBoardEvent
        |> fun board -> board.History
        |> shouldEqual [ NewBoard Fixture.NewBoardEvent ]

    [<Fact>]
    let ``New board always is same`` () =
        Board.createFrom Fixture.NewBoardEvent
        |> shouldEqual (Board.createFrom Fixture.NewBoardEvent)

    [<Fact>]
    let ``New board event requires non empty string`` () =
        NewBoardEvent.Create "" [ Fixture.Columns.Todo ]
        |> shouldEqual (Error BoardError.boardNameCannotBeEmpty)


    [<Fact>]
    let ``New board event requires non empty columns`` () =
        NewBoardEvent.Create "My Great project" []
        |> shouldEqual (Error BoardError.boardCannotHaveZeroColumns)


module Apply =
    [<Fact>]
    let ``FSCheck test`` () =
        let revIsOrig (xs:list<int>) = List.rev xs = xs
        Check.Quick revIsOrig

    [<Fact>]
    let ``All events add to history`` () =
        let revIsOrig (xs:list<int>) = List.rev xs = xs
        Check.Quick revIsOrig

    module SetBoardName =
        [<Fact>]
        let ``Can set board name`` () =
            Fixture.board
            |> withName Fixture.OtherBoardName
            |> Result.map (fun board -> board.Name)
            |> shouldEqual (Ok Fixture.OtherBoardName)

        [<Fact>]
        let ``to empty string, fails`` () =
            Fixture.board
            |> withName ""
            |> shouldEqual (Error BoardError.boardNameCannotBeEmpty)

        [<Fact>]
        let ``Add event to history`` () =
            Fixture.board
            |> withName Fixture.OtherBoardName
            |> Result.map (fun board -> board.History)
            |> shouldEqual (
                Ok(
                    [ SetBoardName
                          { BoardId = Fixture.board.Id
                            Name = Fixture.OtherBoardName }
                      NewBoard Fixture.NewBoardEvent ]
                )
            )

    module AddColumn =
        [<Fact>]
        let ``Add column`` () =
            Fixture.board
            |> withColumnAt Beginning Fixture.ExtraColumns.Backlog
            |> Result.map (fun board -> board.Columns[0])
            |> shouldEqual (Ok(Column.WithName Fixture.ExtraColumns.Backlog))

        [<Fact>]
        let ``with existing column, fails`` () =
            Fixture.board
            |> withColumnAt Beginning Fixture.Columns.Todo
            |> shouldEqual (Error(BoardError.columnAlreadyExist Fixture.Columns.Todo))

    module RemoveColumn =
        [<Fact>]
        let ``With existing column, succeeds`` () =
            Fixture.board
            |> withoutColumn Fixture.Columns.Done
            |> Result.map (fun board -> board.Columns.Length)
            |> shouldEqual (Ok 2)

        [<Fact>]
        let ``with assigned card, fails`` () =
            Fixture.board
            |> withCard Fixture.Card
            >>= withoutColumn Fixture.Columns.Todo
            |> shouldEqual (Error(BoardError.cannotRemoveNonEmptyColumn Fixture.Columns.Todo))


        [<Fact>]
        let ``with non existing column, fails`` () =
            Fixture.board
            |> withoutColumn Fixture.ExtraColumns.Backlog
            |> shouldEqual (Error(BoardError.columnDoesntExist Fixture.ExtraColumns.Backlog))

    module AddCard =
        [<Fact>]
        let ``with valid column, succeeds`` () =
            Fixture.board
            |> withCard Fixture.Card
            |> Result.map (fun board -> board.Columns)
            |> shouldEqual (
                Ok
                    [ { Name = Fixture.Columns.Todo
                        Cards = [ Fixture.Card ] }
                      { Name = Fixture.Columns.Doing
                        Cards = [] }
                      { Name = Fixture.Columns.Done
                        Cards = [] } ]
            )

        [<Fact>]
        let ``with duplicate card id, fails`` () =
            Fixture.board
            |> withCard Fixture.Card
            >>= withCard Fixture.Card
            |> shouldEqual (Error(BoardError.cardAlreadyExistExist Fixture.Card.Id))

    module DeleteCard =
        [<Fact>]
        let ``with valid card id, removes Card`` () =
            Fixture.board
            |> withCard Fixture.Card
            >>= withoutCard Fixture.Card
            |> Result.map (fun board -> board.cards)
            |> Result.valueOr failwith
            |> shouldNotContain Fixture.Card

        [<Fact>]
        let ``with invalid card id, fails`` () =
            Fixture.board
            |> withoutCard Fixture.Card
            |> shouldEqual (Error(BoardError.cardDoesntExist Fixture.Card.Id))

    module MoveCard =
        [<Fact>]
        let ``With valid column, removes from existing column`` () =
            Fixture.board
            |> withCard Fixture.Card
            >>= moveCard Fixture.Card Fixture.Columns.Done Beginning
            >>= (fun board -> board.getColumn Fixture.Columns.Todo)
            |> shouldEqual (
                Ok
                    { Name = Fixture.Columns.Todo
                      Cards = [] }
            )

        [<Fact>]
        let ``With valid column, adds to new column`` () =
            Fixture.board
            |> withCard Fixture.Card
            >>= moveCard Fixture.Card Fixture.Columns.Done Beginning
            >>= (fun board -> board.getColumn Fixture.Columns.Done)
            |> shouldEqual (
                Ok
                    { Name = Fixture.Columns.Done
                      Cards = [ Fixture.Card ] }
            )

        [<Fact>]
        let ``With invalid column, fails`` () =
            Fixture.board
            |> withoutColumn Fixture.Columns.Done
            >>= withCard Fixture.Card
            >>= moveCard Fixture.Card Fixture.Columns.Done Beginning
            |> shouldEqual (Error(BoardError.columnDoesntExist Fixture.Columns.Done))

        [<Fact>]
        let ``With none existing card, fails`` () =
            Fixture.board
            |> moveCard Fixture.Card Fixture.Columns.Todo Beginning
            |> shouldEqual (Error(BoardError.cardDoesntExist Fixture.Card.Id))

    module CreateBoard =
        [<Fact>]
        let ``fails on existing board`` () =
            Fixture.board.applyEvent (NewBoard Fixture.NewBoardEvent)
            |> shouldEqual (Error(BoardError.cannotCreateExistingBoard))
