using AnimeServices.Services;
using Blazored.LocalStorage;
using Moq;

namespace AnimeServices.UnitTests
{
    public class AnimeServiceTests
    {
        [Fact]
        public async Task ToggleFavoriteAsync_AddAndRemoveFavoriteAnime()
        {
            // Arrange
            var localStorageMock = new Mock<ILocalStorageService>();
            var animeService = new AnimeService(null, localStorageMock.Object);

            // Act
            // Toggle an anime ID that is not in the initial favorites
            var newAnimeId = 4;
            await animeService.ToggleFavoriteAsync(newAnimeId);

            // Assert
            // Ensure the new anime ID is added to the favorites
            var updatedFavorites = await animeService.GetFavoritesAsync();
            Assert.Equal(updatedFavorites, new HashSet<int>(4));

            // Toggle the same anime ID again to remove it from the favorites
            await animeService.ToggleFavoriteAsync(newAnimeId);

            // Assert
            // Ensure the anime ID is removed from the favorites
            updatedFavorites = await animeService.GetFavoritesAsync();
            Assert.Equal(updatedFavorites, new HashSet<int>());
        }
    }
}
