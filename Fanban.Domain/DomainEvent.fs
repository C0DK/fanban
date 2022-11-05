namespace Fanban.Domain

open System

[<Struct>]
type EventId =
    private
    | EventId of Guid

    static member New() = EventId(Guid.NewGuid())
    static member FromGuid(value) = EventId(value)
    member this.Value = let (EventId i) = this in i
    override this.ToString () = this.Value.ToString ()
    member this.ToShortString () = (this.Value.ToString ())[..6]


type DomainEvent<'Payload> =
    { Id: EventId
      Created: DateTime
      Payload: 'Payload }

module DomainEvent =
    let newWithPayload (payload: 'Payload) =
        { Id = EventId.New()
          Created = DateTime.UtcNow
          Payload = payload }
