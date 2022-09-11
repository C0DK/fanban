namespace Fanban.Domain

[<Struct>]
type ColumnName =
    private
    | ColumnName of string

    static member New name = ColumnName name
    member this.Value = let (ColumnName i) = this in i
    override this.ToString() =
        this.Value

type Column =
    { Name: ColumnName
      Cards: Card list }

    static member WithName name = { Name = name; Cards = List.empty }
