using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.Tests.Integration;

[TestFixture]
public class ForgetMeUiTests : PageTest
{
    private const string BaseUrl = "https://bdsagroup25chirprazor3-d4dha4bnb6dwgga4.norwayeast-01.azurewebsites.net/";
    private const string Username = "TEST";
    private const string Email = "test@itu.dk";
    private const string Password = "7VTcgjm5txmB7Xy!";
    
    [Test]
    public async Task ForgetMe_Button()
    {
        //await Register(Email,Username, Password);
        await Login(Email,Password);
        await Page.GotoAsync(BaseUrl);
        
        // Navigate to Personal Data page (where the button is)
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Manage/DeletePersonalData");
        
        Console.WriteLine($"Current URL: {Page.Url}");
        Console.WriteLine($"Page title: {await Page.TitleAsync()}");
        
        /*// Take screenshot for debugging
        await Page.ScreenshotAsync(new PageScreenshotOptions { 
            Path = "personal-data-page.png" 
        });*/
        
        var deleteButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Forget me!" });
        
        await Expect(deleteButton).ToBeVisibleAsync();
        
    }
    
    /*[Test] Follow list can not be seen on the users profile
    public async Task ProfilePage_ShowsFollowingList()
    {
        await Login("test@example.com", "Password123!");
        
        // First, follow someone if not already following
        await Page.GotoAsync($"{BaseUrl}/author/Helge");
        var followButton = Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).First;
        if (await followButton.IsVisibleAsync())
        {
            await followButton.ClickAsync();
            await Page.WaitForTimeoutAsync(1000);
        }
        
        // Go to profile page
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Manage");
        
        // Check for following section
        var followingText = Page.GetByText("Following")
            .Or(Page.GetByText("Following list"))
            .Or(Page.GetByText("People you follow"))
            .First;
        
        await Expect(followingText).ToBeVisibleAsync();
        
        // Check Helge is in the following list
        var helgeInList = Page.GetByText("Helge").First;
        await Expect(helgeInList).ToBeVisibleAsync();
        
        // Check link to Helge's profile exists
        var helgeLink = Page.Locator("a[href*='Helge']").First;
        await Expect(helgeLink).ToBeVisibleAsync();
        
        var href = await helgeLink.GetAttributeAsync("href");
        Assert.That(href, Is.Not.Null.And.Contains("/author/Helge"),
            "Link should point to Helge's profile");
    }*/
    
    
    [Test]
    public async Task LoginPage_HasGitHubOAuthButton()
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        // Check for GitHub button
        var githubButton = Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })
            .Or(Page.Locator("button[value='GitHub']"))
            .First;
        
        await Expect(githubButton).ToBeVisibleAsync();
        
        // Check it's inside the OAuth section
        var oauthSection = Page.GetByText("Use another service to log in");
        await Expect(oauthSection).ToBeVisibleAsync();
        
        // Check form action points to ExternalLogin
        var oauthForm = Page.Locator("form#external-account");
        await Expect(oauthForm).ToBeVisibleAsync();
        
        var formAction = await oauthForm.GetAttributeAsync("action");
        Assert.That(formAction, Is.Not.Null.And.Contains("ExternalLogin"),
            "OAuth form should point to ExternalLogin endpoint");
    }
    
    [Test]
    public async Task PersonalDataPage_HasDownloadOption()
    {
        await Login(Email, password: Password);
        
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Manage/PersonalData");
        
        // Should have download button/link
        var downloadButton = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Download" });
        
        await Expect(downloadButton).ToBeVisibleAsync();
        
        // Page should mention personal data
        var pageText = await Page.TextContentAsync("body");
        Assert.That(pageText, Contains.Substring("Personal").Or.Contains("Data"));
    }
    
    // Helper methods
    private async Task Login(string email, string password)
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        
        await Page.GetByLabel("Email").FillAsync(email);
        await Page.GetByLabel("Password").FillAsync(password); 
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        // Just wait a bit instead of waiting for specific URL
        await Page.WaitForTimeoutAsync(3000);
    
        // After registration, manually go to homepage
        await Page.GotoAsync(BaseUrl);
    }
    
    private async Task Register(string email, string displayName, string password)
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Register");
        
        await Page.GetByLabel("Display name").FillAsync(displayName);
        await Page.GetByLabel("Email").FillAsync(email); 
        await Page.GetByLabel("Password").Nth(0).FillAsync(password); // First password field
        await Page.GetByLabel("Password").Nth(1).FillAsync(password); // Second (confirm) password field
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        // Just wait a bit instead of waiting for specific URL
        await Page.WaitForTimeoutAsync(3000);
    
        // After registration, manually go to homepage
        await Page.GotoAsync(BaseUrl);
    }
    
}