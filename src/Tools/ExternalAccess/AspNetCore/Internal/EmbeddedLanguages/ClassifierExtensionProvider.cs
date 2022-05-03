﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.ExternalAccess.AspNetCore.EmbeddedLanguages;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.ExternalAccess.AspNetCore.Internal.EmbeddedLanguages
{
    internal sealed class AspNetCoreClassifierExtensionProvider
        : AbstractProjectExtensionProvider<
            AspNetCoreClassifierExtensionProvider,
            IAspNetCoreEmbeddedLanguageClassifier,
            ExportAspNetCoreEmbeddedLanguageClassifierAttribute>
    {
        protected override bool SupportsLanguage(ExportAspNetCoreEmbeddedLanguageClassifierAttribute exportAttribute, string language)
        {
            return exportAttribute.Language == null
                || exportAttribute.Language.Length == 0
                || exportAttribute.Language.Contains(language);
        }

        protected override bool TryGetExtensionsFromReference(AnalyzerReference reference, out ImmutableArray<IAspNetCoreEmbeddedLanguageClassifier> extensions)
        {
            extensions = default;
            return false;
        }
    }
}
