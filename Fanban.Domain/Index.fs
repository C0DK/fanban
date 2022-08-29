module Fanban.Domain.Index


type Index =
    | Index of int
    | End
    | Beginning

let insertAt (toIndex: Index) (value: 'a) (source: 'a list) =
    match toIndex with
    | Index index -> List.insertAt index value source
    | End -> List.insertAt (source.Length) value source
    | Beginning -> value :: source
