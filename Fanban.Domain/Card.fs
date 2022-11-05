namespace Fanban.Domain

open System

[<Struct>]
type CardId =
    private
    | CardId of Guid

    static member New() = CardId(Guid.NewGuid())
    member this.Value = let (CardId i) = this in i

    override this.ToString () = this.Value.ToString ()
    member this.ToShortString () = (this.Value.ToString ())[..6]

and Card =
    { Id: CardId
      Name: string
      Description: string option
      Created: DateTime
      Updated: DateTime }

    static member New name =
        if (not (String.IsNullOrWhiteSpace name)) then
            Ok
                { Id = CardId.New()
                  Name = name
                  Description = None
                  Created = DateTime.Now
                  Updated = DateTime.Now }
        else
            Error "Issue Name cannot be empty."
