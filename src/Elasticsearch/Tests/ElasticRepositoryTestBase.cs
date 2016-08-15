﻿using System;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Logging;
using Foundatio.Logging.Xunit;
using Foundatio.Repositories.Elasticsearch.Configuration;
using Foundatio.Utility;
using Nest;
using Xunit.Abstractions;
using LogLevel = Foundatio.Logging.LogLevel;

namespace Foundatio.Repositories.Elasticsearch.Tests {
    public abstract class ElasticRepositoryTestBase : TestWithLoggingBase {
        protected readonly InMemoryCacheClient _cache;
        protected readonly IElasticConfiguration _configuration;
        protected readonly IElasticClient _client;

        public ElasticRepositoryTestBase(ITestOutputHelper output) : base(output) {
            SystemClock.Reset();
            Log.MinimumLevel = LogLevel.Trace;
            Log.SetLogLevel<ScheduledTimer>(LogLevel.Warning);

            _cache = new InMemoryCacheClient(Log);
            _configuration = GetElasticConfiguration();
            _client = _configuration.Client;
        }

        protected abstract IElasticConfiguration GetElasticConfiguration();

        protected virtual async Task RemoveDataAsync(bool configureIndexes = true) {
            var minimumLevel = Log.MinimumLevel;
            Log.MinimumLevel = LogLevel.Error;

            await _cache.RemoveAllAsync();

            _configuration.DeleteIndexes();
            if (configureIndexes)
                _configuration.ConfigureIndexes();
            
            await _client.RefreshAsync();

            Log.MinimumLevel = minimumLevel;
        }
    }
}