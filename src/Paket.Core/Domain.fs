﻿module Paket.Domain

/// Represents a NuGet package name
type PackageName =
    private | PackageName of string
    override this.ToString() = 
        match this with
        | PackageName.PackageName name -> name
         
/// Active recognizer to convert a NuGet package name into a string
let (|PackageName|) (PackageName.PackageName name) = name

/// Function to convert a string into a NuGet package name
let PackageName name = PackageName.PackageName name

/// Represents a normalized NuGet package name
type NormalizedPackageName =
    private | NormalizedPackageName of string
    
    override this.ToString() = 
        match this with
        | NormalizedPackageName.NormalizedPackageName name -> name

/// Active recognizer to convert a NuGet package name into a normalized one
let (|NormalizedPackageName|) (PackageName name) =
    NormalizedPackageName.NormalizedPackageName(name.ToLowerInvariant())

/// Function to convert a NuGet package name into a normalized one
let NormalizedPackageName = (|NormalizedPackageName|)
