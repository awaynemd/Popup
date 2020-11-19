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
         Text: string option
         SelectedItem: string option
       }

    let init () = {
      Visibility = Visibility.Collapsed  
      IsOpen = true
      AutoSuggestionList = [ "Alabama"; "Alaska"; "Arizona"; "Arkansas"; "California"; "Colorado"; "Connecticut"; 
                             "Delaware"; "Florida"; "Georgia"; "Hawaii"; "Idaho"; "Illinois"; "Indiana"; "Iowa" ]
      Text = Some "Hi"
      Suggestions = None
      SelectedItem = None
      }

    type Msg =
      | SetOpen of bool
      | SetText of string option
      | SetSelectedItem of string option
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
        match m.Text with
        | Some t -> 
            let SuggestionList =
                m.AutoSuggestionList |> List.filter (fun s -> s.ToLower().Contains(t.ToLower())) |> Some
            {m with Suggestions = SuggestionList }                                 //|> printfn "list is %A"
        | None -> m

    let handleAutoListSelectionChanged m =
        match m.SelectedItem with
        | Some i -> m |> CloseAutoSuggestionBox |> (fun m -> {m with Text = Some i})
        | None -> m |> CloseAutoSuggestionBox |> (fun m -> {m with Text = None})
        

    let handleAutoTextBoxTextChanged m =
        let notNullOrEmpty = not << System.String.IsNullOrEmpty

        match m.Text with
        | None  -> CloseAutoSuggestionBox m          
        | _ -> m |> OpenAutoSuggestionBox |> Suggest 
      

    let update msg m  =
        match msg with
        | SetOpen o -> { m with IsOpen = o }
        | SetText s  -> { m with Text = s }
        | SetSelectedItem i -> {m with SelectedItem = i }
        | AutoListSelectionChanged -> m |> handleAutoListSelectionChanged
        | AutoTextBoxTextChanged -> m |> handleAutoTextBoxTextChanged
        | MakeVisible
        | Collapse
        | NoOp -> m

    

    let bindings()  = [     
       "Suggestions" |> Binding.oneWay ( fun m -> 
                                                    match m.Suggestions with
                                                    | Some s -> s
                                                    | None -> []
                                        )
       "Visibility" |> Binding.oneWay ( fun m -> m.Visibility)
       "IsOpen"  |> Binding.twoWay ( (fun m -> m.IsOpen), SetOpen)
       "Text"  |> Binding.twoWayOpt((fun m -> m.Text),  SetText)
       "SelectedSuggestion"     |> Binding.twoWayOpt((fun m -> m.SelectedItem), SetSelectedItem)
       "AutoTextBoxTextChanged" |> Binding.cmd AutoTextBoxTextChanged
       "AutoListSelectionChanged" |> Binding.cmd AutoListSelectionChanged
    ]

    [<EntryPoint; STAThread>]
    let main argv =
      Program.mkSimpleWpf init update bindings
      |> Program.runWindowWithConfig
         { ElmConfig.Default with LogConsole = true }      
         (AutoCompleteComboBox.Views.MainWindow())