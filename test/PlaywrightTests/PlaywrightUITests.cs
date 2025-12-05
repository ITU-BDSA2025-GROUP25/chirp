using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Chirp.Razor.Tests.PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private const string BaseUrl = "https://bdsagroup25chirprazor3-d4dha4bnb6dwgga4.norwayeast-01.azurewebsites.net/";
    private const string username = "testuser@itu.dk";
    private const string password = "EArzCzGhRe4Hryn!";
    
    [Test]
    public async Task CheepBox_ShouldNotBeVisible_WhenUserIsNotLoggedIn()
    {
        //go to homepage without logging in
        await Page.GotoAsync(BaseUrl);
        
        //look for cheep input box
        var cheepInput = Page.GetByPlaceholder("What's on your mind?");
        
        //cheep box should NOT be visible
        await Expect(cheepInput).Not.ToBeVisibleAsync();
        
        //check that the "Post Cheep" button is not visible
        var postButton = Page.GetByRole(AriaRole.Button, new() { Name = "Post Cheep" });
        await Expect(postButton).Not.ToBeVisibleAsync();
    }
    
    [Test]
    public async Task CheepBox_ShouldBeVisible_AfterSuccessfulLogin()
    {
        // Arrange
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        // Act: Fill in login credentials (update with your actual login selectors)
        await Page.GetByLabel("Email").FillAsync(username); // Update selector
        await Page.GetByLabel("Password").FillAsync(password); // Update selector
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        // Wait for navigation to complete
        await Page.WaitForURLAsync(BaseUrl);
        
        // Assert: Cheep input box should be visible
        /*var cheepInput = Page.GetByPlaceholder("What's on your mind " + username + "?");
        await Expect(cheepInput).ToBeVisibleAsync();*/
        
        // Assert: Post button should be visible
        var postButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
        await Expect(postButton).ToBeVisibleAsync();
    }
    /*
    [Test]
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
