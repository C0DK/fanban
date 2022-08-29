namespace Fanban.Domain

open System

[<Struct>]
type BoardId =
    private
    | BoardId of Guid

    static member New() = BoardId(Guid.NewGuid())
    member this.Value = let (BoardId i) = this in i
