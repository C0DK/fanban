namespace Fanban.Domain

type Index =
    | Index of int
    | End
    | Beginning

module List =
    let insertAt (toIndex: Index) (value: 'a) (source: 'a list) =
        match toIndex with
        | Index index -> List.insertAt index value source
        | End -> List.insertAt source.Length value source
        | Beginning -> value :: source


type Name = Name of string


module Name =
    let create value =
        if value |> String.length > 0 then Ok (Name value) else Error "Expected non empty string!"

    let value (Name e) = e

type NonEmptyList<'a when 'a : comparison> =
    | NonEmptyList of List<'a>

    member this.Value =
        match this with
        | NonEmptyList e -> e

    member this.Length = this.Value.Length

    member this.First = this.Value[0]

module NonEmptyList =

    let create value =
        if value |> Seq.length > 0 then Ok (NonEmptyList value) else Error "Expected non empty list!"

    let value (NonEmptyList e) = e

    let map mapper list = value list |> List.map mapper |> NonEmptyList

    let filter predicate list = value list |> List.filter predicate |> NonEmptyList

    let length list = value list |> List.length

    let insertAt (toIndex: Index) (newValue: 'a) (source: 'a NonEmptyList) =
        List.insertAt toIndex newValue (value source) |> NonEmptyList

    let mapFirstValue mapper (source: 'a NonEmptyList) =
            match (value source) with
            | head :: tail -> (mapper head) :: tail
            | _ -> failwith "Empty list - Invariance failed"
            |> NonEmptyList

