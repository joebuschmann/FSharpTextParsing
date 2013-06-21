module DataStructures

open System

type LineRecord =
| TimeRecord of DateTime
| TimeWaitRecord of int * int * int * int * int

// Attempt to convert a string value into a date.
let (|Date|_|) (input:string) =
  let (success, date) = DateTime.TryParse(input)
  if (success) then Some(date)
  else None

// Attempt to convert a string value into an IP address
// represented by a tuple with four or five integers with
// the port being optional.
let (|IP|_|) (input:string) =
  let delimChars = [|'.'; ':'|]
  let arr = input.Split(delimChars)
  match arr.Length with
  | 4 -> Some((int(arr.[0]), int(arr.[1]), int(arr.[2]), int(arr.[3]), 0))
  | 5 -> Some((int(arr.[0]), int(arr.[1]), int(arr.[2]), int(arr.[3]),
               int(arr.[4])))
  | _ -> None

// Attempt to convert a time record.
let (|TimeEntry|_|) (line:string) =
  let line = line.Trim().Replace("Time : ", "").Replace('_', ' ')
  match line with
  | Date dateTime -> Some(TimeRecord(dateTime))
  | _ -> None

// Attempt to convert a TIME_WAIT record.
let (|TimeWaitEntry|_|) (line:string) =
  let line = line.Replace("TCP", "").Trim()
  let index = line.IndexOf(' ')
  
  match index with
  | -1 -> None
  | index ->
    let line = line.Substring(0, index)
    match line with
    | IP (ip) -> Some(TimeWaitRecord ip)
    | _ -> None

// Parses lines from the file into a list of LineRecords.
let parseFile lines =
  let lines = lines |> List.rev
  let rec parseFileUtil lines acc =
    match lines with
    | hd :: tl ->
      let acc = match hd with
                | TimeEntry timeRecord -> timeRecord :: acc
                | TimeWaitEntry timeWaitRecord -> timeWaitRecord :: acc
                | _ -> acc
      parseFileUtil tl acc
    | [] -> acc
  parseFileUtil lines []
