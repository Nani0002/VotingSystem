using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.Testing.E2E.PageModels;

namespace VotingSystem.Testing.E2E.Tests
{
    public class TopicTest : TestBase
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var loginPage = new LoginPage(Page);
            await loginPage.LoginAsync("admin@example.com", "Admin@123");
            await Page.WaitForURLAsync("**/topics");
            await Page.WaitForSelectorAsync(".card");
        }

        [Fact]
        public async Task TopicsPage_ShowsTopicCards()
        {
            var cards = await Page.QuerySelectorAllAsync(".card");
            Assert.NotEmpty(cards);
        }

        [Fact]
        public async Task AddTopicForm_CreatesNewTopic()
        {
            await Page.GotoAsync("/topic/add");

            await Page.FillAsync("#name", "Playwright Test Topic");
            await Page.FillAsync("#description", "Test topic");
            await Page.FillAsync("#minimalVotes", "1");
            await Page.FillAsync("#maximalVotes", "2");

            await Page.FillAsync("#startDate", DateTime.Today.ToString("yyyy-MM-dd"));
            await Page.FillAsync("#startDate-time", "10:00");
            await Page.FillAsync("#closeDate", DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            await Page.FillAsync("#closeDate-time", "10:00");

            await Page.ClickAsync("button[type=submit]");
            await Page.WaitForURLAsync("**/topics");
            await Page.WaitForSelectorAsync(".card");

            var cards = await Page.Locator(".card").AllInnerTextsAsync();
            foreach (var card in cards)
                Console.WriteLine($"[Card]: {card}");

            Assert.Contains(cards, c => c.Contains("Playwright Test Topic"));
        }

        [Fact]
        public async Task EditTopic_UpdatesDescription()
        {
            await Page.GotoAsync("/topic/add");

            var name = $"Editable Topic {Guid.NewGuid()}";

            await Page.FillAsync("#name", name);
            await Page.FillAsync("#description", "Initial description");
            await Page.FillAsync("#minimalVotes", "1");
            await Page.FillAsync("#maximalVotes", "2");

            await Page.FillAsync("#startDate", DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            await Page.FillAsync("#startDate-time", "10:00");
            await Page.FillAsync("#closeDate", DateTime.Today.AddDays(2).ToString("yyyy-MM-dd"));
            await Page.FillAsync("#closeDate-time", "10:00");

            await Page.ClickAsync("button[type=submit]");
            await Page.WaitForURLAsync("**/topics");
            await Page.WaitForSelectorAsync(".card");

            var card = Page.Locator($".card:has-text(\"{name}\")");
            var editButton = card.Locator("button.btn-success");
            await editButton.ClickAsync();

            await Page.FillAsync("#description", "Updated by Playwright test");

            await Page.ClickAsync("button[type=submit]");
            await Page.WaitForURLAsync("**/topics");

            var updatedCardText = await card.InnerTextAsync();
            Assert.Contains("Updated by Playwright test", updatedCardText);
        }

        [Fact]
        public async Task DeleteTopic_RemovesItFromList()
        {
            await Page.GotoAsync("/topic/add");

            var name = $"DeleteMe {Guid.NewGuid()}";

            await Page.FillAsync("#name", name);
            await Page.FillAsync("#description", "Delete this");
            await Page.FillAsync("#minimalVotes", "1");
            await Page.FillAsync("#maximalVotes", "1");

            await Page.FillAsync("#startDate", DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"));
            await Page.FillAsync("#startDate-time", "10:00");
            await Page.FillAsync("#closeDate", DateTime.Today.AddDays(2).ToString("yyyy-MM-dd"));
            await Page.FillAsync("#closeDate-time", "10:00");

            await Page.ClickAsync("button[type=submit]");
            await Page.WaitForURLAsync("**/topics");
            await Page.WaitForSelectorAsync(".card");

            var card = Page.Locator($".card:has-text(\"{name}\")");
            Assert.True(await card.IsVisibleAsync(), "New topic not visible");

            var deleteButton = card.Locator("button.btn-danger");
            await deleteButton.ClickAsync();

            await Page.WaitForTimeoutAsync(1000);

            var exists = await card.IsVisibleAsync();

            Assert.False(exists, "Topic was not deleted");
        }
    }
}
