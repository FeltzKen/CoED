// IActor.cs
using UnityEngine;

namespace YourGameNamespace
{
    public interface IActor
    {
        void Act();
        void TakeDamage(int damage);
        bool IsActionComplete();
    }
}


