public interface IFollowRepository
{
    Task Follow(string follower, string followee);
    
    Task Unfollow(string follower, string followee);
    
    Task<bool> IsFollowing(string follower, string followee);
    
    Task<List<string>> GetFollowing(string username);
}
