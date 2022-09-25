namespace Fanban.Domain

open Fanban.Domain.ResultHelpers
open Fanban.Domain.BoardError
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

type Board =
    { Id: BoardId
      Name: Name
      Columns: Column NonEmptyList
      History: BoardEvent list }

    member this.cards =NonEmptyList.value this.Columns |> Seq.collect (fun column -> column.Cards)

    member this.tryGetCard(cardId: CardId) =
        this.cards |> Seq.tryFind (fun card -> card.Id = cardId)

    member this.getCard(cardId: CardId) =
        this.tryGetCard cardId |> Result.requireSome (cardDoesntExist cardId)

    member this.tryGetColumn(columnName: Name) =
        this.Columns |> NonEmptyList.value |> Seq.tryFind (fun column -> column.Name = columnName)

    member this.getColumn(columnName: Name) =
        this.tryGetColumn columnName
        |> Result.requireSome (columnDoesntExist columnName)

    member this.applyEvent(event: BoardEvent) =
        match event with
        | CardAdded event ->
            [ (this.cards |> Seq.exists (fun card -> card.Id = event.Payload.Card.Id))
              |> Result.requireFalse (cardAlreadyExistExist event.Payload.Card.Id) ]
            |> GivenValidThenReturn
                { this with Columns = this.Columns |> (NonEmptyList.mapFirstValue (Column.WithCard event.Payload.Card)) }

        | CardMoved event ->
            this.RequireContainsColumn event.Payload.NewColumn
            >>= fun _ -> (this.getCard event.Payload.CardId)
            |> Result.map (fun card ->
                { this with
                    Columns =
                        this.Columns
                        |> NonEmptyList.map (fun column ->
                            match column with
                            | column when column.Name = event.Payload.NewColumn ->
                                { column with Cards = column.Cards |> List.insertAt event.Payload.ColumnIndex card }
                            | column ->
                                { column with Cards = column.Cards |> Seq.except (List.singleton card) |> Seq.toList })
                         })
        | ColumnAdded event ->
            [ (this.tryGetColumn event.Payload.ColumnName)
              |> Result.requireNone (columnAlreadyExist event.Payload.ColumnName) ]
            |> GivenValidThenReturn
                { this with
                    Columns =
                        this.Columns
                        |> NonEmptyList.insertAt event.Payload.Index (Column.WithName event.Payload.ColumnName) }
        | ColumnRemoved event ->
            [ this.RequireContainsColumn event.Payload.ColumnName
              this.RequireColumnIsEmpty event.Payload.ColumnName
              (this.Columns |> NonEmptyList.length) > 1 |> Result.requireTrue boardCannotHaveZeroColumns ]
            |> GivenValidThenReturn
                { this with
                    Columns =
                        this.Columns
                        |> NonEmptyList.filter (fun column -> column.Name <> event.Payload.ColumnName)
                         }
        | BoardNameSet setBoardName -> Ok { this with Name = setBoardName.Payload.Name }
        | BoardCreated _ -> Error cannotCreateExistingBoard
        |> Result.map (fun board -> { board with History = event :: board.History })

    member private board.RequireContainsColumn(name: Name) =
        board.getColumn name |> Result.map (fun _ -> ())

    member private board.RequireColumnIsEmpty(columnName: Name) =
        (board.getColumn columnName)
        |> Result.map (fun column -> column.Cards)
        |> Result.bind (Result.requireEmpty (cannotRemoveNonEmptyColumn columnName))


module Board =
    let withName name (board: Board) =
        DomainEvent.newWithPayload { BoardId = board.Id; Name= name }
        |> BoardNameSet
        |> board.applyEvent

    let withColumnAt index columnName (board: Board) =
        board.applyEvent (
            ColumnAdded(
                DomainEvent.newWithPayload
                    { BoardId = board.Id
                      ColumnName = columnName
                      Index = index }
            )
        )

    let withCard card (board: Board) =
        board.applyEvent (CardAdded(DomainEvent.newWithPayload ({ BoardId = board.Id; Card = card })))

    let moveCard (card: Card) columnName (index: Index) (board: Board) =
        board.applyEvent (
            CardMoved(
                DomainEvent.newWithPayload { BoardId = board.Id
                                             CardId = card.Id
                                             NewColumn = columnName
                                             ColumnIndex = index }
            )
        )

    let withoutColumn columnName (board: Board) =
        board.applyEvent (
            ColumnRemoved(
                DomainEvent.newWithPayload
                    { BoardId = board.Id
                      ColumnName = columnName }
            )
        )

    let create (event: BoardCreatedPayload DomainEvent) =
        { Id = event.Payload.BoardId
          Name = event.Payload.Name
          Columns = event.Payload.ColumnNames |> NonEmptyList.map Column.WithName
          History = List.singleton (BoardEvent.BoardCreated event) }
