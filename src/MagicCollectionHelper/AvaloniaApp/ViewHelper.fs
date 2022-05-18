namespace MagicCollectionHelper.AvaloniaApp

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.Layout

open MagicCollectionHelper.Core

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
            let map = map mapKey mapValue

            match tree with
            | Leaf value -> Leaf(mapValue value)
            | Nodes nodes ->
                nodes
                |> List.map (fun (name, child) -> mapKey name, map child)
                |> Nodes

        let inline mapKey mapKey = map mapKey id
        let inline mapValue mapValue = map id mapValue

        let inline sumBy summer tree =
            let rec sumBy summer tree =
                match tree with
                | Nodes nodes -> List.sumBy (snd >> sumBy summer) nodes
                | Leaf value -> summer value

            sumBy summer tree

    // Specific version of HungTree, which we use for our expander structure
    module ExpanderTree =
        let sumUpCards search =
            HungTree.sumBy (
                List.filter (fun oldableEntry -> Search.fits search oldableEntry)
                >> List.sumBy (fun entry ->
                    let amount = entry ^. AgedEntryWithInfoOptic.amount

                    match search.old with
                    | Some false ->
                        let amountOld = entry ^. AgedEntryWithInfoOptic.amountOld
                        amount - amountOld
                    | _ -> amount)
            )

    let label row label =
        TextBlock.create [
            TextBlock.column 0
            TextBlock.row row
            TextBlock.text label
            TextBlock.verticalAlignment VerticalAlignment.Center
        ]
