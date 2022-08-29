namespace Fanban.Domain.Tests

open FsUnit.Xunit
open Xunit.Sdk

module TestHelper =
    let shouldMatch predicate a =
        predicate a |> fun _ -> raise (XunitException "Predicate was not true!")

    let shouldError result =
        match result with
        | Error _ -> ()
        | Ok _ -> raise (XunitException "Expected Error but got OK")

    let shouldNotBeEmpty (seq: seq<_>) = should not' (be Empty)
