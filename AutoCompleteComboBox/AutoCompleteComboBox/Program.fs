namespace AutoCompleteComboBox

open System
open Elmish.WPF
open System.Windows


module App =

    type Model =
       { Visibility: System.Windows.Visibility
         IsOpen: bool
         AutoSuggestionList: string list
         Suggestions: string list option
         Text: string
       }

    let init () = {
      Visibility = Visibility.Visible   
      IsOpen = true
      AutoSuggestionList = [ "Alabama"; "Alaska"; "Arizona"; "Arkansas"; "California"; "Colorado"; "Connecticut"; 
                             "Delaware"; "Florida"; "Georgia"; "Hawaii"; "Idaho"; "Illinois"; "Indiana"; "Iowa" ]
      Text = "Hi"
      Suggestions = None
      }

    type Msg =
      | SetText of string
      | AutoListSelectionChanged
      | AutoTextBoxTextChanged
      | MakeVisible
      | Collapse
      | NoOp

    let CloseAutoSuggestionBox m =
        { m with Visibility = Visibility.Collapsed; IsOpen = false }  

    let OpenAutoSuggestionBox m  =
         { m with Visibility = Visibility.Visible; IsOpen = true } 

    let Suggest m =
        let SuggestionList =
            m.AutoSuggestionList |> List.filter (fun s -> s.ToLower().Contains(m.Text.ToLower())) |> Some
        {m with Suggestions = SuggestionList }


    let update msg m  =
        match msg with
        | SetText s  -> { m with Text = s }
        | AutoListSelectionChanged
        | AutoTextBoxTextChanged
        | MakeVisible
        | Collapse
        | NoOp -> m

    let handleAutoTextBoxTextChanged obj m =
      let notNullOrEmpty = not << System.String.IsNullOrEmpty

      match m.Text with
      | null  -> CloseAutoSuggestionBox m          |> ignore 
      | _ -> m |> OpenAutoSuggestionBox |> Suggest |> ignore

      NoOp

    let bindings()  = [     
       "AutoSuggestionList" |> Binding.oneWay ( fun m -> m.AutoSuggestionList)
       //"SelectedSuggestion"
       "Visibility" |> Binding.oneWay ( fun m -> m.Visibility)
       "IsOpen"  |> Binding.oneWay ( fun m -> m.IsOpen)
       "Text"  |> Binding.twoWay((fun m -> m.Text),  SetText)
       "AutoTextBoxTextChanged" |> Binding.cmdParam (handleAutoTextBoxTextChanged)
    ]

    [<EntryPoint; STAThread>]
    let main argv =
      Program.mkSimpleWpf init update bindings
      |> Program.runWindowWithConfig
         { ElmConfig.Default with LogConsole = true }      
         (AutoCompleteComboBox.Views.MainWindow())