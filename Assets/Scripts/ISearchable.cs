using UnityEngine;

namespace CoED
{
    /// <summary>
    /// Implement this on anything that can be "searched" by the player.
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Called when the player searches and finds this object.
        /// </summary>
        void OnSearch();
    }
}
