using Collection.Saving;
using UnityEngine;

namespace Collection.Story
{
    // Something in a level that is used up once and stays used up in the save slot: a chest opened, a barrel
    // broken. Each has an id of its own, given to it in the editor when it is placed (and a new one when it is
    // a copy of another), under which the slot remembers it.
    public abstract class Spendable : MonoBehaviour
    {
        [Tooltip("What the save knows it by. Made by itself; changing it makes saved games forget this one.")]
        [SerializeField] string id;

        public bool Spent => SaveManager.Slot.story.spent.Contains(id);

        protected void Spend()
        {
            StorySave story = SaveManager.Slot.story;
            if (!story.spent.Contains(id))
                story.spent.Add(id);
            SaveManager.MarkDirty();
        }

#if UNITY_EDITOR
        // An id for one that has none, and a new one for a duplicate. Not for the prefab itself, whose instances
        // must each get their own.
        void OnValidate()
        {
            if (Application.isPlaying || !gameObject.scene.IsValid() || UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            bool taken = false;
            if (!string.IsNullOrEmpty(id))
                foreach (Spendable other in FindObjectsByType<Spendable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    if (other != this && other.id == id)
                        taken = true;

            if (string.IsNullOrEmpty(id) || taken)
            {
                id = System.Guid.NewGuid().ToString("N").Substring(0, 12);
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
