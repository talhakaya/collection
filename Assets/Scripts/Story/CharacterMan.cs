using System;
using System.Collections.Generic;
using Collection.Saving;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Collection.Story
{
    // The main character's look: kept in the story part of the collection's save file (SaveManager.Slot.story).
    // Options are saved by catalog ID (not list position or display name), so saves survive reordering and renaming
    // catalog entries.
    //
    // The character creator is a development tool, not part of the game: it is opened with F2 in the editor and in
    // development builds, or from this component's context menu. Confirming it saves the look to the slot.
    public class CharacterMan : MonoBehaviour
    {
        public CharacterAppearance player;
        public CharacterCreator creator;

        // Every catalog choice on the appearance: save key, option IDs, and the index field.
        IEnumerable<(string key, List<string> ids, Func<int> get, Action<int> set)> Options()
        {
            var a = player;
            var c = a.catalog;
            List<string> Colors(List<CharacterCatalog.ColorOption> list) => CharacterCatalog.Ids(list);
            List<string> Clothing(List<CharacterCatalog.ClothingOption> list) => CharacterCatalog.Ids(list);

            yield return ("skin", Colors(c.skinColors), () => a.skinColor, v => a.skinColor = v);
            yield return ("top", Clothing(c.tops), () => a.topType, v => a.topType = v);
            yield return ("topColor", Colors(c.clothColors), () => a.shirtColor, v => a.shirtColor = v);
            yield return ("bottom", Clothing(c.bottoms), () => a.bottomType, v => a.bottomType = v);
            yield return ("pantsColor", Colors(c.clothColors), () => a.pantsColor, v => a.pantsColor = v);
            yield return ("shoesColor", Colors(c.clothColors), () => a.shoesColor, v => a.shoesColor = v);
            yield return ("jacket", Clothing(c.jackets), () => a.jacketType, v => a.jacketType = v);
            yield return ("jacketColor", Colors(c.clothColors), () => a.jacketColor, v => a.jacketColor = v);
        }

        void Start()
        {
            Restore();
        }

        void Update()
        {
            if (!Debug.isDebugBuild)
                return;
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.f2Key.wasPressedThisFrame)
                OpenCreator();
        }

        [ContextMenu("Open Character Creator")]
        public void OpenCreator()
        {
            if (Application.isPlaying && !creator.isOpen)
                creator.Open(player, Capture);
        }

        // Writes the look to the slot being played.
        public void Capture()
        {
            StorySave save = SaveManager.Slot.story;
            save.characterSaved = true;
            save.height = player.height;
            save.fatness = player.fatness;
            save.optionKeys.Clear();
            save.optionIds.Clear();
            foreach (var (key, ids, get, _) in Options())
            {
                int index = get();
                if (index >= 0 && index < ids.Count)
                {
                    save.optionKeys.Add(key);
                    save.optionIds.Add(ids[index]);
                }
            }
            SaveManager.MarkDirty();
        }

        // A slot with no look saved keeps the one the character has in the scene.
        public void Restore()
        {
            StorySave save = SaveManager.Slot.story;
            if (!save.characterSaved)
                return;

            player.height = save.height;
            player.fatness = save.fatness;
            foreach (var (key, ids, _, set) in Options())
            {
                // IDs missing from the catalog (e.g. a removed option) keep the current choice.
                int at = save.optionKeys.IndexOf(key);
                int index = at >= 0 && at < save.optionIds.Count ? ids.IndexOf(save.optionIds[at]) : -1;
                if (index >= 0)
                    set(index);
            }
            player.Apply();
        }
    }
}
