using ClassLib2;
using Xunit;

namespace ClassLib2
{
    public class Class2Tests
    {
        [Fact]
        public void ShouldObtainGreetingWhenSomeoneMeetMsBullet()
        {
            // When
            var greeting = new Class2().Greeting();

            // Then
            Assert.Equal("Hello MsBullet!", greeting);
        }
    }
}
