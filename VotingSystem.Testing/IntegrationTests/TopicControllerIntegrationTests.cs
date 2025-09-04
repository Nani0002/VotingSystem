using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using VotingSystem.DataAccess;
using VotingSystem.DataAccess.Models;
using VotingSystem.Shared.Models;
using VotingSystem.WebAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VotingSystem.Testing.IntegrationTests
{
    public class TopicControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        private static readonly LoginRequestDto AdminLogin = new()
        {
            Email = "admin@example.com",
            Password = "Admin@123"
        };

        private static readonly LoginRequestDto UserLogin = new()
        {
            Email = "user@example.com",
            Password = "User@123"
        };

        public TopicControllerIntegrationTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<VotingSystemDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<VotingSystemDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestMoviesDatabase");
                    });

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<VotingSystemDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();


                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    SeedUsers(userManager);

                    SeedTopics(db, userManager);
                });
            });

            _client = _factory.CreateClient();
        }

        #region Get

        [Fact]
        public async Task GetAllTopics_ReturnsAllTopics()
        {
            // Act
            var response = await _client.GetAsync("/topics/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var topics = await response.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            Assert.NotNull(topics);
            Assert.True(topics.Count >= 2);
        }

        [Fact]
        public async Task GetTopicById_ReturnsCorrectTopic()
        {
            // Arrange
            var allResponse = await _client.GetAsync("/topics/all");
            var allTopics = await allResponse.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topicId = allTopics!.First().Id;

            // Act
            var response = await _client.GetAsync($"/topics/{topicId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var topic = await response.Content.ReadFromJsonAsync<TopicResponseDto>();
            Assert.Equal(topicId, topic!.Id);
        }

        [Fact]
        public async Task GetUserTopics_ReturnsOnlyOwnedTopics()
        {
            // Arrange
            await Login(AdminLogin);

            // Act
            var response = await _client.GetAsync("/topics");

            // Assert
            response.EnsureSuccessStatusCode();
            var topics = await response.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            Assert.NotNull(topics);
            Assert.All(topics!, t => Assert.Equal(AdminLogin.Email, GetTopicOwnerEmail(t.Id)));
        }

        #endregion

        #region Create

        [Fact]
        public async Task CreateTopic_AsAuthenticatedUser_ReturnsCreatedTopic()
        {
            // Arrange
            await Login(AdminLogin);
            var newTopic = new TopicRequestDto
            {
                Name = "Új téma",
                Description = "Ez egy teszt téma",
                StartDate = DateTime.UtcNow.AddMinutes(5),
                CloseDate = DateTime.UtcNow.AddHours(1),
                MinimalVotes = 1,
                MaximalVotes = 2,
                Choices = new List<string> { "A", "B", "C" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/topics", newTopic);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<TopicResponseDto>();
            Assert.Equal("Új téma", created!.Name);
        }

        [Fact]
        public async Task VoteOnTopic_ValidVote_ReturnsCreated()
        {
            // Arrange
            await Login(UserLogin);

            var allTopicsResp = await _client.GetAsync("/topics/all");
            var topics = await allTopicsResp.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topic = topics!.First(t => t.Choices.Count >= 2);

            var voteRequest = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = new List<string> { topic.Choices[0] }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/topics/vote", voteRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task VoteOnTopic_InvalidChoice_ReturnsBadRequest()
        {
            // Arrange
            await Login(UserLogin);

            var allTopicsResp = await _client.GetAsync("/topics/all");
            var topics = await allTopicsResp.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topic = topics!.First();

            var voteRequest = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = new List<string> { "not-a-valid-choice" }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/topics/vote", voteRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task VoteOnTopic_TwiceBySameUser_ReturnsConflictOrBadRequest()
        {
            // Arrange
            await Login(UserLogin);

            var allTopicsResp = await _client.GetAsync("/topics/all");
            var topics = await allTopicsResp.Content.ReadFromJsonAsync<List<TopicResponseDto>>();

            // Pick a topic that allows only one vote
            var topic = topics!
                .First(t => t.MinimalVotes == 1 && t.MaximalVotes == 1 && t.CloseDate > DateTime.UtcNow);

            var voteRequest = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = new List<string> { topic.Choices.First() }
            };

            // Act - First vote should succeed
            var firstResponse = await _client.PostAsJsonAsync("/topics/vote", voteRequest);
            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

            // Act - Second vote should fail
            var secondResponse = await _client.PostAsJsonAsync("/topics/vote", voteRequest);

            // Assert - Second vote is rejected
            Assert.True(
                secondResponse.StatusCode == HttpStatusCode.Conflict ||
                secondResponse.StatusCode == HttpStatusCode.BadRequest,
                $"Expected Conflict or BadRequest, got {secondResponse.StatusCode}"
            );
        }

        [Fact]
        public async Task VoteOnTopic_InvalidChoiceCount_ReturnsBadRequest()
        {
            // Arrange
            await Login(UserLogin);

            var allTopicsResp = await _client.GetAsync("/topics/all");
            var topics = await allTopicsResp.Content.ReadFromJsonAsync<List<TopicResponseDto>>();

            // Choose a topic with min 1 and max 2 votes
            var topic = topics!
                .First(t => t.MinimalVotes == 1 && t.MaximalVotes == 2 && t.Choices.Count >= 3 && t.CloseDate > DateTime.UtcNow);

            // Too few (0 choices)
            var tooFew = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = new List<string>() // No selection
            };

            // Too many (3+ choices)
            var tooMany = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = topic.Choices.Take(3).ToList() // Exceeds MaxVotes of 2
            };

            // Act - Too few
            var responseFew = await _client.PostAsJsonAsync("/topics/vote", tooFew);
            Assert.Equal(HttpStatusCode.BadRequest, responseFew.StatusCode);

            // Act - Too many
            var responseMany = await _client.PostAsJsonAsync("/topics/vote", tooMany);
            Assert.Equal(HttpStatusCode.BadRequest, responseMany.StatusCode);
        }

        [Fact]
        public async Task VoteOnTopic_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            var allTopicsResp = await _client.GetAsync("/topics/all");
            var topics = await allTopicsResp.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topic = topics!.First();

            var voteRequest = new VoteRequestDto
            {
                TopicId = topic.Id,
                SelectedChoices = new List<string> { topic.Choices.First() }
            };

            // Clear token if any
            _client.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _client.PostAsJsonAsync("/topics/vote", voteRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Update

        [Fact]
        public async Task UpdateTopic_AsOwner_UpdatesCloseDate()
        {
            // Arrange
            await Login(AdminLogin);

            var allResponse = await _client.GetAsync("/topics/all");
            var topics = await allResponse.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topic = topics!.First();

            var updateRequest = new TopicRequestDto
            {
                Name = topic.Name,
                Description = topic.Description,
                StartDate = topic.StartDate,
                CloseDate = topic.CloseDate.AddMinutes(10),
                MinimalVotes = topic.MinimalVotes,
                MaximalVotes = topic.MaximalVotes,
                Choices = topic.Choices,
                Live = topic.Live
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/topics/{topic.Id}", updateRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var updated = await response.Content.ReadFromJsonAsync<TopicResponseDto>();
            Assert.Equal(updateRequest.CloseDate, updated!.CloseDate);
        }

        [Fact]
        public async Task UpdateTopic_AsNonOwner_ReturnsForbidden()
        {
            // Arrange
            await Login(AdminLogin);

            var allResponse = await _client.GetAsync("/topics/all");
            var topics = await allResponse.Content.ReadFromJsonAsync<List<TopicResponseDto>>();
            var topic = topics!.First();

            await Login(UserLogin); // switch to non-owner

            var updateRequest = new TopicRequestDto
            {
                Name = topic.Name,
                Description = topic.Description,
                StartDate = topic.StartDate,
                CloseDate = topic.CloseDate.AddMinutes(10),
                MinimalVotes = topic.MinimalVotes,
                MaximalVotes = topic.MaximalVotes,
                Choices = topic.Choices,
                Live = topic.Live
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/topics/{topic.Id}", updateRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteTopic_AsOwner_DeletesSuccessfully()
        {
            // Arrange
            await Login(AdminLogin);

            var newTopic = new TopicRequestDto
            {
                Name = "Törlendő téma",
                Description = "Csak teszt",
                StartDate = DateTime.UtcNow.AddMinutes(5),
                CloseDate = DateTime.UtcNow.AddHours(1),
                MinimalVotes = 1,
                MaximalVotes = 1,
                Choices = new List<string> { "igen", "nem" }
            };

            var createResponse = await _client.PostAsJsonAsync("/topics", newTopic);
            var createdTopic = await createResponse.Content.ReadFromJsonAsync<TopicResponseDto>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/topics/{createdTopic!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Verify it's really gone
            var getResponse = await _client.GetAsync($"/topics/{createdTopic.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteTopic_AsNonOwner_ReturnsForbidden()
        {
            // Arrange
            await Login(AdminLogin);
            var newTopic = new TopicRequestDto
            {
                Name = "Téma törléshez",
                Description = "Csak teszt",
                StartDate = DateTime.UtcNow.AddMinutes(5),
                CloseDate = DateTime.UtcNow.AddHours(1),
                MinimalVotes = 1,
                MaximalVotes = 1,
                Choices = new List<string> { "igen", "nem" }
            };

            var createResponse = await _client.PostAsJsonAsync("/topics", newTopic);
            var createdTopic = await createResponse.Content.ReadFromJsonAsync<TopicResponseDto>();

            await Login(UserLogin); // switch to non-owner

            // Act
            var deleteResponse = await _client.DeleteAsync($"/topics/{createdTopic!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
        }

        #endregion

        #region Helpers

        private void SeedTopics(VotingSystemDbContext context, UserManager<User> userManager)
        {
            context.Topics.AddRange(
                new Topic
                {
                    Name = "Menjünk haza?",
                    Description = "Menjünk-e haza most, vagy várjunk még?",
                    StartDate = DateTime.Now.AddMinutes(-5),
                    CloseDate = DateTime.Now.AddMinutes(20),
                    MinimalVotes = 1,
                    MaximalVotes = 1,
                    OwnerId = userManager.FindByEmailAsync(AdminLogin.Email).Result!.Id,
                    Choices =
                    [
                        new Choice { Value = "igen" },
                        new Choice { Value = "nem" }
                    ]
                },
                new Topic
                {
                    Name = "Milyen filmet nézzek meg?",
                    Description = "-",
                    StartDate = DateTime.Now.AddMinutes(-5),
                    CloseDate = DateTime.Now.AddHours(12),
                    MinimalVotes = 1,
                    MaximalVotes = 2,
                    OwnerId = userManager.FindByEmailAsync(AdminLogin.Email).Result!.Id,
                    Choices =
                    [
                        new Choice { Value = "Film1" },
                        new Choice { Value = "Film2" },
                        new Choice { Value = "Film3" },
                        new Choice { Value = "Film4" }
                    ]
                }
            );

            context.SaveChanges();
        }

        private static void SeedUsers(UserManager<User> userManager)
        {
            var adminUser = userManager.FindByEmailAsync(AdminLogin.Email).Result;
            if (adminUser == null)
            {
                adminUser = new User { UserName = AdminLogin.Email, Email = AdminLogin.Email, Name = "Test Admin" };
                userManager.CreateAsync(adminUser, AdminLogin.Password).Wait();
            }

            var user = userManager.FindByEmailAsync(UserLogin.Email).Result;
            if (user == null)
            {
                user = new User { UserName = UserLogin.Email, Email = UserLogin.Email, Name = "Test User" };
                userManager.CreateAsync(user, UserLogin.Password).Wait();
            }
        }

        public void Dispose()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<VotingSystemDbContext>();
            db.Database.EnsureDeleted();

            _factory.Dispose();
            _client.Dispose();
        }

        private async Task Login(LoginRequestDto loginRequest)
        {
            var response = await _client.PostAsJsonAsync("users/login", loginRequest);
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse?.AuthToken);
        }

        private string GetTopicOwnerEmail(int topicId)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VotingSystemDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var topic = db.Topics.FirstOrDefault(t => t.Id == topicId);
            if (topic == null) throw new Exception("Topic not found");

            var user = userManager.FindByIdAsync(topic.OwnerId).Result;
            return user?.Email ?? throw new Exception("Owner user not found");
        }

        #endregion
    }
}
