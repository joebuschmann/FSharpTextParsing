FSharpTextParsing
=================

This is a simple example of parsing a log file with F#. It uses Active Patterns to match on the text and convert the text to representative data structures.

The solution includes a sample file log of TCP time wait states, a driver application, a function library, and unit tests.

For example, the Active Pattern definition below matches a string representing an IP address and returns a tuple with 5 integers.



    // Attempt to convert a string value into an IP address
    // represented by a tuple with four or five integers with
    // the port being optional.
    let (|IP|_|) (input:string) =
      let delimChars = [|'.'; ':'|]
      let arr = input.Split(delimChars)
      match arr.Length with
      | 4 -> Some((int(arr.[0]), int(arr.[1]), int(arr.[2]), int(arr.[3]), 0))
      | 5 -> Some((int(arr.[0]), int(arr.[1]), int(arr.[2]), int(arr.[3]), int(arr.[4])))
      | _ -> None
