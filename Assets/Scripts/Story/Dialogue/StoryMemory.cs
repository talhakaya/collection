using System.Collections.Generic;
using Collection.Saving;

namespace Collection.Story
{
    // Things the story remembers in the save slot being played, each a name and a value: which option the player
    // picked somewhere, mostly. Yarn's own variables are not used.
    public static class StoryMemory
    {
        public static void Set(string name, string value)
        {
            StorySave save = SaveManager.Slot.story;
            int at = save.memoryKeys.IndexOf(name);
            if (at >= 0 && at < save.memoryValues.Count)
            {
                save.memoryValues[at] = value;
            }
            else
            {
                save.memoryKeys.Add(name);
                save.memoryValues.Add(value);
            }
            SaveManager.MarkDirty();
        }

        // Null when nothing has been remembered under the name.
        public static string Get(string name)
        {
            StorySave save = SaveManager.Slot.story;
            int at = save.memoryKeys.IndexOf(name);
            return at >= 0 && at < save.memoryValues.Count ? save.memoryValues[at] : null;
        }

        public static bool Is(string name, string value)
        {
            return Get(name) == value;
        }
    }
}
