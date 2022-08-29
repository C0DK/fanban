module Fanban.Domain.BoardError

let cardDoesntExist (id: CardId) = $"Card with ID='{id}' doesn't exist"
let cardAlreadyExistExist (id: CardId) = $"Card with ID='{id}' already exist"
let columnDoesntExist (columnName: ColumnName) = $"Column '{columnName}' doesn't exist"
let columnAlreadyExist (columnName: ColumnName) = $"Column '{columnName}' already exist"

let cannotRemoveNonEmptyColumn (columnName: ColumnName) =
    $"Cannot remove column '{columnName}' when it is not empty"

let boardNameCannotBeEmpty = "Board name cannot be empty"

let boardCannotHaveZeroColumns = "Board is required to have at least one column"
let cannotCreateExistingBoard = "Board is already created - an additional event makes no sense"
