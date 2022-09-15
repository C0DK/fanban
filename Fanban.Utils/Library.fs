module Fanban.Utils.Option

let FromTuple ((parsed, value) : bool * 'a) =
    if parsed then Some value else None
