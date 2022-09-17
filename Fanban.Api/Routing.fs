module Fanban.Api.Routing
open Giraffe
open Fanban.Api

let webApp : HttpHandler = choose [
        GET >=>
            choose [
                route "/" >=> text "Hello World"
                route "/boards/" >=> Endpoints.boards Configuration.repository.GetBoards
                routef "/board/%O/" (Endpoints.getBoard Configuration.repository.GetBoard)
            ]
        setStatusCode 404 >=> text "Not Found" ]