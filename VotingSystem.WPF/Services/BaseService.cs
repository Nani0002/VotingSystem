using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using VotingSystem.WPF.View;

namespace VotingSystem.WPF.Services
{
    public class BaseService
    {
        protected void ShowErrorMessage(string text)
        {
            MessageWindow window = new MessageWindow(text);
            window.ShowDialog();
        }

        protected void HandleError(string responseContent)
        {
            try
            {
                var error = JsonSerializer.Deserialize<ValidationErrorResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (error?.Errors != null && error.Errors.Any())
                {
                    var errorMessages = error.Errors.SelectMany(e => e.Value).ToList();
                    string combinedMessage = string.Join("\n", errorMessages);
                    ShowErrorMessage(combinedMessage);
                    return;
                }
            }
            catch (JsonException)
            {
            }

            ShowErrorMessage(responseContent);
        }
    }

    public class ValidationErrorResponse
    {
        [JsonPropertyName("errors")] 
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}
