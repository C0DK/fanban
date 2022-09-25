module Fanban.Domain.Tests.Primitives


open Fanban.Domain
open FsToolkit.ErrorHandling
open Xunit
open FsUnitTyped
open FsCheck
open FsCheck.Xunit

module NonEmptyString =
    module create =
        [<Fact>]
        let ``given empty string, fails`` () =
                (Name.create "")
                |> shouldEqual (Error "Expected non empty string!")

        [<Fact>]
        let ``given string, is good`` () =
                (Name.create "test")
                |> shouldEqual (Ok (Name "test"))

        [<Property>]
        let ``given non empty string, ok`` (value: string NonNull) =
            let nonEmptyString = value.Get;
            nonEmptyString <> "" ==> (
                (Name.create nonEmptyString) = (Ok (Name nonEmptyString)))

module NonEmptyList =
    module create =
        [<Fact>]
        let ``given empty list, fails`` () =
                (NonEmptyList.create List.Empty)
                |> shouldEqual (Error "Expected non empty list!")

        [<Property>]
        let ``given non empty list, ok`` (value: string list) =
            not value.IsEmpty ==> (
                (NonEmptyList.create value) = (Ok (NonEmptyList value)))
