# Top level expression
`Expression = ValueExpression (Operation ValueExpression)*`
### Examples
- `7`
- `1d20`
- `2d8h2 + 3`
- `4d6 + 2d8 - 2`

## Operations
The top level expression is evaluated from left to right, no exceptions, no pemdas here!
- Add: `+`
- Subtract: `-`

## Supported Value Expressions
### Flat Values
- Flat modifiers: `7`, `-8`, `-29384`. These are signed integers and internally support up to 32 bits.
### Dice Expressions
- Constant Die: `1d20`, `3d6kh`, `10d8L5`. These represent dice with 1 or more sides and are written in a `ndX` fashion where `n` is the number of dice rolled and `X` represents the range `1, 2, ..., X` inclusive. E.g., `1d20` reads as "Roll one number between 1 and 20 inclusive". Regex: `/[0-9]+d[0-9]+/i`
- Range Die: `1d[-3 .. 3]`, `1d[1..20]` (equivalent to `1d20`). These `[a .. b]` expressions represent consecutive whole numbers between `a` and `b` inclusive.
- Enumeration Die: `1d[1,3,5]`, `4d[1,1,1,2]`, `10d[1,2,3,4]` (equivalent to `10d4`). Enumerations list out all the potential values that the dice expression can roll. Repetitions are allowed and respected and can be used to represent weighted outcomes.

## Dice Expression Modifiers
- Keep High / Advantage: `KH`, `KH2`, `H`, `H4` (case insensitive). Writing `4d20h2` reads as "Roll 4 dice representing numbers between 1 and 20 inclusive. Of those 4 results, keep the highest 2 values". E.g., `4d20h2 => [1,6,18,9] => [18, 9]`. Regex: `/k?h[0-9]*/i`
- Keep Low / Disadvantage: `LH`, `LH2`, `L`, `L4` (case insensitive). Same behaviour as "Keep High" but you keep the lowest N results instead. Regex: `/k?l[0-9]*/i`
- Total (default): `T`. Add all of the resulting numbers together. Regex: `/t?/i`

###### Disclaimer
Regex values provided only for demonstration purposes in order to convey some of the building blocks of these expressions. If you're interested in precisely what this parsers understands please look at the [parser](https://github.com/Eliemer/Dice-Calculator/blob/master/src/Parser.fs)
