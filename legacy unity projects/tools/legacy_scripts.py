"""Brings a legacy (Unity 4-era) game's scripts up to Unity 6 and into the collection.

    python legacy_scripts.py "Assets/games/Herbie" Herbie

Run from the repository root, on the COPY under Assets/games/<Name>, before Unity
imports it. Only mechanical rewrites are done here; see PORTING.md for what is left to
do by hand. It reports what it changed per file so the result can be checked.

Rewrites:
  - Input.GetAxis/GetAxisRaw/GetButton*/GetMouseButton*/mousePosition
        -> TaloketoInputManager.* (adds "using Collection.Controls;")
  - the component shortcut properties Unity 5 removed:
        rigidbody2D, collider2D, rigidbody, collider, renderer, audio, camera
        -> GetComponent<...>()
  - Rigidbody2D .velocity -> .linearVelocity (only directly after a GetComponent)
  - Application.LoadLevel(0) -> SceneManager.LoadScene(the active scene's path)
  - everything wrapped in "namespace Games.<Name>"

Not done (grep for these afterwards): Application.Quit, Application.LoadLevel with
anything but 0, Input.GetKey(KeyCode...), OnGUI/GUIText/GUITexture, and comparisons
of an axis with exactly 1 or -1.
"""
import io
import os
import re
import sys

SHORTCUTS = [
    ("rigidbody2D", "Rigidbody2D"),
    ("collider2D", "Collider2D"),
    ("rigidbody", "Rigidbody"),
    ("collider", "Collider"),
    ("renderer", "Renderer"),
    ("audio", "AudioSource"),
    ("camera", "Camera"),
]


def convert(src, namespace, notes):
    def note(what, n):
        if n:
            notes.append("%s x%d" % (what, n))

    out = src.replace("\r\n", "\n")

    out, n = re.subn(
        r"(?<![\w.])Input\.(GetAxisRaw|GetAxis|GetButtonDown|GetButtonUp|GetButton|"
        r"GetMouseButtonDown|GetMouseButtonUp|GetMouseButton|mousePosition)\b",
        r"TaloketoInputManager.\1", out)
    note("input", n)
    uses_input = n > 0

    for prop, component in SHORTCUTS:
        # "x.rigidbody2D" and a bare "rigidbody2D." / "rigidbody2D;" on this object. A
        # bare word is only rewritten when it is used as an object (followed by a dot),
        # so locals or fields that happen to share the name are left alone.
        # A script that declares its own member of that name ("public AudioClip[] audio;")
        # is not using the shortcut at all.
        if re.search(r"[\w>\]]\s+%s\s*[=;]" % prop, out):
            notes.append("%s declared here - left alone" % prop)
            continue
        out, n = re.subn(r"\.%s\b(?!\s*\()" % prop, ".GetComponent<%s>()" % component, out)
        note("." + prop, n)
        out, n = re.subn(r"(?<![\w.<])%s\." % prop, "GetComponent<%s>()." % component, out)
        note(prop, n)

    out, n = re.subn(r"GetComponent<Rigidbody2D>\(\)\.velocity\b",
                     "GetComponent<Rigidbody2D>().linearVelocity", out)
    note("velocity", n)

    out, n = re.subn(r"Application\.LoadLevel\s*\(\s*0\s*\)",
                     "SceneManager.LoadScene(SceneManager.GetActiveScene().path)", out)
    note("LoadLevel(0)", n)
    uses_scenes = n > 0

    if re.search(r"^\s*namespace\s+\S", out, re.M):
        notes.append("already in a namespace - not wrapped")
        return out

    lines = out.split("\n")
    body_start = 0
    for i, line in enumerate(lines):
        t = line.strip()
        if t == "" or t.startswith("using ") or t.startswith("//"):
            body_start = i + 1
            continue
        break

    head = [l for l in lines[:body_start] if l.strip() != ""]
    if uses_scenes and "using UnityEngine.SceneManagement;" not in head:
        head.append("using UnityEngine.SceneManagement;")
    if uses_input and "using Collection.Controls;" not in head:
        head.append("using Collection.Controls;")

    body = lines[body_start:]
    while body and body[-1].strip() == "":
        body.pop()

    result = head + ["", "namespace " + namespace, "{"]
    for l in body:
        result.append(("\t" + l) if l.strip() != "" else "")
    result.append("}")
    return "\n".join(result) + "\n"


def main():
    if len(sys.argv) != 3:
        sys.exit(__doc__)
    folder, name = sys.argv[1], sys.argv[2]
    namespace = "Games." + name.replace(" ", "")

    leftovers = re.compile(
        r"Application\.Quit|Application\.LoadLevel|Input\.GetKey|OnGUI|GUIText|GUITexture|"
        r"GetAxis(Raw)?\([^)]*\)\s*==\s*-?1\b")

    for root, _, files in os.walk(folder):
        for f in sorted(files):
            if not f.endswith(".cs"):
                continue
            path = os.path.join(root, f)
            with io.open(path, encoding="utf-8-sig", newline="") as fh:
                src = fh.read()
            notes = []
            out = convert(src, namespace, notes)
            with io.open(path, "w", encoding="utf-8", newline="") as fh:
                fh.write(out)
            todo = sorted(set(m.group(0) for m in leftovers.finditer(out)))
            if notes or todo:
                print("%-28s %s%s" % (f, ", ".join(notes),
                                      ("   BY HAND: " + "; ".join(todo)) if todo else ""))


if __name__ == "__main__":
    main()
