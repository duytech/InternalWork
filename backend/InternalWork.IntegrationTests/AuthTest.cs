using InternalWork.IntegrationTests.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace InternalWork.IntegrationTests
{
    public class AuthTest
    {
        [Fact]
        public async Task LoginTest()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7294");

            var json = JsonSerializer.Serialize(new { userName = "admin@company.com", password = "l6kxet@A" });
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync("/api/auth/login", data);

            result.EnsureSuccessStatusCode();
            string resultContent = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<LoginResponse>(resultContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.NotEmpty(response.Token);
        }
    }
}