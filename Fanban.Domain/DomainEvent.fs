namespace Fanban.Domain

open System

[<Struct>]
type EventId =
    private
    | EventId of Guid

    static member New() = EventId(Guid.NewGuid())
    static member FromGuid(value) = EventId(value)
    member this.Value = let (EventId i) = this in i

type DomainEvent<'Payload> =
    { Id: EventId
      Created: DateTime
      payload: 'Payload }

module DomainEvent =
    let newWithPayload (payload: 'Payload) =
        {
            Id = EventId.New()
            Created = DateTime.UtcNow
            payload= payload
        }
