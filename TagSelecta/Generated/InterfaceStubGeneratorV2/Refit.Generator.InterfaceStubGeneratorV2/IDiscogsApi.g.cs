#nullable enable
#pragma warning disable
namespace Refit.Implementation
{

    partial class Generated
    {

    /// <inheritdoc />
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::TagSelectaRefitInternalGenerated.PreserveAttribute]
    [global::System.Reflection.Obfuscation(Exclude=true)]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    partial class TagSelectaDiscogsIDiscogsApi
        : global::TagSelecta.Discogs.IDiscogsApi

    {
        /// <inheritdoc />
        public global::System.Net.Http.HttpClient Client { get; }
        readonly global::Refit.IRequestBuilder requestBuilder;

        /// <inheritdoc />
        public TagSelectaDiscogsIDiscogsApi(global::System.Net.Http.HttpClient client, global::Refit.IRequestBuilder requestBuilder)
        {
            Client = client;
            this.requestBuilder = requestBuilder;
        }


        private static readonly global::System.Type[] ______typeParameters = new global::System.Type[] {typeof(int) };

        /// <inheritdoc />
        public async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release> GetRelease(int @id)
        {
            var ______arguments = new object[] { @id };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("GetRelease", ______typeParameters );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }

        private static readonly global::System.Type[] ______typeParameters0 = new global::System.Type[] {typeof(int) };

        /// <inheritdoc />
        public async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release> GetMaster(int @id)
        {
            var ______arguments = new object[] { @id };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("GetMaster", ______typeParameters0 );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }

        private static readonly global::System.Type[] ______typeParameters1 = new global::System.Type[] {typeof(string) };

        /// <inheritdoc />
        public async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.SearchResult> Search(string @q)
        {
            var ______arguments = new object[] { @q };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("Search", ______typeParameters1 );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.SearchResult>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }

        private static readonly global::System.Type[] ______typeParameters2 = new global::System.Type[] {typeof(int) };

        /// <inheritdoc />
        async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release> global::TagSelecta.Discogs.IDiscogsApi.GetRelease(int @id)
        {
            var ______arguments = new object[] { @id };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("GetRelease", ______typeParameters2 );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }

        private static readonly global::System.Type[] ______typeParameters3 = new global::System.Type[] {typeof(int) };

        /// <inheritdoc />
        async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release> global::TagSelecta.Discogs.IDiscogsApi.GetMaster(int @id)
        {
            var ______arguments = new object[] { @id };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("GetMaster", ______typeParameters3 );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.Release>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }

        private static readonly global::System.Type[] ______typeParameters4 = new global::System.Type[] {typeof(string) };

        /// <inheritdoc />
        async global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.SearchResult> global::TagSelecta.Discogs.IDiscogsApi.Search(string @q)
        {
            var ______arguments = new object[] { @q };
            var ______func = requestBuilder.BuildRestResultFuncForMethod("Search", ______typeParameters4 );

            return await ((global::System.Threading.Tasks.Task<global::TagSelecta.Discogs.SearchResult>)______func(this.Client, ______arguments)).ConfigureAwait(false);
        }
    }
    }
}

#pragma warning restore
