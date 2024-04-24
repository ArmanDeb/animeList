using AnimeServices.Services;
using Blazored.LocalStorage;
using animeList.Pages;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using Moq;

namespace AnimeServices.IntegrationTests
{
	public class IntegrationTest
	{
		TestContext ctx = new TestContext();

		[Fact]
		public async Task TestLoadAnimeList()
		{
			//Arrange
            var localStorageMock = new Mock<ILocalStorageService>();
            var animeService = new AnimeService(new HttpClient(), localStorageMock.Object);
			ctx.Services.AddSingleton(animeService);
			var page = ctx.RenderComponent<AnimeList>();

			//Act
			await Task.Delay(10000);
			page.Render();

			//Assert
			var listCount = page.FindAll("li").Count();
			Assert.True(listCount > 5);
		}
	}
}
