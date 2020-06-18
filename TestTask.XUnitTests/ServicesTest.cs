using TestTask.Services;
using Xunit;

namespace TestTask.XUnitTests
{
    public class ServicesTest
    {
        [Fact]
        public void GetOrderNumberTest1()
        { 
            //Arrange
            bool excpected = true;
            //Act
            int actual = new Generator().GetOrderNumber(); 
            //Assert
            Assert.Equal(actual>=1000000 &&actual<= 9999999, excpected); 
        } 
    } 
} 