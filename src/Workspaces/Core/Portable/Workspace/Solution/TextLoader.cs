﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// A class that represents access to a source text and its version from a storage location.
    /// </summary>
    public abstract class TextLoader
    {
        private const double MaxDelaySecs = 1.0;
        private const int MaxRetries = 5;
        internal static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(MaxDelaySecs / MaxRetries);

        internal virtual string? FilePath => null;

        /// <summary>
        /// Load a text and a version of the document.
        /// </summary>
        /// <exception cref="IOException" />
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="OperationCanceledException"/>
        public virtual Task<TextAndVersion> LoadTextAndVersionAsync(CancellationToken cancellationToken)
#pragma warning disable CS0618 // Type or member is obsolete
            => LoadTextAndVersionAsync(workspace: null, documentId: null, cancellationToken);
#pragma warning restore CS0618

        /// <summary>
        /// Load a text and a version of the document.
        /// </summary>
        /// <param name="workspace">Obsolete. Null.</param>
        /// <param name="documentId">Obsolete. Null.</param>
        /// <exception cref="IOException" />
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="OperationCanceledException"/>
        [Obsolete("Use LoadTextAndVersionAsync(CancellationToken) instead")]
        public virtual Task<TextAndVersion> LoadTextAndVersionAsync(Workspace? workspace, DocumentId? documentId, CancellationToken cancellationToken)
            => throw new NotImplementedException("The API is obsolete, call LoadTextAndVersionAsync(CancellationToken) instead");

        /// <summary>
        /// Load a text and a version of the document in the workspace.
        /// </summary>
        /// <exception cref="IOException" />
        /// <exception cref="InvalidDataException"/>
        /// <exception cref="OperationCanceledException"/>
        internal virtual TextAndVersion LoadTextAndVersionSynchronously(CancellationToken cancellationToken)
        {
            // this implementation exists in case a custom derived type does not have access to internals
            return LoadTextAndVersionAsync(cancellationToken).WaitAndGetResult_CanCallOnBackground(cancellationToken);
        }

        internal async Task<TextAndVersion> LoadTextAsync(CancellationToken cancellationToken)
        {
            var retries = 0;

            while (true)
            {
                try
                {
                    return await LoadTextAndVersionAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                }
                catch (IOException e)
                {
                    if (++retries > MaxRetries)
                    {
                        return CreateFailedText(e.Message);
                    }

                    // fall out to try again
                }
                catch (InvalidDataException e)
                {
                    return CreateFailedText(e.Message);
                }

                // try again after a delay
                await Task.Delay(RetryDelay, cancellationToken).ConfigureAwait(false);
            }
        }

        internal TextAndVersion LoadTextSynchronously(CancellationToken cancellationToken)
        {
            var retries = 0;

            while (true)
            {
                try
                {
                    return LoadTextAndVersionSynchronously(cancellationToken);
                }
                catch (IOException e)
                {
                    if (++retries > MaxRetries)
                    {
                        return CreateFailedText(e.Message);
                    }

                    // fall out to try again
                }
                catch (InvalidDataException e)
                {
                    return CreateFailedText(e.Message);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // try again after a delay
                Thread.Sleep(RetryDelay);
            }
        }

        private TextAndVersion CreateFailedText(string message)
        {
            Location location;
            string display;

            var filePath = FilePath;

            if (filePath == null)
            {
                location = Location.None;
                display = "<no path>";
            }
            else
            {
                location = Location.Create(filePath, textSpan: default, lineSpan: default);
                display = filePath;
            }

            return TextAndVersion.Create(
                SourceText.From(string.Empty, Encoding.UTF8),
                VersionStamp.Default,
                Diagnostic.Create(WorkspaceDiagnosticDescriptors.ErrorReadingFileContent, location, new[] { display, message }));
        }

        /// <summary>
        /// Creates a new <see cref="TextLoader"/> from an already existing source text and version.
        /// </summary>
        public static TextLoader From(TextAndVersion textAndVersion)
        {
            if (textAndVersion == null)
            {
                throw new ArgumentNullException(nameof(textAndVersion));
            }

            return new TextDocumentLoader(textAndVersion);
        }

        /// <summary>
        /// Creates a <see cref="TextLoader"/> from a <see cref="SourceTextContainer"/> and version. 
        /// 
        /// The text obtained from the loader will be the current text of the container at the time
        /// the loader is accessed.
        /// </summary>
        public static TextLoader From(SourceTextContainer container, VersionStamp version, string? filePath = null)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return new TextContainerLoader(container, version, filePath);
        }

        private sealed class TextDocumentLoader : TextLoader
        {
            private readonly TextAndVersion _textAndVersion;

            internal TextDocumentLoader(TextAndVersion textAndVersion)
                => _textAndVersion = textAndVersion;

            public override Task<TextAndVersion> LoadTextAndVersionAsync(CancellationToken cancellationToken)
                => Task.FromResult(_textAndVersion);

            internal override TextAndVersion LoadTextAndVersionSynchronously(CancellationToken cancellationToken)
                => _textAndVersion;
        }

        private sealed class TextContainerLoader : TextLoader
        {
            private readonly SourceTextContainer _container;
            private readonly VersionStamp _version;
            private readonly string? _filePath;

            internal TextContainerLoader(SourceTextContainer container, VersionStamp version, string? filePath)
            {
                _container = container;
                _version = version;
                _filePath = filePath;
            }

            public override Task<TextAndVersion> LoadTextAndVersionAsync(CancellationToken cancellationToken)
                => Task.FromResult(LoadTextAndVersionSynchronously(cancellationToken));

            internal override TextAndVersion LoadTextAndVersionSynchronously(CancellationToken cancellationToken)
                => TextAndVersion.Create(_container.CurrentText, _version, _filePath);
        }
    }
}
