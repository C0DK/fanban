namespace Fanban.Domain

type BoardCreatedPayload =
    { BoardId: BoardId
      Name: Name
      ColumnNames: Name NonEmptyList }

and BoardNameSetPayload = { BoardId: BoardId; Name: Name }

and ColumnAddedPayload =
    { BoardId: BoardId
      ColumnName: Name
      Index: Index }

and ColumnRemovedPayload = { BoardId: BoardId; ColumnName: Name }

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

    member this.Id =
        match this with
        | BoardNameSet domainEvent -> domainEvent.Id
        | ColumnAdded domainEvent -> domainEvent.Id
        | ColumnRemoved domainEvent -> domainEvent.Id
        | CardAdded domainEvent -> domainEvent.Id
        | CardMoved domainEvent -> domainEvent.Id
        | BoardCreated domainEvent -> domainEvent.Id

    member this.BoardId =
        match this with
        | BoardNameSet domainEvent -> domainEvent.Payload.BoardId
        | ColumnAdded domainEvent -> domainEvent.Payload.BoardId
        | ColumnRemoved domainEvent -> domainEvent.Payload.BoardId
        | CardAdded domainEvent -> domainEvent.Payload.BoardId
        | CardMoved domainEvent -> domainEvent.Payload.BoardId
        | BoardCreated domainEvent -> domainEvent.Payload.BoardId

    member this.typeName =
        match this with
        | BoardNameSet _ -> nameof BoardNameSet
        | ColumnAdded _ -> nameof ColumnAdded
        | ColumnRemoved _ -> nameof ColumnRemoved
        | CardAdded _ -> nameof CardAdded
        | CardMoved _ -> nameof CardMoved
        | BoardCreated _ -> nameof BoardCreated

    member this.HumanReadableString =
        match this with
        | BoardNameSet domainEvent -> $"Set Board name to '{domainEvent.Payload.Name}'"
        | ColumnAdded domainEvent -> $"Added Column '{domainEvent.Payload.ColumnName}' at {domainEvent.Payload.Index}"
        | ColumnRemoved domainEvent -> $"Removed Column '{domainEvent.Payload.ColumnName}'"
        | CardAdded domainEvent ->
            $"Added card '{domainEvent.Payload.Card.Name}' ('{domainEvent.Payload.Card.Id.ToShortString()}')"
        | CardMoved domainEvent ->
            $"Moved card '{domainEvent.Payload.CardId.ToShortString()}' to '{domainEvent.Payload.NewColumn}' at {domainEvent.Payload.ColumnIndex}"
        | BoardCreated domainEvent -> $"Created board '{domainEvent.Payload.Name}'"


module BoardCreated =
    let Create name columns =
        DomainEvent.newWithPayload
            { BoardId = BoardId.New()
              Name = name
              ColumnNames = columns }
