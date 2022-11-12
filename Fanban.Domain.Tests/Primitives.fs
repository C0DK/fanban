module Fanban.Domain.Tests.Primitives


open Fanban.Domain
open FsToolkit.ErrorHandling
open Xunit
open FsUnitTyped
open FsCheck
open FsCheck.Xunit

module Name =
    module create =
        [<Fact>]
        let ``given empty string, fails`` () =
            (Name.create "") |> shouldEqual (Error "Expected non empty string!")

        [<Property>]
        let ``given non empty string, ok`` (value: string NonNull) =
            let nonEmptyString = value.Get

            nonEmptyString <> ""
            ==> ((Name.create nonEmptyString) = (Ok(Name nonEmptyString)))

module NonEmptyList =
    module create =
        [<Fact>]
        let ``given empty list, fails`` () =
            (NonEmptyList.create List.Empty)
            |> shouldEqual (Error "Expected non empty list!")

        [<Property>]
        let ``given non empty list, ok`` (value: string list) =
            not value.IsEmpty ==> ((NonEmptyList.create value) = (Ok(NonEmptyList value)))

    module map =

        [<Property(Skip = "The constructor is sorta broken")>]
        let ``given any NonEmptyList and any map, result is non-empty``
            (
                value: string NonEmptyList,
                mapper: string -> string
            ) =
            (NonEmptyList.map mapper value)
            |> NonEmptyList.value
            |> fun v -> not (Seq.isEmpty v)
