﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;

#if CODE_STYLE
using TOption = Microsoft.CodeAnalysis.Options.IOption2;
#else
using TOption = Microsoft.CodeAnalysis.Options.IOption;
#endif

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal readonly record struct IdeAnalyzerOptions(
        bool FadeOutUnusedImports,
        bool FadeOutUnreachableCode,
        bool ReportInvalidPlaceholdersInStringDotFormatCalls,
        bool ReportInvalidRegexPatterns)
    {
        public static readonly IdeAnalyzerOptions CodeStyleDefault = new(
            FadeOutUnusedImports: false,
            FadeOutUnreachableCode: false,
            ReportInvalidPlaceholdersInStringDotFormatCalls: true,
            ReportInvalidRegexPatterns: true);
    }

    internal static partial class AnalyzerHelper
    {
        public static IdeAnalyzerOptions GetIdeOptions(this SyntaxTreeAnalysisContext context)
#if CODE_STYLE
            => IdeAnalyzerOptions.CodeStyleDefault;
#else
            => (context.Options is WorkspaceAnalyzerOptions workspaceOptions) ? workspaceOptions.GetIdeOptions(context.Tree.Options.Language) : IdeAnalyzerOptions.CodeStyleDefault;
#endif

        public static IdeAnalyzerOptions GetIdeOptions(this OperationAnalysisContext context)
#if CODE_STYLE
            => IdeAnalyzerOptions.CodeStyleDefault;
#else
            => (context.Options is WorkspaceAnalyzerOptions workspaceOptions) ? workspaceOptions.GetIdeOptions(context.Operation.Language) : IdeAnalyzerOptions.CodeStyleDefault;
#endif

        public static IdeAnalyzerOptions GetIdeOptions(this SyntaxNodeAnalysisContext context)
#if CODE_STYLE
            => IdeAnalyzerOptions.CodeStyleDefault;
#else
            => (context.Options is WorkspaceAnalyzerOptions workspaceOptions) ? workspaceOptions.GetIdeOptions(context.Node.Language) : IdeAnalyzerOptions.CodeStyleDefault;
#endif

        public static IdeAnalyzerOptions GetIdeOptions(this SemanticModelAnalysisContext context)
#if CODE_STYLE
            => IdeAnalyzerOptions.CodeStyleDefault;
#else
            => (context.Options is WorkspaceAnalyzerOptions workspaceOptions) ? workspaceOptions.GetIdeOptions(context.SemanticModel.Language) : IdeAnalyzerOptions.CodeStyleDefault;
#endif

        public static T GetOption<T>(this SemanticModelAnalysisContext context, Option2<T> option)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.SemanticModel.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this SyntaxNodeAnalysisContext context, Option2<T> option)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Node.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this SyntaxTreeAnalysisContext context, Option2<T> option)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Tree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this OperationAnalysisContext context, Option2<T> option)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Operation.Syntax.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this SemanticModelAnalysisContext context, PerLanguageOption2<T> option, string? language)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.SemanticModel.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, language, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this SyntaxNodeAnalysisContext context, PerLanguageOption2<T> option, string? language)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Node.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, language, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this SyntaxTreeAnalysisContext context, PerLanguageOption2<T> option, string? language)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Tree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, language, syntaxTree, cancellationToken);
        }

        public static T GetOption<T>(this OperationAnalysisContext context, PerLanguageOption2<T> option, string? language)
        {
            var analyzerOptions = context.Options;
            var syntaxTree = context.Operation.Syntax.SyntaxTree;
            var cancellationToken = context.CancellationToken;

            return GetOption(analyzerOptions, option, language, syntaxTree, cancellationToken);
        }

        public static bool TryGetEditorConfigOption<T>(this AnalyzerOptions analyzerOptions, TOption option, SyntaxTree syntaxTree, [MaybeNullWhen(false)] out T value)
        {
            var configOptions = analyzerOptions.AnalyzerConfigOptionsProvider.GetOptions(syntaxTree);
            return configOptions.TryGetEditorConfigOption(option, out value);
        }
    }
}
