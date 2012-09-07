open System
open System.IO
open Mono.Cecil

let die fmt = Printf.ksprintf (fun s -> printfn "%s" s; Environment.Exit 1; unbox 0) fmt

type Variance = Covariant | Contravariant
type TypeParameter = {
    Name: string
    Variance: Variance
}
type TypeChange = {
    Name: string
    Parameters: TypeParameter[]
}

let setVariance v (p: GenericParameter) =
    match v with
    | Covariant -> 
        p.IsCovariant <- true
        p.IsContravariant <- false
    | Contravariant ->
        p.IsCovariant <- false
        p.IsContravariant <- true

let parseTypeParameter (a: string) : TypeParameter =
    if a.Length < 2 then
        die "Invalid type parameter modifier '%s'" a
        
    let v =
        match a.[0] with
        | '-' -> Contravariant
        | '+' -> Covariant
        | x -> die "Invalid modifier '%c'" x

    { Name = a.Substring 1
      Variance = v }

let parseTypeChange (a: string) = 
    let parts = a.Split '/'
    if parts.Length < 2 then 
        die "Invalid definition '%s'" a
    { Name = parts.[0]
      Parameters = Array.map parseTypeParameter parts.[1..] }

let applyVarianceChange (t: TypeDefinition) (p: TypeParameter) =
    let pp = t.GenericParameters |> Seq.tryFind (fun x -> x.Name = p.Name) 
    match pp with
    | Some parameter -> setVariance p.Variance parameter
    | _ -> die "Type parameter '%s' not found" p.Name

let applyTypeChange (modulo: ModuleDefinition) (changes: TypeChange) = 
    let ty = modulo.GetType changes.Name
    if ty = null then
        die "Type '%s' not found" changes.Name

    if not ty.IsInterface then
        die "Type '%s' is not an interface" changes.Name
    if ty.GenericParameters.Count = 0 then
        die "Type '%s' does not have any type parameters" changes.Name
    
    Array.iter (applyVarianceChange ty) changes.Parameters

[<EntryPoint>]
let main argv = 
    if argv.Length = 0 then
        die "Missing arguments"

    let dll = argv.[0]
    if not (File.Exists dll) then
        die "No such file %s" dll

    let changes = Array.map parseTypeChange argv.[1..]
    let modulo = ModuleDefinition.ReadModule dll
    Array.iter (applyTypeChange modulo) changes
    modulo.Write dll
    0
