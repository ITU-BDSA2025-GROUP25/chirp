using Xunit;

namespace Chirp.Tests;

//This test is ony used for making sure a simple test will run normally without any reliance on data
//==================================================================================================
public class SimpleTest
{
    [Fact]
    public void Test1()
    {
        Assert.True(1 == 1);
    }
}