module Beatha.Parser

module Parse =
    val rule : string -> Result<Core.Rule, string>