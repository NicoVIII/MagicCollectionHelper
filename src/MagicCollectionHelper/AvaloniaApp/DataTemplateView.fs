namespace Avalonia.FuncUI.Components

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Templates
open Avalonia.FuncUI.Library
open Avalonia.FuncUI.Types
open Avalonia.FuncUI.Components.Hosts
open Avalonia.Data
open Avalonia.Data.Core
open System.Linq.Expressions

module Observable =
    open System

    let subscribeWeakly (source: IObservable<'a>, callback: 'a -> unit, target: 'target) =
        let mutable sub: IDisposable = null
        let mutable disposed: bool = false
        let wr = WeakReference<'target>(target)

        let dispose() =
            lock (sub) (fun () ->
                if not disposed then sub.Dispose(); disposed <- true)

        let callback' x =
            let isAlive, _target = wr.TryGetTarget()
            if isAlive then callback x else dispose()

        sub <- Observable.subscribe callback' source
        sub

[<AutoOpen>]
module internal Extensions =
    open Avalonia.Interactivity
    open System
    open System.Reactive.Linq

    type IObservable<'a> with
        member this.SubscribeWeakly(callback: 'a -> unit, target) =
            Observable.subscribeWeakly(this, callback, target)

    type IInteractive with
        member this.GetObservable<'args when 'args :> RoutedEventArgs>(routedEvent: RoutedEvent<'args>) : IObservable<'args> =

            let sub = Func<IObserver<'args>, IDisposable>(fun observer ->
                // push new update to subscribers
                let handler = EventHandler<'args>(fun _ e ->
                    observer.OnNext e
                )

                // subscribe to event changes so they can be pushed to subscribers
                this.AddDisposableHandler(routedEvent, handler, routedEvent.RoutingStrategies)
            )

            Observable.Create(sub)

type NDataTemplateView<'data, 'data2, 'view when 'view :> IView>
    (viewFunc: 'data -> 'view,
     matchFunc: ('data -> bool) voption,
     itemsSource: Expression<Func<'data, 'data2 seq>> voption,
     supportsRecycling: bool) =

    member this.ViewFunc = viewFunc
    member this.MatchFunc = matchFunc
    member this.ItemsSource = itemsSource
    member this.SupportsRecycling = supportsRecycling

    override this.Equals (other: obj) : bool =
        match other with
        | :? DataTemplateView<'data, 'view> as other ->
            this.ViewFunc.GetType() = other.ViewFunc.GetType() &&
            this.MatchFunc.GetType() = other.MatchFunc.GetType() &&
            this.SupportsRecycling = other.SupportsRecycling
        | _ -> false

    override this.GetHashCode () =
        (this.ViewFunc.GetType(), this.SupportsRecycling).GetHashCode()

    interface ITreeDataTemplate with
        member this.ItemsSelector (item: obj) : InstancedBinding =
            match this.ItemsSource with
            | ValueNone -> null
            | ValueSome expression ->
                match item with
                | :? 'data as data ->
                    InstancedBinding.OneWay(ExpressionObserver.Create(data, expression), BindingPriority.Style)
                | _ -> null

        member this.Match (data: obj) : bool =
            match data, matchFunc with
            | :? 'data as data, ValueSome f -> f data
            | :? 'data, ValueNone -> true
            | _ -> false

        member this.Build (_data: obj) : IControl =
            let host = HostControl()

            let update (data: 'data) : unit =
                let view = Some (this.ViewFunc data :> IView)
                (host :> IViewHost).Update view

            host
                .GetObservable(Control.DataContextProperty)
                .SubscribeWeakly<obj>((fun data ->
                    match data with
                    | :? 'data as t -> update(t)
                    | _ -> ()
                ), this) |> ignore

            host :> IControl

type NDataTemplateView<'data> =
    /// <summary>
    /// Create a DataTemplateView for data matching type ('data)
    /// </summary>
    /// <typeparam name="'data">The Type of the data.</typeparam>
    /// <param name="viewFunc">The function that creates a view from the passed data.</param>
    static member create(itemsSelector, viewFunc: 'data -> 'view when 'view :> IView) : NDataTemplateView<'data, 'a, 'view> =
        NDataTemplateView<'data, 'a, 'view>(viewFunc = (fun a -> viewFunc a),
                                matchFunc = ValueNone,
                                itemsSource = ValueSome itemsSelector,
                                supportsRecycling = true)
