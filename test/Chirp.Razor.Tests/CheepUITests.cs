using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Chirp.Razor.Tests;
using NUnit.Framework;


namespace Chirp.Razor.Tests;

public class CheepUITests : PageTest
{
    private const string BaseUrl = "https://bdsagroup25chirprazor3-d4dha4bnb6dwgga4.norwayeast-01.azurewebsites.net";
    
    [Test]
    public async Task CheepBox_OnlyAppears_WhenUserIsLoggedIn()
    {
        //Navigate to public page
        await Page.GotoAsync($"{BaseUrl}/");

        //Cheep box should NOT be visible when not logged in
        await Expect(Page.Locator(".cheepbox")).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='text']")).Not.ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='submit'][value='Share']")).Not.ToBeVisibleAsync();

        //Now log in
        await LoginTestUser();

        //Navigate to public page again
        await Page.GotoAsync($"{BaseUrl}/");

        //Cheep box SHOULD be visible when logged in
        await Expect(Page.Locator(".cheepbox")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='text']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type='submit'][value='Share']")).ToBeVisibleAsync();
        
        //Verify the greeting message
        var greeting = Page.Locator("h3:has-text(\"What's on your mind\")");
        await Expect(greeting).ToBeVisibleAsync();
    }
    
    private async Task LoginTestUser()
    {
        await Page.GotoAsync($"{BaseUrl}/Account/Login");
        
        //Fill login form. If run on localhost, change the Email and Password
        await Page.Locator("input[name='Input.Email']").FillAsync("wile@itu.dk");
        await Page.Locator("input[name='Input.Password']").FillAsync("123123We!");
        await Page.Locator("button[type='submit']").ClickAsync();

        // Wait for login to complete
        await Expect(Page.Locator(".cheepbox")).ToBeVisibleAsync();
    }
}