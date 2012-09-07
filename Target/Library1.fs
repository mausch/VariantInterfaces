namespace Target

type IVariant<'OutR, 'InA> =
    abstract GetSomething: unit -> 'OutR
    abstract SetSomething: 'InA -> unit
    abstract GetSetSomething: 'InA -> 'OutR