# Introduction

A little command-line utility to add variance to interfaces in a .NET assembly.

This is especially useful for F# since it can't define variance yet.

# Example

Given an assembly Target.dll with


    namespace Coco
    
    type IVariant<'OutR, 'InA> =
        abstract GetSomething: unit -> 'OutR
        abstract SetSomething: 'InA -> unit
        abstract GetSetSomething: 'InA -> 'OutR
        
You can make `IVariant` covariant in `OutR` and contravariant in `InR` by running:

    VariantInterfaces.exe Target.dll Coco.IVariant`2/+OutR/-InA
    
This overwrites the original DLL.

# Binaries

[Available on Nuget](http://nuget.org/packages/VariantInterfaces).