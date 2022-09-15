namespace Fanban.Domain

open System

[<Struct>]
type BoardId =
    private
    | BoardId of Guid

    static member New() = BoardId(Guid.NewGuid())
    static member Parse (value: string) = BoardId(Guid.Parse(value))
    static member TryParse (value: string) =
        let couldParse, result = Guid.TryParse(value)
        if couldParse then Some (BoardId result) else None
    member this.Value = let (BoardId i) = this in i
    override this.ToString() = this.Value.ToString()
