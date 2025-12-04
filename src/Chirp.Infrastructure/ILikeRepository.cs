public interface ILikeRepository
{
    Task LikeCheep(int cheepId, string username);
    Task UnlikeCheep(int cheepId, string username);
    Task<bool> HasLiked(int cheepId, string username);
    Task<int> CountLikes(int cheepId);
}