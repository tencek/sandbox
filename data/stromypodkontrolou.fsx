#r @"..\packages\FSharp.Data.2.4.2\lib\net45\FSharp.Data.dll"

open System.Web
open System.Text
open FSharp.Data

fsi.ShowDeclarationValues <- false

[<Literal>]
let treesData = """{
  "markers": [
    {
      "color": "green",
      "icon_url": "/static/img/tree-markers/green.png",
      "show_crown": true,
      "object_id": 7,
      "label": "#7",
      "caption": "ol\u0161e lepkav\u00e1 (Alnus glutinosa) strom 7 na plo\u0161e JS Pod Babou",
      "spread": 1.0,
      "taxon_type": "tree-broad-leaved",
      "project_type": "trees-in-a-city",
      "position": {
        "lat": 49.228783774598597,
        "lng": 17.653325979254198
      },
      "rid": 5332279,
      "type": "tree",
      "is_private": 1
    }
  ],
  "request_id": "1476816103507"
}
"""
type Trees = JsonProvider<treesData>

let dataFile = __SOURCE_DIRECTORY__ + @"\Assets\stromypodkontrolou-2016-10-18.json"
let trees = 
    Trees.Load(dataFile).Markers
    |> Seq.cache

let westBorder = (trees |> Seq.minBy (fun marker -> marker.Position.Lng)).Position.Lng
let eastBorder = (trees |> Seq.maxBy (fun marker -> marker.Position.Lng)).Position.Lng
let northBorder = (trees |> Seq.maxBy (fun marker -> marker.Position.Lat)).Position.Lat
let southBorder = (trees |> Seq.minBy (fun marker -> marker.Position.Lat)).Position.Lat

