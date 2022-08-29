module Fanban.Domain.Tests.Card

open Fanban.Domain
open Fanban.Domain.Tests.TestHelper
open Xunit

module New =
    [<Fact>]
    let ``with empty string, fails`` () = Card.New "" |> shouldError
