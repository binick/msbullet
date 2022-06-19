using ClassLib1;
using Xunit;

namespace ClassLib1
{
    public class Class1Tests
    {
        [Fact]
        public void ShouldObtainGreetingWhenSomeoneMeetMsBullet()
        {
            // When
            var greeting = new Class1().Greeting();

            // Then
            Assert.Equal("Hello MsBullet!", greeting);
        }
    }
}
