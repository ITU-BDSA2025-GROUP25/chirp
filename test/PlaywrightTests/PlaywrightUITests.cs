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
