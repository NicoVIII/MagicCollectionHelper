namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

module ViewHelper =
    /// A node of the HungTree. It contains a key and another tree
    type HungTreeNode<'Key, 'Value> = 'Key * HungTree<'Key, 'Value>

    /// The hung tree is a tree where every path ends with a value. Therefore it is like a tree were
    /// on every branch "hangs" a value.
    and HungTree<'Key, 'Value> =
        | Nodes of HungTreeNode<'Key, 'Value> list
        | Leaf of 'Value

    module HungTree =
        let rec map mapKey mapValue tree =
            match tree with
            | Leaf value -> Leaf(mapValue value)
            | Nodes nodes ->
                nodes
                |> List.map (fun (name, child) -> mapKey name, map mapKey mapValue child)
                |> Nodes

        let inline mapKey mapKey = map mapKey id
        let inline mapValue mapValue = map id mapValue

    let label row label =
        TextBlock.create [
            TextBlock.column 0
            TextBlock.row row
            TextBlock.text label
            TextBlock.verticalAlignment VerticalAlignment.Center
        ]

module String =
    let iContains (s1: string) (s2: string) = (s1.ToLower()).Contains(s2.ToLower())
