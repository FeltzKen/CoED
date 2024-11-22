// TurnManager.cs
using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }
        private List<IActor> actors = new List<IActor>();       // All actors to manage
        private float baseSpeed;                                // Player's speed as a base reference
        private Dictionary<Vector3Int, IActor> tileReservations = new Dictionary<Vector3Int, IActor>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            baseSpeed = PlayerManager.Instance.Speed; // Assume the player speed is our base speed
        }

        public void RegisterActor(IActor actor)
        {
            if (!actors.Contains(actor))
            {
                actors.Add(actor);
                actor.ActionPoints = 0f; // Initialize action points to zero
               // Debug.Log($"TurnManager: Registered actor {actor.GetType().Name}");
            }
        }

        public void RemoveActor(IActor actor)
        {
            if (actors.Contains(actor))
            {
                actors.Remove(actor);
                Debug.Log($"TurnManager: Removed actor {actor.GetType().Name}");

                // Clear any tile reservations held by the actor
                List<Vector3Int> tilesToRemove = new List<Vector3Int>();
                foreach (var tile in tileReservations)
                {
                    if (tile.Value == actor)
                    {
                        tilesToRemove.Add(tile.Key);
                    }
                }
                foreach (var tile in tilesToRemove)
                {
                    tileReservations.Remove(tile);
                }
            }
        }

        public void PlayerActionTaken()
        {
            foreach (IActor actor in actors)
            {
                // Accumulate action points based on the speed ratio relative to player speed
                float speedRatio = actor.Speed / baseSpeed;
                actor.ActionPoints += speedRatio;

                if (actor.ActionPoints >= 1.0f)
                {
                    // If action points reach or exceed 1, the actor gets to perform an action
                    actor.ActionPoints -= 1.0f; // Decrease by 1 after acting to allow for fractional accumulation
                    actor.PerformAction();
                }
            }
        }

        public bool ReserveTile(Vector3Int tile, IActor actor)
        {
            if (tileReservations.ContainsKey(tile))
            {
                // If the tile is occupied by an enemy and the actor is the player, allow attack
                if (tileReservations[tile] is EnemyAI && actor is PlayerManager)
                {
                    Debug.Log("TurnManager: Tile is occupied by an enemy. Allowing attack.");
                    return true; // Allow interaction but do not reserve the tile
                }

                // If the tile is occupied by the player and the actor is an enemy, allow attack
                if (tileReservations[tile] is PlayerManager && actor is EnemyAI)
                {
                    Debug.Log("TurnManager: Tile is occupied by the player. Allowing enemy attack.");
                    return true; // Allow interaction but do not reserve the tile
                }

                Debug.Log("TurnManager: Tile is reserved. Blocking movement.");
                return false; // Tile is reserved, movement blocked
            }

            tileReservations[tile] = actor;
            return true; // Tile reserved successfully
        }

        public void ReleaseTile(Vector3Int tile)
        {
            if (tileReservations.ContainsKey(tile))
            {
                tileReservations.Remove(tile);
            }
        }

        public bool IsTileReserved(Vector3Int tile)
        {
            return tileReservations.ContainsKey(tile);
        }
    }
}
