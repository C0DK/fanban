namespace Fanban.Domain

type BoardCreatedPayload =
    { BoardId: BoardId
      Name: Name
      ColumnNames: Name NonEmptyList }

and BoardNameSetPayload =
    { BoardId: BoardId
      Name: Name }

and ColumnAddedPayload =
    { BoardId: BoardId
      ColumnName: Name
      Index: Index }

and ColumnRemovedPayload =
    { BoardId: BoardId
      ColumnName: Name }

and CardAddedPayload = { BoardId: BoardId; Card: Card }

and CardMovedPayload =
    { BoardId: BoardId
      CardId: CardId
      NewColumn: Name
      ColumnIndex: Index }

and BoardEvent =
    | BoardNameSet of BoardNameSetPayload DomainEvent
    | ColumnAdded of ColumnAddedPayload DomainEvent
    | ColumnRemoved of ColumnRemovedPayload DomainEvent
    | CardAdded of CardAddedPayload DomainEvent
    | CardMoved of CardMovedPayload DomainEvent
    | BoardCreated of BoardCreatedPayload DomainEvent

module BoardCreated =
    let Create name columns =
        DomainEvent.newWithPayload
            { BoardId = BoardId.New()
              Name = name
              ColumnNames = columns }
