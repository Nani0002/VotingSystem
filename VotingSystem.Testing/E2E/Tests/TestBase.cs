﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace VotingSystem.Testing.E2E.Tests
{
    public abstract class TestBase : PageTest
    {
        private const string FrontendBaseUrl = "https://localhost:7137";
        private const string BackendBaseUrl = "https://localhost:7183/";

        public override BrowserNewContextOptions ContextOptions()
        {
            return new BrowserNewContextOptions()
            {
                ViewportSize = new()
                {
                    Width = 1920,
                    Height = 1080
                },
                BaseURL = FrontendBaseUrl,
                RecordVideoDir = "videos",
                RecordVideoSize = new RecordVideoSize { Width = 640, Height = 480 }
            };
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // Reset database before each test
            using var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BackendBaseUrl),
            };
            await httpClient.PostAsync("/e2e-test/reset-database", null);
        }
    }
}
