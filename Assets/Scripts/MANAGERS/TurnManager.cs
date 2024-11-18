using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using YourGameNamespace;

namespace YourGameNamespace
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance { get; private set; }

        private bool isProcessingTurn;
        private List<IActor> actors = new List<IActor>();
        private int currentActorIndex = 0;


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

        private void Update()
        {
            if (!isProcessingTurn && actors.Count > 0)
            {
                StartCoroutine(ProcessTurn());
            }
        }

        public void RegisterActor(IActor actor)
        {
            if (!actors.Contains(actor))
            {
                actors.Add(actor);
                Debug.Log($"TurnManager: Registered actor {actor.GetType().Name}");
            }
        }
        public void ActionCompleted(IActor actor)
        {
            Debug.Log($"TurnManager: Action completed for {actor.GetType().Name}");
            // Any additional logic after an actor's action is completed
        }        private IEnumerator ProcessTurn()
        {
            isProcessingTurn = true;

            if (currentActorIndex >= actors.Count)
            {
                currentActorIndex = 0; // Start a new round
                Debug.Log("TurnManager: Starting a new round.");
            }

            IActor currentActor = actors[currentActorIndex];
            currentActorIndex++;

            if (currentActor != null)
            {
                Debug.Log($"TurnManager: Processing action for {currentActor.GetType().Name}");
                currentActor.Act();

                // Wait until the actor's action is complete
                yield return new WaitUntil(() => currentActor.IsActionComplete());
            }
            isProcessingTurn = false; // Ensure this is set to false

        }

       public void RemoveActor(IActor actor)
        {
            if (actors.Contains(actor))
            {
                actors.Remove(actor);
                Debug.Log($"TurnManager: Removed actor {actor.GetType().Name}");

                // Adjust the current actor index if necessary
                if (currentActorIndex > 0)
                {
                    currentActorIndex--;
                }
            }
        }
    }
}
