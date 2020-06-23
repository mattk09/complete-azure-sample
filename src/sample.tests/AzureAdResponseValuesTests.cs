using System.Text.Json;
using Sample.Extensions.Models;
using Xunit;

namespace Sample.Tests
{
    public class AzureAdResponseValuesTests
    {
        [Theory]
        [InlineData("id_token=token&state=state_value&state_session=guid", 3)]
        [InlineData("key1=1&key2=2&key3=3&key4=4", 4)]
        [InlineData("key1=1", 1)]
        public void SuccessfullyParseFormData(string str, int size)
        {
            var adRespValues = new AzureAdResponseValues(str);
            Assert.Equal(adRespValues.Count(), size);
        }

        [Theory]
        [InlineData("id_token=token#state=state_value#state_session=guid", 1)]
        [InlineData("id_token=token,state=state_value", 1)]
        [InlineData("id_token=token", 1)]
        public void FailParsingStringWithUnknowSepartor(string str, int size)
        {
            var adRespValues = new AzureAdResponseValues(str);
            Assert.Equal(adRespValues.Count(), size);
        }

        [Theory]
        [InlineData("id_token=token&state=state_value&state_session=guid")]
        [InlineData("key1=1&key2=2&key3=3&key4=4")]
        [InlineData("key1=1")]
        public void ValidJsonReturned(string str)
        {
            var adRespValues = new AzureAdResponseValues(str);
            var jsonStr = adRespValues.ToJson();
            var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(jsonStr));
            Assert.Null(exception);
        }
    }
}
