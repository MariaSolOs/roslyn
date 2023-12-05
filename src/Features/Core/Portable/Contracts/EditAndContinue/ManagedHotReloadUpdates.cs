﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace Microsoft.CodeAnalysis.Contracts.EditAndContinue;

[DataContract]
internal readonly struct ManagedHotReloadUpdates
{
    [DataMember(Name = "updates")]
    public ImmutableArray<ManagedHotReloadUpdate> Updates { get; }

    [DataMember(Name = "diagnostics")]
    public ImmutableArray<ManagedHotReloadDiagnostic> Diagnostics { get; }

    public ManagedHotReloadUpdates(ImmutableArray<ManagedHotReloadUpdate> updates, ImmutableArray<ManagedHotReloadDiagnostic> diagnostics)
    {
        Updates = updates;
        Diagnostics = diagnostics;
    }
}
