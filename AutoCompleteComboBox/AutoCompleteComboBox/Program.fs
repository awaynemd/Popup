namespace AutoCompleteComboBox

open System
open Elmish
open Elmish.WPF
open System.Windows


module App =

    type Model =
       { IsOpen: bool
         Suggestions: string list
         Text: string
         SelectedItem: string option
       }

    let init () =
      { IsOpen = false
        Suggestions = [ "Alabama"; "Alaska"; "Arizona"; "Arkansas"; "California"; "Colorado"; "Connecticut"; 
                        "Delaware"; "Florida"; "Georgia"; "Hawaii"; "Idaho"; "Illinois"; "Indiana"; "Iowa" ]
        Text = "A"
        SelectedItem = None
      }

    type Msg =
      | SetOpen of bool
      | SetText of string
      | SetSelectedItem of string option

    let CloseAutoSuggestionBox m =
        { m with IsOpen = false }

    let OpenAutoSuggestionBox m  =
         { m with IsOpen = true }

    let filteredSuggestions m =
        m.Suggestions |> List.filter (fun s -> s.ToLower().Contains(m.Text.ToLower()))

    let update msg m  =
        match msg with
        | SetOpen o -> { m with IsOpen = o }
        | SetText s -> { m with Text = s; IsOpen = true }
        | SetSelectedItem i -> { m with SelectedItem = i; IsOpen = i.IsNone; Text = if i.IsSome then "" else m.Text }

    let bindings () = [
       "Suggestions" |> Binding.oneWay filteredSuggestions
       "IsOpen" |> Binding.twoWay ( (fun m -> m.IsOpen), SetOpen)
       "Text" |> Binding.twoWay((fun m -> m.Text), SetText)
       "SelectedSuggestion" |> Binding.twoWayOpt((fun m -> m.SelectedItem), SetSelectedItem)
    ]

    let vm = ViewModel.designInstance (init ()) (bindings ())

    let main window =
      Program.mkSimpleWpf init update bindings
      |> Program.withConsoleTrace
      |> Program.runWindowWithConfig
         { ElmConfig.Default with LogConsole = true }      
         window