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
      Payload: 'Payload }

    member this.map(selector: 'Payload -> 'NewPayload) : DomainEvent<'NewPayload> =
        { Id = this.Id
          Created = this.Created
          Payload = (selector this.Payload) }

module DomainEvent =
    let newWithPayload (payload: 'Payload) =
        { Id = EventId.New()
          Created = DateTime.UtcNow
          Payload = payload }

    let map (selector: 'OldPayload -> 'NewPayload) (event: DomainEvent<'OldPayload>) : DomainEvent<'NewPayload> =
        { Id = event.Id
          Created = event.Created
          Payload = (selector event.Payload) }
