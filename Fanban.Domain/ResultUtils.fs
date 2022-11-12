namespace Fanban.Domain

open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result

module ResultHelpers =

    let Return value (result: Result<Unit, string>) = result |> Result.map (fun () -> value)

    let And (resultA: Result<Unit, string>) (resultB: Result<Unit, string>) =
        resultA |> Result.bind (fun _ -> resultB)

    let rec Ands (results: Result<Unit, string> list) (result: Result<Unit, string>) =
        match results with
        | [] -> result
        | [ head ] -> head |> And result
        | head :: tail -> (result |> And head) |> Ands tail

    let GivenValidThenReturn value (results: Result<Unit, string> list) =
        match results with
        | [] -> Ok value
        | head :: tail -> Ands tail head |> Return value

module Result =
    let okOrFail result = result |> Result.valueOr failwith
