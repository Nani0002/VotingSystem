using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using VotingSystem.Blazor.WebAssembly.Components;
using VotingSystem.Blazor.WebAssembly.Services;
using VotingSystem.Blazor.WebAssembly.ViewModels;

namespace VotingSystem.Testing.ComponentTests
{
    public class TopicsTests : IDisposable
    {
        private readonly TestContext _context = new();


        public TopicsTests()
        {

        }


        public void Dispose() => _context.Dispose();

        [Fact]
        public void DateTimeInput_SetsDateAndTimeCorrectly()
        {
            var ctx = new TestContext();
            var dateTime = new DateTime(2024, 5, 15, 12, 30, 0);
            DateTime? updatedValue = null;

            var component = ctx.RenderComponent<DateTimeInput>(parameters => parameters
                .Add(p => p.Id, "test-date")
                .Add(p => p.Text, "Test Date")
                .Add(p => p.Value, dateTime)
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<DateTime?>(this, val => updatedValue = val))
            );

            // Simulate changing the time to 14:00
            component.Find("input[type='time']").Change("14:00");

            Assert.Equal(new DateTime(2024, 5, 15, 14, 0, 0), updatedValue);
        }

        [Fact]
        public async Task Select2Input_UpdatesValueOnJSCall()
        {
            var ctx = new TestContext();
            var jsMock = new Mock<IJSRuntime>();
            ctx.Services.AddSingleton(jsMock.Object);

            var valueChangedCalled = false;
            List<ChoiceViewModel> newChoices = [];

            var component = ctx.RenderComponent<Select2Input>(parameters => parameters
                .Add(p => p.Text, "Select options")
                .Add(p => p.ValueChanged, EventCallback.Factory.Create<List<ChoiceViewModel>>(this, val => {
                    valueChangedCalled = true;
                    newChoices = val;
                }))
            );

            await component.Instance.OnSelect2Changed(new() { "Option A", "Option B" });

            Assert.True(valueChangedCalled);
            Assert.Equal(2, newChoices.Count);
            Assert.Contains(newChoices, c => c.Value == "Option A");
        }
    }
}
