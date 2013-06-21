
open System
open System.Collections.Generic
open System.IO
open DataStructures

let lines = File.ReadAllLines("out_time0.txt") |> Array.toList
let items = parseFile lines

// Find the times of each TIME_WAIT record and return a tuple
// with the time and IP address.  Since the time was written
// to the file once for a set of addresses, it has to be
// passed into the util function as state.  The accumulator
// parameter exists to enable tail call optimization.
let getTimeWaitTimes items =
  let rec util items (currentTime:DateTime) acc =
      match items with
      | hd :: tl ->
        let currentTime = match hd with
                          | TimeRecord newTime -> newTime
                          | _ -> currentTime

        let acc = match hd with
                  | TimeWaitRecord (val1, val2, val3, val4, val5) ->
                    (currentTime, (val1, val2, val3, val4, val5)) :: acc
                  | _ -> acc

        //printfn "Current Time %s" (currentTime.ToString())
        util tl currentTime acc
      | [] -> acc
  util items DateTime.MinValue []

let timeWaits = getTimeWaitTimes items

// Create a map of the number of IP addresses in a
// TIME_WAIT state at a give time.  The time is the key.
let counts = timeWaits |> List.fold (fun (state:Map<_,_>) (time, _) ->
                                      if (state.ContainsKey(time)) then
                                        state |> Map.map
                                                (fun k v ->
                                                  if k = time then
                                                    v + 1
                                                  else v)
                                      else
                                        state.Add(time, 1)
                                    ) (Map.empty)

// Write the results to a CSV file.
let file = File.CreateText("results.csv")
file.WriteLine("Time,Count")

counts |> Map.iter (fun k v -> file.WriteLine("{0},{1}", k, v) )

file.Flush()

ignore(Console.ReadLine())

