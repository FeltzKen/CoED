// IActor.cs
using UnityEngine;

namespace YourGameNamespace
{
    public interface IActor
    {
        float Speed { get; }            // Speed of the actor
        float ActionPoints { get; set; } // Accumulated action points
        bool IsActionComplete();        // Checks if the action is complete
        void Act();                     // Plans and executes an action
        void PerformAction();           // Executes the current action
    }
}
