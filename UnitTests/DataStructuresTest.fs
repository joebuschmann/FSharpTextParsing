
module DataStructuresTest

open System
open DataStructures
open Xunit

[<Fact>]
let TestDateActivePattern() =
  let matchDate s =
    match s with
    | Date date -> date
    | _ -> DateTime.MinValue

  let date1 = matchDate ""
  let date2 = matchDate "4/5/2010"
  let date3 = matchDate "4-5-2010"
  let date4 = matchDate "4-35-2010"

  Assert.Equal(DateTime.MinValue, date1)
  Assert.Equal(DateTime(2010, 4, 5), date2)
  Assert.Equal(DateTime(2010, 4, 5), date3)
  Assert.Equal(DateTime.MinValue, date4)

[<Fact>]
let TestTimeEntryActivePattern() =
  let matchTimeRecord line =
    match line with
    | TimeEntry (TimeRecord dateTime) -> dateTime
    | _ -> DateTime.MinValue
  
  let line1 = matchTimeRecord " Time : 2011-11-09_08:27:05"
  let line2 = matchTimeRecord " Time : "
  let line3 = matchTimeRecord "2011-11-09_08:35:52"
  let line4 = matchTimeRecord "  TCP    10.28.65.14:1800       10.28.65.15:808        TIME_WAIT       0"

  Assert.Equal(DateTime(2011, 11, 9, 8, 27, 5), line1)
  Assert.Equal(DateTime.MinValue, line2)
  Assert.Equal(DateTime(2011, 11, 9, 8, 35, 52), line3)
  Assert.Equal(DateTime.MinValue, line4)

[<Fact>]
let TestIPActivePattern() =
  let matchIp input =
    match input with
    | IP ip -> Some(ip)
    | _ -> None

  let test1 = ("10.28.65.14:1800", (10, 28, 65, 14, 1800))
  let test2 = ("10.28.65.14", (10, 28, 65, 14, 0))
  let matchfun = fst >> matchIp

  let result1 = matchfun test1
  let result2 = matchfun test2

  let checkResult result expected =
    match result with
    | Some(ip) -> Assert.Equal(ip, expected)
    | None -> failwith "IP match not found"

  checkResult result1 (snd test1)
  checkResult result2 (snd test2)

[<Fact>]
let TestTimeWaitEntryActivePattern() =
  let matchEntry line =
    match line with
    | TimeWaitEntry (TimeWaitRecord (v1, v2, v3, v4, v5)) -> Some((v1, v2, v3, v4, v5))
    | _ -> None

  let line1 = " Time : 2011-11-09_08:27:05"
  let line2 = " Time : "
  let line3 = "2011-11-09_08:35:52"
  let line4 = "  TCP    10.28.65.14:1800       10.28.65.15:808        TIME_WAIT       0"

  Assert.Equal(None, (matchEntry line1))
  Assert.Equal(None, (matchEntry line2))
  Assert.Equal(None, (matchEntry line3))
  Assert.Equal(Some(10, 28, 65, 14, 1800), (matchEntry line4))

[<Fact>]
let TestParseFile() =
  let verifyTimeRecord item expected =
    match item with
    | TimeRecord(time) -> Assert.Equal(expected, time)
    | _ -> failwith("Item is not a date.")

  let verifyTimeWaitRecord item expected =
    match item with
    | TimeWaitRecord(val1, val2, val3, val4, val5) ->
        Assert.Equal(expected, (val1, val2, val3, val4, val5))
    | _ -> failwith("Item is not a time wait record")

  let lines = [" Time : 2011-11-09_08:28:26";
               " Time : ";
               "2011-11-09_08:35:52";
               "  TCP    10.28.65.14:1800       10.28.65.15:808        TIME_WAIT       0"]

  let items = parseFile lines

  Assert.Equal(3, items.Length)

  let item1 = items.[0]
  let item2 = items.[1]
  let item3 = items.[2]

  verifyTimeRecord item1 (DateTime(2011, 11, 9, 8, 28, 26))
  verifyTimeRecord item2 (DateTime(2011, 11, 9, 8, 35, 52))
  verifyTimeWaitRecord item3 (10, 28, 65, 14, 1800)