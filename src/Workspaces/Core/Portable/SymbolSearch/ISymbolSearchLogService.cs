﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.SymbolSearch
{
    /// <summary>
    /// Used so we can mock out logging in unit tests.
    /// </summary>
    internal interface ISymbolSearchLogService
    {
        ValueTask LogExceptionAsync(string exception, string text, CancellationToken cancellationToken);
        ValueTask LogInfoAsync(string text, CancellationToken cancellationToken);
    }
}
