namespace Fanban.Domain

open Fanban.Domain.ResultHelpers
open Fanban.Domain.Index
open Fanban.Domain.BoardError
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

type Board =
    { Id: BoardId
      Name: string
      Columns: Column list
      History: BoardEvent DomainEvent list }

    member this.cards = this.Columns |> Seq.collect (fun column -> column.Cards)

    member this.tryGetCard(cardId: CardId) =
        this.cards |> Seq.tryFind (fun card -> card.Id = cardId)

    member this.getCard(cardId: CardId) =
        this.tryGetCard cardId |> Result.requireSome (cardDoesntExist cardId)

    member this.tryGetColumn(columnName: ColumnName) =
        this.Columns |> Seq.tryFind (fun column -> column.Name = columnName)

    member this.getColumn(columnName: ColumnName) =
        this.tryGetColumn columnName
        |> Result.requireSome (columnDoesntExist columnName)

    member this.applyEvent event =
        match event with
        | AddCard event ->
            [ (this.cards |> Seq.exists (fun card -> card.Id = event.Card.Id))
              |> Result.requireFalse (cardAlreadyExistExist event.Card.Id) ]
            |> GivenValidThenReturn
                { this with
                    Columns =
                        match this.Columns with
                        | head :: tail -> { head with Cards = insertAt Beginning event.Card head.Cards } :: tail
                        | _ -> failwith "List of columns was empty - Invariance failed" }

        | MoveCard event ->
            this.RequireContainsColumn event.NewColumn
            >>= fun _ -> (this.getCard event.CardId)
            |> Result.map (fun card ->
                { this with
                    Columns =
                        this.Columns
                        |> Seq.map (fun column ->
                            match column with
                            | column when column.Name = event.NewColumn ->
                                { column with Cards = column.Cards |> insertAt event.ColumnIndex card }
                            | column ->
                                { column with Cards = column.Cards |> Seq.except (List.singleton card) |> Seq.toList })
                        |> Seq.toList })
        | AddColumn event ->
            [ (this.tryGetColumn event.ColumnName)
              |> Result.requireNone (columnAlreadyExist event.ColumnName) ]
            |> GivenValidThenReturn
                { this with Columns = this.Columns |> insertAt event.Index (Column.WithName event.ColumnName) }
        | RemoveColumn event ->
            [ this.RequireContainsColumn event.ColumnName
              this.RequireColumnIsEmpty event.ColumnName
              this.Columns.Length > 1 |> Result.requireTrue boardCannotHaveZeroColumns ]
            |> GivenValidThenReturn
                { this with
                    Columns =
                        this.Columns
                        |> Seq.filter (fun column -> column.Name <> event.ColumnName)
                        |> Seq.toList }
        | SetBoardName setBoardName -> Ok { this with Name = setBoardName.Name }
        | BoardCreated _ -> Error cannotCreateExistingBoard
        // Todo wrap this everywhere else
        |> Result.map (fun board -> { board with History = DomainEvent.newWithPayload event :: board.History })

    member private board.RequireContainsColumn(name: ColumnName) =
        board.getColumn name |> Result.map (fun _ -> ())

    member private board.RequireColumnIsEmpty(columnName: ColumnName) =
        (board.getColumn columnName)
        |> Result.map (fun column -> column.Cards)
        |> Result.bind (Result.requireEmpty (cannotRemoveNonEmptyColumn columnName))


module Board =
    let withName name (board: Board) =
        SetBoardNameEvent.New board.Id name >>= board.applyEvent

    let withColumnAt index columnName (board: Board) =
        board.applyEvent (
            AddColumn
                { BoardId = board.Id
                  ColumnName = columnName
                  Index = index }
        )

    let withCard card (board: Board) =
        board.applyEvent (AddCard { BoardId = board.Id; Card = card })

    let moveCard (card: Card) columnName (index: Index) (board: Board) =
        board.applyEvent (
            MoveCard
                { BoardId = board.Id
                  CardId = card.Id
                  NewColumn = columnName
                  ColumnIndex = index }
        )

    let withoutColumn columnName (board: Board) =
        board.applyEvent (
            RemoveColumn
                { BoardId = board.Id
                  ColumnName = columnName }
        )

    let create (event: DomainEvent<BoardCreated>) =
        { Id = event.payload.Id
          Name = event.payload.Name
          Columns = event.payload.ColumnNames |> Seq.map Column.WithName |> Seq.toList
          History = List.singleton (event) }
