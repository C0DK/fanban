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

    member this.cards = NonEmptyList.value this.Columns |> Seq.collect (fun column -> column.Cards)

    member this.tryGetCard(cardId: CardId) =
        this.cards |> Seq.tryFind (fun card -> card.Id = cardId)

    member this.getCard(cardId: CardId) =
        this.tryGetCard cardId |> Result.requireSome (cardDoesntExist cardId)

    member this.tryGetColumn(columnName: Name) =
        this.Columns
        |> NonEmptyList.value
        |> Seq.tryFind (fun column -> column.Name = columnName)

    member this.getColumn(columnName: Name) =
        this.tryGetColumn columnName
        |> Result.requireSome (columnDoesntExist columnName)


    member board.RequireContainsColumn(name: Name) =
        board.getColumn name |> Result.map (fun _ -> ())

    member board.RequireColumnIsEmpty(columnName: Name) =
        (board.getColumn columnName)
        |> Result.map (fun column -> column.Cards)
        |> Result.bind (Result.requireEmpty (cannotRemoveNonEmptyColumn columnName))


module Board =

    let private applyCardAdded (payload: CardAddedPayload) (board: Board) =
        [ (board.cards |> Seq.exists (fun card -> card.Id = payload.Card.Id))
          |> Result.requireFalse (cardAlreadyExistExist payload.Card.Id) ]
        |> GivenValidThenReturn
            { board with Columns = board.Columns |> (NonEmptyList.mapFirstValue (Column.WithCard payload.Card)) }

    let private applyCardMoved (payload: CardMovedPayload) (board: Board) =
        board.RequireContainsColumn payload.NewColumn
        >>= fun _ -> (board.getCard payload.CardId)
        |> Result.map (fun card ->
            { board with
                Columns =
                    board.Columns
                    |> NonEmptyList.map (fun column ->
                        match column with
                        | column when column.Name = payload.NewColumn ->
                            { column with Cards = column.Cards |> List.insertAt payload.ColumnIndex card }
                        | column ->
                            { column with Cards = column.Cards |> Seq.except (List.singleton card) |> Seq.toList }) })

    let private applyColumnAdded (payload: ColumnAddedPayload) (board: Board) =
        [ (board.tryGetColumn payload.ColumnName)
          |> Result.requireNone (columnAlreadyExist payload.ColumnName) ]
        |> GivenValidThenReturn
            { board with
                Columns =
                    board.Columns
                    |> NonEmptyList.insertAt payload.Index (Column.WithName payload.ColumnName) }

    let private applyColumnRemoved (payload: ColumnRemovedPayload) (board: Board) =
        [ board.RequireContainsColumn payload.ColumnName
          board.RequireColumnIsEmpty payload.ColumnName
          (board.Columns |> NonEmptyList.length) > 1
          |> Result.requireTrue boardCannotHaveZeroColumns ]
        |> GivenValidThenReturn
            { board with
                Columns =
                    board.Columns
                    |> NonEmptyList.filter (fun column -> column.Name <> payload.ColumnName) }

    // These private methods should not be called directly as they leave the board in an invalid state, by not updating the history
    let private applyBoardNameSet (payload: BoardNameSetPayload) (this: Board) = Ok { this with Name = payload.Name }

    let private withEventInHistory (event: BoardEvent) (board: Board) =
        { board with History = event :: board.History }

    let applyEvent (event: BoardEvent) =
        match event with
        | CardAdded event -> applyCardAdded event.Payload
        | CardMoved event -> applyCardMoved event.Payload
        | ColumnAdded event -> applyColumnAdded event.Payload
        | ColumnRemoved event -> applyColumnRemoved event.Payload
        | BoardNameSet event -> applyBoardNameSet event.Payload
        | BoardCreated _ -> (fun _ -> Error cannotCreateExistingBoard)
        >> Result.map (withEventInHistory event)

    let withName name (board: Board) =
        board
        |> applyEvent (BoardNameSet(DomainEvent.newWithPayload { BoardId = board.Id; Name = name }))

    let withColumnAt index columnName (board: Board) =
        applyEvent
            (ColumnAdded(
                DomainEvent.newWithPayload
                    { BoardId = board.Id
                      ColumnName = columnName
                      Index = index }
            ))
            board

    let withCard card (board: Board) =
        applyEvent (CardAdded(DomainEvent.newWithPayload ({ BoardId = board.Id; Card = card }))) board

    let moveCard (card: Card) columnName (index: Index) (board: Board) =
        applyEvent
            (CardMoved(
                DomainEvent.newWithPayload
                    { BoardId = board.Id
                      CardId = card.Id
                      NewColumn = columnName
                      ColumnIndex = index }
            ))
            board

    let withoutColumn columnName (board: Board) =
        applyEvent
            (ColumnRemoved(
                DomainEvent.newWithPayload
                    { BoardId = board.Id
                      ColumnName = columnName }
            ))
            board

    let create (event: BoardCreatedPayload DomainEvent) =
        { Id = event.Payload.BoardId
          Name = event.Payload.Name
          Columns = event.Payload.ColumnNames |> NonEmptyList.map Column.WithName
          History = List.singleton (BoardCreated event) }


    let ApplyEvents (events: BoardEvent list) =
        result {
            let! firstEvent = events |> Seq.tryHead |> Result.requireSome "Events were empty!"

            let! board =
                match firstEvent with
                | BoardCreated domainEvent -> Ok(create domainEvent)
                | _ -> Error $"First event was not a {nameof (BoardCreated)} event"


            let folder (boardResult: Result<Board, string>) (event: BoardEvent) =
                boardResult >>= (fun board -> applyEvent event board)

            let! board = events.Tail |> Seq.fold folder (Ok board)

            return board
        }
