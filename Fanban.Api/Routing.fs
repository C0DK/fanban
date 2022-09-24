module Fanban.Api.Routing
open System.Globalization
open Fanban.Application.Commands
open Giraffe
open Fanban.Api

let culture = (Some CultureInfo.InvariantCulture)

let webApp : HttpHandler = choose [
        GET >=> route "/" >=> text "Hello World"
        GET >=> route "/boards/" >=> Endpoints.boards Configuration.repository.GetBoards
        POST >=> route "/boards/" >=> bindModel<CreateBoard> culture (Endpoints.createBoard Configuration.repository.AddBoard)
        GET >=> routef "/board/%O/" (Endpoints.getBoard Configuration.repository.GetBoard)
        setStatusCode 404 >=> text "Not Found" ]