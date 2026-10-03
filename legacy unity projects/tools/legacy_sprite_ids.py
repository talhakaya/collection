"""Repairs sprite references that Unity 6 drops when importing a Unity 4 project.

    python legacy_sprite_ids.py "legacy unity projects/herbie/Assets" "Assets/games/Herbie"
    python legacy_sprite_ids.py "legacy unity projects/herbie/Assets" "Assets/games/Herbie" --apply

Run from the repository root AFTER the game's scenes and prefabs have been re-saved as
text (AssetDatabase.ForceReserializeAssets, see PORTING.md). Without --apply it only
reports.

Why: in Unity 4 a single sprite normally has file ID 21300000, recorded in the image's
.meta under fileIDToRecycleName. An image that was renamed in the old project keeps the
old name on 21300000 and gets its real sprite on 21300002 (or higher). Unity 6 gives the
sprite 21300000 again, so every scene and prefab reference to the old ID silently becomes
"None" - no error, the object just has no picture.

This finds those images in the LEGACY metas and repoints references in the ported
.unity/.prefab files to 21300000. Multi-sprite sheets (spriteMode 2) are reported and left
alone: they need their sprites matched by name.
"""
import io
import os
import re
import sys


def main():
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    if len(args) != 2:
        sys.exit(__doc__)
    legacy, port = args
    apply = "--apply" in sys.argv

    odd = {}
    for root, _, files in os.walk(legacy):
        for f in files:
            if not f.endswith(".meta"):
                continue
            text = io.open(os.path.join(root, f), encoding="utf-8", errors="replace").read()
            block = re.search(r"fileIDToRecycleName:\n((?:    \d+: .*\n)*)", text)
            if not block:
                continue
            ids = [(i, n.strip()) for i, n in re.findall(r"    (\d+): (.*)", block.group(1))]
            ids = [(i, n) for i, n in ids if i.startswith("213")]
            if not ids:
                continue
            guid = re.search(r"^guid: (\w+)", text, re.M).group(1)
            base = os.path.splitext(f[:-len(".meta")])[0]
            if re.search(r"spriteMode: 2", text):
                print("SHEET (left alone): %s %s" % (base, ids))
                continue
            mine = [i for i, n in ids if n == base]
            if mine != ["21300000"]:
                odd[guid] = (base, ids)

    for guid, (base, ids) in sorted(odd.items(), key=lambda kv: kv[1][0]):
        print("%-20s %s %s" % (base, guid, ids))

    total = 0
    for root, _, files in os.walk(port):
        for f in files:
            if not (f.endswith(".unity") or f.endswith(".prefab") or f.endswith(".asset")):
                continue
            path = os.path.join(root, f)
            text = io.open(path, encoding="utf-8", errors="replace", newline="").read()
            if not text.startswith("%YAML"):
                print("NOT TEXT (re-save it first): " + path)
                continue
            count = [0]

            def fix(m):
                if m.group(2) in odd and m.group(1) != "21300000":
                    count[0] += 1
                    return "{fileID: 21300000, guid: " + m.group(2)
                return m.group(0)

            new = re.sub(r"\{fileID: (213\d+), guid: (\w+)", fix, text)
            if count[0]:
                print("%s: %d reference(s)" % (f, count[0]))
                total += count[0]
                if apply:
                    io.open(path, "w", encoding="utf-8", newline="").write(new)

    print("%d reference(s) %s" % (total, "repointed" if apply else "to repoint (dry run; add --apply)"))


if __name__ == "__main__":
    main()
