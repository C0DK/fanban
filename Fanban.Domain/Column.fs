namespace Fanban.Domain

type Column =
    { Name: Name
      Cards: Card list }

    static member WithName name = { Name = name; Cards = List.empty }

module Column =
    let WithCard card column = { column with Cards = List.insertAt Beginning card column.Cards }