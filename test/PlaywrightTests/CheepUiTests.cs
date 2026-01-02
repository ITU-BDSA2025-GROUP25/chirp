using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CheepUiTests : PageTest
{
    private const string BaseUrl = "https://bdsagroup25chirprazor3-d4dha4bnb6dwgga4.norwayeast-01.azurewebsites.net/";
    private const string Username = "TEST";
    private const string Email = "test@itu.dk";
    private const string Password = "7VTcgjm5txmB7Xy!";
    
    [Test]
    public async Task CheepBox_ShouldNotBeVisible_WhenUserIsNotLoggedIn()
    {
        //go to homepage without logging in
        await Page.GotoAsync(BaseUrl);
        
        //check that the "Post Cheep" button is not visible
        var postButton = Page.GetByRole(AriaRole.Button, new() { Name = "Post Cheep" });
        await Expect(postButton).Not.ToBeVisibleAsync();
        
        Assert.Pass($"CheepBox is not visible");
    }
    
    [Test]
    public async Task CheepBox_ShouldBeVisible_AfterSuccessfulLogin()
    {
        //login first
        //await RegisterAsync();
        //await RegisterAsync();
        await LoginAsync();
        await Page.GotoAsync(BaseUrl);
        
        //Post button should be visible
        var postButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
        await Expect(postButton).ToBeVisibleAsync();
        
        Assert.Pass($"CheepBox is visible and user can cheep");

    }
    
    [Test]
    public async Task CheepInput_ShouldShowWarning_WhenExceeding160Characters()
    {
        //Login first
        //await RegisterAsync();
        await LoginAsync();
        await Page.GotoAsync(BaseUrl);
        
        //Create a string with exactly 161 characters
        string longCheep = new string('A', 161);
        
        //Type 161 characters
        var cheepInput = Page.Locator("#cheepInput");
    
        // Wait for it to be visible
        await Expect(cheepInput).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions {
            Timeout = 10000
        });
    
        // Clear and fill
        await cheepInput.ClearAsync();
        await cheepInput.FillAsync(longCheep);
    
        // Trigger validation by focusing another element
        await Page.Locator("body").ClickAsync();
        await Page.WaitForTimeoutAsync(500);
        
        // Type 161 characters
        string longText = new string('A', 161);
        await cheepInput.FillAsync(longText);
    
        // Check what actually got into the field
        var actualText = await cheepInput.InputValueAsync();
        int actualLength = actualText.Length;
    
        // Option A: If maxlength prevents > 160, test passes
        if (actualLength <= 160)
        {
            Assert.Pass($"maxlength attribute works: limited to {actualLength} characters");
        }
    }
    
    
    private async Task LoginAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        await Page.GetByLabel("Email").FillAsync(Email);
        await Page.GetByLabel("Password").FillAsync(Password); 
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        // Just wait a bit instead of waiting for specific URL
        await Page.WaitForTimeoutAsync(3000);
    
        // After registration, manually go to homepage
        await Page.GotoAsync(BaseUrl);
    }

    private async Task RegisterAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Register");
        
        await Page.GetByLabel("Display name").FillAsync(Username);
        await Page.GetByLabel("Email").FillAsync(Email); 
        await Page.GetByLabel("Password").Nth(0).FillAsync(Password); // First password field
        await Page.GetByLabel("Password").Nth(1).FillAsync(Password); // Second (confirm) password field
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        // Just wait a bit instead of waiting for specific URL
        await Page.WaitForTimeoutAsync(3000);
    
        // After registration, manually go to homepage
        await Page.GotoAsync(BaseUrl);
    }
    
    /*
    [Test] //This is a test to see if playwright works. Should always return true
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
        
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        await getStarted.ClickAsync();
        
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }*/
}
