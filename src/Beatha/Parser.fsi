module Beatha.Parser

type Rule =
    { Birth: int list
      Survival: int list }

val rule : string -> Result<Rule, string>