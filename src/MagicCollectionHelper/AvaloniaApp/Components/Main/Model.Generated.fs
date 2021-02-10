//------------------------------------------------------------------------------
//        This code was generated by myriad.
//        Changes to this file will be lost when the code is regenerated.
//------------------------------------------------------------------------------
namespace rec MagicCollectionHelper.AvaloniaApp.Components.Main

module StateLenses =
    open MagicCollectionHelper.AvaloniaApp.Components.Main
    let cards =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.cards),
            (fun (x: State) (value: MagicCollectionHelper.Core.Types.CardEntry list) -> { x with cards = value })
        )

    let setData =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.setData),
            (fun (x: State) (value: MagicCollectionHelper.Core.Types.SetDataMap) -> { x with setData = value })
        )

    let text =
        MagicCollectionHelper.Core.Types.Lens(
            (fun (x: State) -> x.text),
            (fun (x: State) (value: string) -> { x with text = value })
        )
